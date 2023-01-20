using System.Collections;
using System.Collections.Generic;
using System;
using UnityEngine;
using UnityEngine.AI;

public class ComplexRewind : Rewind
{
    [SerializeField] private bool _recordPosition = false;
    [SerializeField] private bool _recordRotation = false;

    private List<Vector3> _recordedPositions = null;
    private List<Vector3> _recordedRotations = null;

    private void Awake()
    {
        if(_recordPosition) _recordedPositions = new List<Vector3>((int)(10 / 0.02));
        if (_recordRotation) _recordedRotations = new List<Vector3>((int)(10 / 0.02));
    }

    private void FixedUpdate()
    {
        if (IsRecording)
        {
            if (_recordPosition) RecordValue(_recordedPositions, transform.position);
            if (_recordRotation) RecordValue(_recordedRotations, transform.rotation.eulerAngles);
        }
        else if (IsRewinding)
        {
            if (_recordPosition && _recordedPositions.Count > 0)
            {
                transform.position = _recordedPositions[0];
                _recordedPositions.RemoveAt(0);
            }
            if(_recordRotation && _recordedRotations.Count > 0)
            {
                transform.rotation = Quaternion.Euler(_recordedRotations[0]);
                _recordedRotations.RemoveAt(0);
            }
        }
    }

    private void RecordValue(List<Vector3> _recordedValues, Vector3 value)
    {
        if (_recordedValues.Count > (int)(10 / 0.02)) _recordedValues.RemoveAt(_recordedValues.Count - 1);
        _recordedValues.Insert(0, value);
    }

    public override void ClearAll()
    {
        _recordedPositions?.Clear();
        _recordedRotations?.Clear();
    }

    public void OnDeath() // listening when player dies
    {
        IsRecording = false;
        IsRewinding = false;
        ClearAll();
    }
}
