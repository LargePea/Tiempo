using Sirenix.OdinInspector;
using System.Collections;
using UnityEngine;
using UnityEngine.Events;

public class Projectile : MonoBehaviour, IPausable
{
    [Header("Projectile")]
    [FoldoutGroup("Projectile")] [SerializeField] private Transform _startingTransform;
    [FoldoutGroup("Projectile")] [SerializeField] private float _pointReachedDistance = 0.2f;

    [Header("Highlight Meshes")]
    [FoldoutGroup("Time Pause")] [SerializeField] private Renderer[] _highlightabledRenderers;
    [FoldoutGroup("Time Pause")] [SerializeField] private Material _defaultMaterial;
    [FoldoutGroup("Time Pause")] [SerializeField] private Material _highlightMaterial;

    [Header("Flashing Highlight Shader")]
    [FoldoutGroup("Time Pause")] [SerializeField] private float _speedUpStandard = 3.0f;
    [FoldoutGroup("Time Pause")] [SerializeField] private float _defaultFlashing = 2.0f;
    [FoldoutGroup("Time Pause")] [SerializeField] private float _speedUpFlashing = 8.0f;

    [Header("Pausing")]
    [FoldoutGroup("Time Pause")] [SerializeField] private UnityEvent OnResume = new UnityEvent();
    [FoldoutGroup("Time Pause")] [SerializeField] private UnityEvent<GameObject> OnResumeUpdateUI = new UnityEvent<GameObject>();

    private Vector3 _destination;
    private bool _canMove;
    private float _speed;

    public bool CanMove => _canMove;
    public UnityEvent OnArrival;
    public UnityEvent OnCollision;

    // private
    private Rigidbody _rb;
    private Collider _collider; 

    public bool IsPaused { get; set; }
    public bool IsPausable { get; set; } = true;
    Renderer IPausable._highlightabledRenderers { get; set; }

    private IEnumerator _stopMovingCo;
    private float _pauseTime;
    private float _pausedTimeElapsed;

    //rewind variables
    private float _initalPausedTimeElapsed;
    private bool _initalIsPaused;
    private bool _initalCanMove;

    protected void Awake()
    {
        _rb = GetComponent<Rigidbody>();
        _collider = GetComponent<Collider>();
    }

    public void Init(float distance, float speed)
    {
        _destination = _startingTransform.position + transform.forward * distance;
        _speed = speed;
        _canMove = true;
        _collider.enabled = true;
    }


    private void FixedUpdate()
    {    // Time Pausing
        if (IsPaused || !_canMove)
        {
            _rb.velocity = Vector3.zero;
            return;
        }
        else _rb.velocity = transform.forward * _speed;

        // checks if point is reached
        float distance = Vector3.Distance(transform.position, _destination);
        if (distance < _pointReachedDistance) Arrive();

        // move the platform
        _rb.velocity = transform.forward * _speed;

    }

    // set the debug visual line
    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position, 0.2f);
        Gizmos.color = Color.green;
        Gizmos.DrawLine(transform.position, _destination);
    }

    // hide the disc
    public void Arrive()
    {
        _collider.enabled = false;
        if(_canMove) OnCollision.Invoke();
        OnArrival.Invoke();
        _canMove = false;
        transform.SetPositionAndRotation(_startingTransform.position, _startingTransform.rotation);
    }

    public void SaveState()
    {
        _initalCanMove = _canMove;
        _initalIsPaused = IsPaused;
        _initalPausedTimeElapsed = _pausedTimeElapsed;
    }

    public void ReloadState()
    {
        IsPaused = _initalIsPaused;
        _pausedTimeElapsed = _initalPausedTimeElapsed;
        _canMove = _initalCanMove;

        if (IsPaused)
        {
            StopAction(_pauseTime);
        }
    }

    // set the destructible disc here
    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.CompareTag("Player")) return;
        OnCollision.Invoke();
        Arrive();
    }

    public void StopAction(float stopTime)
    {
        _stopMovingCo = StopMoving(stopTime);
        IsPaused = true;
        IsPausable = false;

        if (_stopMovingCo != null) StopCoroutine(_stopMovingCo);
        StartCoroutine(_stopMovingCo);
    }

    public IEnumerator StopMoving(float pauseTime)
    {
        if (_pauseTime == 0) _pauseTime = pauseTime;
        while (_pausedTimeElapsed < pauseTime)
        {
            _pausedTimeElapsed += Time.fixedDeltaTime;
            SetFlashHighlight(_pausedTimeElapsed, IsPaused);        // turn on the flash highlight
            yield return CoroutineWaitTimes.WaitForFixedUpdate;
        }
        _pausedTimeElapsed = 0;
        IsPaused = false;
        IsPausable = true;
        SetFlashHighlight(_pausedTimeElapsed, IsPaused);           // turn off the flash highlight
        OnResume.Invoke();
        OnResumeUpdateUI.Invoke(gameObject);
    }
    public void Unpause()
    {
        if (_stopMovingCo != null) StopCoroutine(_stopMovingCo);
        _pausedTimeElapsed = 0;
        IsPausable = true;
        IsPaused = false;
        SetFlashHighlight(_pausedTimeElapsed, IsPaused);           // turn off the flash highlight
    }

    public void UnpauseEvent()
    {
        Unpause();
        OnResume.Invoke();
    }

    private void SetHighlightMaterial(bool isHighlighted)
    {
        if (_highlightabledRenderers == null || _highlightMaterial == null || _defaultMaterial == null)
            return;

        foreach (var rend in _highlightabledRenderers)
        {
            if (rend != null)
            {
                float fresnelStrength = isHighlighted ? 1f : 0f;
                rend.material.SetFloat("_FresnelStrength", fresnelStrength);
            }
        }
    }
    public void OnTargeting(bool isHighlining)
    {
        if (IsPaused) return;
        SetHighlightMaterial(isHighlining);
    }

    private void SetFlashHighlight(float pulseSpeed, bool isPulsating)
    {
        foreach (var rend in _highlightabledRenderers)
        {
            if (rend != null)
            {
                int pulsating = isPulsating ? 1 : 0;

                rend.material.SetInt("_IsPulsating", pulsating);            // turn on or off flashing highlight
                rend.material.SetFloat("_FresnelStrength", pulsating);

                // change flashing speed
                if (pulseSpeed < _speedUpStandard) rend.material.SetFloat("_PulseSpeed", _defaultFlashing);
                if (pulseSpeed > _speedUpStandard) rend.material.SetFloat("_PulseSpeed", _speedUpFlashing);
            }
        }
    }

    public void PausingAudio(int dialogueCount)
    {
        return;
    }
}
