using Sirenix.OdinInspector;
using System.Collections;
using UnityEngine;
using UnityEngine.VFX;

public class FlippingPlatform : PlatformController
{
    [SerializeField] private Animator _animator;
    [SerializeField] private VisualEffect[] _steams;

    [FoldoutGroup("SFX")] [SerializeField] private FMODEventReference _start;
    [FoldoutGroup("SFX")] [SerializeField] private FMODEventReference _turning;
    [FoldoutGroup("SFX")] [SerializeField] private FMODEventReference _stop;
    [FoldoutGroup("SFX")] public bool IsPlayerEnter { set; get; }

    // private
    private float _waitElapsedTime = 0;                 // the duration of elapsed time

    // to check if the stay elapsed time reaches the platform duration time or not
    private bool goingDown => _waitElapsedTime >= _platformUpWaitTime && _animator.GetBool("FlipUp");
    private bool goingUp => _waitElapsedTime >= _platformDownWaitTime && !_animator.GetBool("FlipUp");

    private float _initalWaitTime;
    private bool _initalFlipUp;
    private float _initalAnimatorSpeed;

    private void Awake()
    {
        //set animation speeds
        _animator.SetFloat("FlipUpMultiplier", _platformUpSpeedMultiplier);
        _animator.SetFloat("FlipDownMultiplier", _platformDownSpeedMultiplier);
    }

    private void FixedUpdate()
    {
        if (IsPaused) return;
        //if is waiting increase wait time
        if (_animator.GetCurrentAnimatorStateInfo(0).normalizedTime > 1) _waitElapsedTime += Time.fixedDeltaTime;

        bool playSteam = _animator.GetBool("FlipUp") ? _waitElapsedTime > _platformUpWaitTime - 0.5 : _waitElapsedTime > _platformDownWaitTime - 0.25;
        if (playSteam) foreach (var steam in _steams) steam.Play();

        //toggle platform movement when it is done waiting
        if (goingDown || goingUp)
        {
            _waitElapsedTime = 0;
            _animator.SetBool("FlipUp", !_animator.GetBool("FlipUp"));
        }
    }

    public void PauseAnimator()
    {
        _animator.speed = 0;
    }

    public void UnpauseAnimator()
    {
        _animator.speed = 1;
    }

    public override void SaveState()
    {
        base.SaveState();
        _initalWaitTime = _waitElapsedTime;
        _initalFlipUp = _animator.GetBool("FlipUp");
        _initalAnimatorSpeed = _animator.speed;
    }

    public override void ReloadState()
    {
        base.ReloadState();
        _waitElapsedTime = _initalWaitTime;
        _animator.SetBool("FlipUp", _initalFlipUp);
        _animator.speed = _initalAnimatorSpeed;
    }

    public override void PlayerDeath()
    {
        base.PlayerDeath();
        _waitElapsedTime = 0;
        StartCoroutine(ResetAnimator());
    }

    private IEnumerator ResetAnimator()
    {
        _animator.SetTrigger("Reset");
        _animator.SetBool("FlipUp", true);
        _animator.speed = 1;
        yield return null;
        _animator.ResetTrigger("Reset");
    }

    public void PlaySteam()
    {
        foreach (var steam in _steams) steam.Play();
    }

    public void StartSFX()
    {
        if (!IsPlayerEnter) return;
        _start.PlayOneShotAtLocation(transform.position);
    }

    public void TruningSFX()
    {
        if (!IsPlayerEnter) return;
        _turning.PlayOneShotAtLocation(transform.position);
    }

    public void StopSFX()
    {
        if (!IsPlayerEnter) return;
        _stop.PlayOneShotAtLocation(transform.position);
    }
}
