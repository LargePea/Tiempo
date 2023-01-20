using System.Collections;
using System.Collections.Generic;
using System;
using UnityEngine;
using UnityEngine.Events;

public class Ghost : PooledObject
{
    [SerializeField] GameObject _camera;
    [SerializeField] private PausingController _pauseController;
    private RecordedPlayerInputs _recordedInputs;

    private Animator _animator;
    private CharacterMovement3D _characterMovement3D;
    private Rigidbody _rigidbody;
    private Targetable _myTargetable;

    private Vector3 _lastPosition = Vector3.zero;
    private bool _playback = false;
    private float _timeElapsed = 0f;

    public UnityEvent<bool> OnFinishedReplay = new UnityEvent<bool>();
    public UnityEvent OnKilled = new UnityEvent();

    protected override void Awake()
    {
        _animator = GetComponent<Animator>();
        _characterMovement3D = GetComponent<CharacterMovement3D>();
        _rigidbody = GetComponent<Rigidbody>();
        _myTargetable = GetComponent<Targetable>();
        _myTargetable.IsTargetable = false;
    }

    public void BeginGhostMovement(RecordedPlayerInputs recordedInputs)
    {
        _recordedInputs = recordedInputs;
        _playback = true;
        _rigidbody.isKinematic = false;
        _myTargetable.IsTargetable = true;
    }

    private void FixedUpdate()
    {
        if (_playback)
        {
            _timeElapsed += Time.fixedDeltaTime;

            if (_recordedInputs.Positions.Count > 0)
            {
                //set animator walk
                if (Vector2.Distance(new Vector2(_lastPosition.x, _lastPosition.z), new Vector2(_recordedInputs.Positions[0].x, _recordedInputs.Positions[0].z)) > 0.01) _animator.SetFloat("Speed", 2, 0.1f, Time.fixedDeltaTime);
                else _animator.SetFloat("Speed", 0, 0.1f, Time.fixedDeltaTime);

                //set transform x and z
                _lastPosition = _recordedInputs.Positions[0];
                transform.position = new Vector3(_recordedInputs.Positions[0].x, transform.position.y, _recordedInputs.Positions[0].z);
                _recordedInputs.Positions.RemoveAt(0);
            }

            if(_recordedInputs.Rotations.Count > 0)
            {
                //set rotation
                if(Vector3.Distance(transform.rotation.eulerAngles, _recordedInputs.Rotations[0]) > 0.01) transform.Rotate(_recordedInputs.Rotations[0] - transform.rotation.eulerAngles);
                //transform.rotation = Quaternion.Euler(_recordedInputs.Rotations[0]);
                _recordedInputs.Rotations.RemoveAt(0);
            }

            if(_recordedInputs.CameraPositions.Count > 0)
            {
                _camera.transform.position = _recordedInputs.CameraPositions[0];
                _recordedInputs.CameraPositions.RemoveAt(0);
            }

            if(_recordedInputs.CameraRotations.Count > 0)
            {
                _camera.transform.rotation = Quaternion.Euler(_recordedInputs.CameraRotations[0]);
                _recordedInputs.CameraRotations.RemoveAt(0);
            }

            if (_recordedInputs.JumpInputs.Contains(_timeElapsed))
            {
                _characterMovement3D.Jump();
            }
            if (_recordedInputs.JumpCancels.Contains(_timeElapsed))
            {
                _characterMovement3D.LongJump();
            } 
            if (_recordedInputs.PauseInputs.Contains(_timeElapsed))
            {
                _pauseController.Pause();
                
            } 
            if (_recordedInputs.UnpauseInputs.Contains(_timeElapsed)) _pauseController.Pause();

            // check if all values are used up if they are end the ghost
            if(!(_recordedInputs.Positions.Count > 0))
            {
                KillGhost();
            }
        }
    }

    public void KillGhost()
    {
        _rigidbody.isKinematic = true;
        _recordedInputs.Clear();
        OnFinishedReplay.Invoke(true);
        _pauseController.ForceDequeueAll();
        _timeElapsed = 0;
        _playback = false;
        _myTargetable.IsTargetable = false;
        if(!IsPooled) Pool.Release(this);
        OnFinishedReplay.RemoveAllListeners();
    }

    public void KillZGhost()
    {
        OnKilled.Invoke();
        OnKilled.RemoveAllListeners();
    }
}
