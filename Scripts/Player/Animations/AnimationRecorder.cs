using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AnimationRecorder : MonoBehaviour
{
    [SerializeField] protected Animator _animator;
    [SerializeField] private bool _recordSpeeds = false;
    private List<float> _animatorSpeeds = new List<float>((int)(10 / 0.02));
    private bool _recording = false;

    private IEnumerator _animCo;

    private void Awake()
    {
        _animator = GetComponent<Animator>();
    }

    private void FixedUpdate()
    {
        if(_recording) _animatorSpeeds.Insert(0, _animator.speed);
    }

    public void RecordAnimation()
    {
        _animator.StartRecording(0);
        if(_recordSpeeds) _recording = true;
    }

    public void PlayerDeath()
    {
        _animator.StopRecording();
        _recording = false;
        //reset the speeds when done
        _animatorSpeeds.Clear();
    }

    public void StopRecord()
    {
        _animator.StopRecording();
        _recording = false;
    }

    public void StopRecordStartPlayBack()
    {
        StopRecord();

        _animator.StartPlayback();
        _animCo = Playback();
        StartCoroutine(_animCo);
    }

    private IEnumerator Playback()
    {
        yield return null;

        //Check if anything is recorded if not return
        if (_animator.recorderStartTime == -1) yield break;

        _animator.playbackTime = _animator.recorderStopTime;
        while (_animator.playbackTime >= 0)
        {
            //set animator speeds
            if(!_recordSpeeds 
                || (_animatorSpeeds.Count > 0 && _animatorSpeeds[0] == 1)) 
                        _animator.playbackTime = Mathf.Clamp(_animator.playbackTime - Time.fixedDeltaTime, 0, _animator.recorderStopTime);
            if (_recordSpeeds && _animatorSpeeds.Count > 0) _animatorSpeeds.RemoveAt(0);
            yield return CoroutineWaitTimes.WaitForFixedUpdate;
        }
    }

    public void StopPlayback()
    {
        if(_animCo != null) StopCoroutine(_animCo);
        _animator.StopPlayback();

        //reset the speeds when done
        _animatorSpeeds.Clear();
    }
}
