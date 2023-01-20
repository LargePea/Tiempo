using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CharacterAnimations : AnimationRecorder
{
    // damping time smooths rapidly changing values sent to animator
    [SerializeField] private float _dampTime = 0.1f;

    private CharacterMovement _characterMovement;

    private void Start()
    {
        _animator = GetComponent<Animator>();
        _characterMovement = GetComponent<CharacterMovement>();
    }

    private void Update()
    {
        if (_animator.recorderMode == AnimatorRecorderMode.Playback) return;

        // to reset the value of animation
        if(_characterMovement.IsIdle)
        {
            _animator.SetFloat("Speed", 0, _dampTime, Time.deltaTime);
            _animator.SetFloat("VerticalVelocity", 0, _dampTime, Time.deltaTime); //damp time acts as a total lerp time to smooth animations
        }
        else
        {
            // send velocity to animator, ignoring y-velocity
            Vector3 velocity = _characterMovement.Velocity;
            Vector3 flattenedVelocity = new Vector3(velocity.x, 0f, velocity.z);
            float speed = Mathf.Min(_characterMovement.MoveInput.magnitude, flattenedVelocity.magnitude / _characterMovement.Speed);
            _animator.SetFloat("Speed", speed, _dampTime, Time.deltaTime);
            // send grounded state
            // send isolated y-velocity
            _animator.SetFloat("VerticalVelocity", velocity.y);

        }

        _animator.SetBool("IsGrounded", _characterMovement.IsGrounded);
    }
}