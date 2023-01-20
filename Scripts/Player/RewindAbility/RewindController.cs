using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class RewindController : MonoBehaviour
{
    [SerializeField] private float _rewindDuration = 5f;
    [SerializeField] private UnityEvent<float> _startRecord = new UnityEvent<float>();
    [SerializeField] private UnityEvent _endRecord = new UnityEvent();
    [SerializeField] private Ghost _ghostPrefab;

    private Rewind _playerRewind;
    private bool _canRecord = true;

    private void Start()
    {
        _playerRewind = GetComponent<Rewind>();
    }

    public void SetCanRecord(bool enabled)
    {
        _canRecord = enabled;
    }

    public void ToggleRecord()
    {
        if (_playerRewind.IsRewinding) return;

        Debug.Log($"Tried Toggling Record: {_canRecord}");

        if (_playerRewind.IsRecording)
        {
            _endRecord.Invoke();
        }
        else if (_canRecord)
        {
            SetCanRecord(false);
            _startRecord.Invoke(_rewindDuration);
        }
    }

    public void SpawnGhost(RecordedPlayerInputs playerInputs)
    {
        Ghost ghost = PoolSystem.Instance.Get(_ghostPrefab, transform.position, transform.rotation) as Ghost;
        ghost.BeginGhostMovement(playerInputs);
        ghost.OnFinishedReplay.AddListener(SetCanRecord);
    }

}
