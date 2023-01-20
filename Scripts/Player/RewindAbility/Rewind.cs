using System.Collections;
using System;
using UnityEngine;
using UnityEngine.Events;

public class Rewind : MonoBehaviour
{
    public float TimeElapsed { get; protected set; } = 0f;
    public bool IsRewinding { get; protected set; }
    public bool IsRecording { get; protected set; }

    public UnityEvent BeginRewind = new UnityEvent();
    public UnityEvent FinishRewind = new UnityEvent();
    public UnityEvent BeginRecord = new UnityEvent();
    public UnityEvent FinishRecord  = new UnityEvent();


    public void OverrideTimeElapsed(float newTime)
    {
        TimeElapsed = newTime;
    }

    public virtual void ClearAll() { }

    public virtual void StartRecord()
    {
        if (IsRecording) return;
        IsRecording = true;
        BeginRecord.Invoke();
    }

    public virtual void StopRecord(bool rewind = true)
    {
        if (!IsRecording) return;
        IsRecording = false;
        FinishRecord.Invoke();
        if(rewind) StartRewind();
        else ClearAll();
    }

    public virtual void StartRewind()
    {
        if (IsRewinding) return; 
        IsRewinding = true;
        BeginRewind.Invoke();
    }

    public virtual void StopRewind()
    {
        if (!IsRewinding) return;
        IsRewinding = false;
        FinishRewind.Invoke();
    }

    public void SetRecord(bool isRecording)
    {
        if (isRecording) StartRecord();
        else 
        {
            StopRecord();
        }
    }

    public void SetRewind(bool isRewinding)
    {
        if(isRewinding) StartRewind();
        else StopRewind();
    }
}
