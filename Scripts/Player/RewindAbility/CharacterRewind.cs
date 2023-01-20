using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using System;
using FMOD.Studio;
using FMODUnity;

public class CharacterRewind : Rewind
{
    [SerializeField] private float _rewindDuration = 10f;
    [SerializeField] private Ghost _ghostPrefab;
    [SerializeField] private GameObject _camera;
    [SerializeField] private PooledRewind _locatorPrefab;

    [Header("Rewind Shader")] 
    [SerializeField] private Material _rewindShader;
    [SerializeField] private float _fadeTime;
    private float _lastOpacity;

    //Rewind Controll
    private bool _canRecord = true;
    private PausingController _pausingController;
    private Ghost _ghost;
    private PlayerDialogue _playerDialogue;

    //public properties
    public float RewindDuration => _rewindDuration;


    private List<Vector3> _recordedPositions = new List<Vector3>((int)(10 / .02));
    private List<Vector3> _recordedRotations = new List<Vector3>((int)(10 / .02));
    private List<Vector3> _recordedCameraPositions = new List<Vector3>((int)(10 / .02));
    private List<Vector3> _recordedCameraRotations = new List<Vector3>((int)(10 / .02));
    private List<float> _recordedJumps = new List<float>();
    private List<float> _recordedPauses = new List<float>();
    private List<float> _recordedJumpCancels = new List<float>();
    private List<float> _recordedUnpauses = new List<float> ();
    private LinkedList<IPausable> _initalpasuedQueue = new LinkedList<IPausable>();

    //Ghost values
    private List<Vector3> _ghostPositions = new List<Vector3>((int)(10 / .02));
    private List<Vector3> _ghostRotations = new List<Vector3>((int)(10 / .02));
    private List<Vector3> _ghostCameraPositions = new List<Vector3>((int)(10 / .02));
    private List<Vector3> _ghostCameraRotations = new List<Vector3>((int)(10 / .02));
    private PooledRewind _locator;

    //rewind Audio
    private static EventInstance BT;

    private void Awake()
    {
        _rewindShader.SetFloat("_FullScreenIntensity", 0);
        _playerDialogue = GetComponent<PlayerDialogue>();

        //initialize rewind sound snapshot
        BT = RuntimeManager.CreateInstance("snapshot:/Rewind");
        //BT.start();
        //BT.setParameterByName("RewindIntensity", 0);
    }

    private void Start()
    {
        _pausingController = FindObjectOfType<PausingController>(true);
    }

    private void FixedUpdate()
    {
        //Recording
        if (IsRecording)
        {
            RecordMovementUpdate(_recordedPositions, _ghostPositions, transform.position);
            RecordMovementUpdate(_recordedRotations, _ghostRotations, transform.rotation.eulerAngles);
            RecordMovementUpdate(_recordedCameraPositions, _ghostCameraPositions, _camera.transform.position);
            RecordMovementUpdate (_recordedCameraRotations, _ghostCameraRotations, _camera.transform.rotation.eulerAngles);
            TimeElapsed += Time.fixedDeltaTime;
            if (TimeElapsed > _rewindDuration) ToggleRecord(false);
        }

        //REWINDING
        else if (IsRewinding)
        {
            if (_recordedPositions.Count > 0)
            {
                transform.position = _recordedPositions[0];
                _recordedPositions.RemoveAt(0);
            }
            if (_recordedRotations.Count > 0)
            {
                transform.rotation = Quaternion.Euler(_recordedRotations[0]);
                _recordedRotations.RemoveAt(0);
            }

            TimeElapsed -= Time.fixedDeltaTime;
            if(TimeElapsed < 0)
            {
                SummonGhost();
                TimeElapsed = 0;
                StopRewind();
            }
        }
    }

    public void ToggleRecord(bool startRecord)
    {
        if (_canRecord)
        {
            if (!IsRecording && startRecord)
            {
                StartRecord();
                BT.start();
            }

            else if (IsRecording && !startRecord) StopRecord();
        }
        else if (_ghost != null && !_ghost.IsPooled) _ghost.KillGhost();
    }

    public override void StopRecord(bool rewind = true)
    {
        if (!IsRecording) return;
        IsRecording = false;

        if (rewind) 
        {
            SetCanRecord(false);
            FinishRecord.Invoke();
            StartRewind();
        } 

        else
        {
            ClearAll();
            TimeElapsed = 0;
            BT.stop(FMOD.Studio.STOP_MODE.IMMEDIATE);
        }
    }

    public override void StartRewind()
    {
        base.StartRewind();
        WorldTimeController.ChangeTimeScale(WorldTimeController.TimeSpeedUpMultiplier);
        StartCoroutine(UpdateShader(0, 0.5f));
        //BT.setParameterByName("RewindIntensity", 1);
        WorldState.SetWorldIsRewind(true);
    }

    public override void StopRewind()
    {
        base.StopRewind();
        WorldTimeController.ChangeTimeScale(1);
        RewriteQueue(_initalpasuedQueue, _pausingController._pasuedLinkedList);
        StartCoroutine(UpdateShader(0.5f, 0));
        DespawnLocator();
        _recordedCameraPositions.Clear();
        _recordedCameraRotations.Clear();
        //BT.setParameterByName("RewindIntensity", 0);
        BT.stop(FMOD.Studio.STOP_MODE.IMMEDIATE);
        WorldState.SetWorldIsRewind(false);
    }

    public void GamePaused(bool isPaused)
    {
        if (isPaused)
        {
            _lastOpacity = _rewindShader.GetFloat("_FullScreenIntensity");
            BT.setParameterByName("RewindIntensity", 0);
            _rewindShader.SetFloat("_FullScreenIntensity", 0);
        }
        else if(!isPaused && WorldState.WorldIsRewind)
        {
            BT.setParameterByName("RewindIntensity", 1);
            _rewindShader.SetFloat("_FullScreenIntensity", _lastOpacity);
        }
    }

    public override void StartRecord()
    {
        base.StartRecord();
        BT.setParameterByName("RewindIntensity", 1);
        RewriteQueue(_pausingController._pasuedLinkedList, _initalpasuedQueue);
        _locator = PoolSystem.Instance.Get(_locatorPrefab, new Vector3(transform.position.x, transform.position.y + 0.01f, transform.position.z), transform.rotation) as PooledRewind;
    }

    public void SetCanRecord(bool canRecord)
    {
        _canRecord = canRecord;
    }

    private void SummonGhost()
    {
        _ghost = PoolSystem.Instance.Get(_ghostPrefab, transform.position, transform.rotation) as Ghost;
        GetComponent<MeshTrail>().DeactivateRewindTrail(_ghost.transform);
        RecordedPlayerInputs packagedInputs = new RecordedPlayerInputs(_ghostPositions, _ghostRotations, _ghostCameraPositions, _ghostCameraRotations, _recordedJumps, _recordedJumpCancels, _recordedPauses, _recordedUnpauses);
        _ghost.BeginGhostMovement(packagedInputs);
        _ghost.OnFinishedReplay.AddListener(SetCanRecord);
        _ghost.OnKilled.AddListener(GhostDeathVoice);
    }

    private void GhostDeathVoice()
    {
        if (UnityEngine.Random.Range(0, 5) == 0) _playerDialogue.GhostDeath();
    }

    public void DespawnLocator()
    {
        if (_locator != null && !_locator.IsPooled) _locator.PoolParticles();
    }

    private IEnumerator UpdateShader(float start, float end)
    {
        float timeElapsed = 0f;
        while(timeElapsed < _fadeTime)
        {
            timeElapsed += Time.deltaTime;
            _rewindShader.SetFloat("_FullScreenIntensity", Mathf.Lerp(start, end, timeElapsed / _fadeTime));
            yield return null;
        }
    }

    private void RecordMovementUpdate(List<Vector3> recordedMovements, List<Vector3> ghostMovements, Vector3 value)
    {
        if (recordedMovements.Count > (int)(10 / Time.fixedDeltaTime)) recordedMovements.RemoveAt(recordedMovements.Count - 1);
        recordedMovements.Insert(0, value);
        ghostMovements.Add(value);
    }

    private void RewriteQueue(LinkedList<IPausable> newQueue, LinkedList<IPausable> originalQueue)
    {
        originalQueue.Clear();

        foreach(IPausable pausable in newQueue)
        {
            originalQueue.AddLast(pausable);
        }
    }

    public void OnTimePaused()
    {
        if (IsRecording) _recordedPauses.Add(TimeElapsed);
    }

    public void OnJumpedPerformed()
    {
        if (IsRecording) _recordedJumps.Add(TimeElapsed);
    }

    public void OnJumpedCanceled()
    {
        if (IsRecording) _recordedJumpCancels.Add(TimeElapsed);
    }

    public void OnUnpausedTime()
    {
        if (IsRecording) _recordedUnpauses.Add(TimeElapsed);
    }

    public override void ClearAll()
    {
        base.ClearAll();
        _recordedCameraPositions.Clear();
        _recordedCameraRotations.Clear();
        _recordedJumps.Clear();
        _recordedJumpCancels.Clear();
        _recordedPauses.Clear();
        _recordedUnpauses.Clear();
        _recordedPositions.Clear();
        _recordedRotations.Clear();
        _ghostCameraPositions.Clear();
        _ghostCameraRotations.Clear();
        _ghostPositions.Clear();
        _ghostRotations.Clear();
    }

    public void ResetRewind()
    {
        TimeElapsed = 0;
    }
}
