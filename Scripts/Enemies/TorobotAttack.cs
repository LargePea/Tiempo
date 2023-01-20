using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.VFX;
using UnityEngine.Events;
using Sirenix.OdinInspector;

public class TorobotAttack : EnemyAbility
{
    [Header("Charge")]
    [SerializeField] private GameObject _chargeHitbox;
    [SerializeField] private float _knockBackForce = 5f;
    [SerializeField] private float _chargeSpeed = 30f;
    [SerializeField] private float _chargeCoolDown;
    [SerializeField] private float _reactionTime = 0.25f;

    [Header("VFX")]
    [SerializeField] private VisualEffect[] _noseSteam;
    [SerializeField] private float _startSteamDuration = 0.1f;
    [SerializeField] private float _endSteamDuration = 0.5f;
    [SerializeField] private Renderer _eyesRenderer;

    [Header("Sounds")]
    [FoldoutGroup("Attacking Noises")]
    [SerializeField] private UnityEvent _PrepareAttack = new UnityEvent();
    [FoldoutGroup("Attacking Noises")]
    [SerializeField] private UnityEvent _AttackPlayer = new UnityEvent();

    [FoldoutGroup("Collision")]
    [SerializeField] private UnityEvent _OnCollideWood = new UnityEvent();
    [FoldoutGroup("Collision")]
    [SerializeField] private UnityEvent _OnCollideMetal = new UnityEvent();

    [SerializeField] private UnityEvent CollisionShake = new UnityEvent();

    private Vector3 _chargingDest;

    private bool _isCooldown;
    private float _elapsedCooldown;
    private Vector3 _aimDirection;

    private bool _initalIsCooldown;
    private float _initalElapsedCooldown;
    private Vector3 _initalAimDirection;

    protected override IEnumerator CountDown(Targetable victim)
    {
        bool playSteam = true;
        _physicalMovement.ResetVelocity();
        _physicalMovement.OverrideHasInput(true);
        if(_physicalMovement.PlaySFX) _PrepareAttack.Invoke();

        StartCoroutine(PlaySteam(_startSteamDuration));

        //wait for countdown
        while (_countDownElapsed < _countDownTime)
        {
            
            _aimDirection = _currentTarget.AimPosition - _myTargetable.AimPosition;
            if (_countDownElapsed < _countDownTime - _reactionTime)
            {
                _physicalMovement.SetLookDirection(_aimDirection);
            }
            else if (playSteam)
            {
                playSteam = false;
                StartCoroutine(PlaySteam(_endSteamDuration));
                _eyesRenderer.material.SetInt("_IsEmissive", 1);
            }
            _countDownElapsed += Time.fixedDeltaTime;
            yield return CoroutineWaitTimes.WaitForFixedUpdate;
        }
        
        //attack the target after cooldown
        Damage(victim);
    }

    public override void Damage(Targetable victim)
    {
        StartCoroutine(Charging());
    }

    private IEnumerator Charging()
    {
        _animator.SetBool("Stunned", false);
        _animator.SetTrigger("ChargeUp");
        
        //if after rewind it is in cooldown state skip charging

        if (!_isCooldown)
        {
            if (_physicalMovement.PlaySFX) _AttackPlayer.Invoke();
            _physicalMovement.ChangeSpeed(_chargeSpeed);
            if (_chargeHitbox != null) _chargeHitbox.SetActive(true); // activate hitbox when charging;

            //flatten charging direciton and calculate destination
            _physicalMovement.SetLookDirection(transform.forward);
            _physicalMovement.SetMoveInput(transform.forward);
            _chargingDest = transform.forward * 1000;

            while (Vector3.Distance(transform.position, _chargingDest) > 1f)
            {
                Debug.DrawLine(transform.position, _chargingDest, Color.cyan);
                _physicalMovement.MoveTo(_chargingDest);
                yield return null;
            }
        }

        //after reached destination Stop and cooldown
        _animator.SetBool("Stunned", true);
        _isCooldown = true;
        if (_chargeHitbox != null) _chargeHitbox.SetActive(false);
        _physicalMovement.Stop();
        _physicalMovement.ResetVelocity();

        while (_elapsedCooldown < _chargeCoolDown)
        {
            _elapsedCooldown += Time.deltaTime;
            yield return null;
        }
        _isCooldown = false;
        _movement.IsAttacking = false;

        _animator.SetBool("Stunned", false);
        //reset timers
        _elapsedCooldown = 0;
        _countDownElapsed = 0;

        //chase player again after done cooldown
        _movement.ChangeState(EnemyStates.Chase);
    }

    public void Collision(Collider other)
    {
        if (other.CompareTag("TorobotIgnore")) return;

        CollisionShake.Invoke();
        _eyesRenderer.material.SetInt("_IsEmissive", 0);

        ITorobotInteract interactable = other.GetComponentInParent<ITorobotInteract>();

        if (interactable != null)
        {
            Vector3 dir = (other.transform.position - transform.position).normalized;
            Vector3 flattenDir = new Vector3(dir.x, 0, dir.z);
            if (other.TryGetComponent(out Health health) && other.TryGetComponent(out Targetable victim))
            {
                DamageInfo info = new DamageInfo
                {
                    Amount = _damage,
                    Victim = victim
                };

                health.Damage(info);
            }

            if(other.TryGetComponent(out MaterialChecker material))
            {
                switch (material.GroundMaterial)
                {
                    case FloorMaterial.Metal:
                        if (_physicalMovement.PlaySFX) _OnCollideMetal.Invoke();
                        break;
                    case FloorMaterial.Wood:
                        if (_physicalMovement.PlaySFX) _OnCollideWood.Invoke();
                        break;
                }
            }

            interactable.Collide(flattenDir, _knockBackForce);
        }

        //manually update _cestination when collided with something
        _elapsedCooldown = 0;
        _chargingDest = transform.position;
    }

    private IEnumerator PlaySteam(float steamDuration)
    {
        foreach (var steam in _noseSteam) steam.SetFloat("SpawnRate", 32f);
        yield return new WaitForSeconds(steamDuration);
        foreach (var steam in _noseSteam) steam.SetFloat("SpawnRate", 0f);
    }

    public override void SaveAbilityState()
    {
        base.SaveAbilityState();
        _initalIsCooldown = _isCooldown;
        _initalElapsedCooldown = _elapsedCooldown;
        _initalAimDirection = _aimDirection;
    }

    public override void ReloadAbilityState()
    {
        base.ReloadAbilityState();
        _isCooldown = _initalIsCooldown;
        _elapsedCooldown = _initalElapsedCooldown;
        _aimDirection = _initalAimDirection;
        _physicalMovement.ResetVelocity();
    }

    public override void ResetAbility()
    {
        base.ResetAbility();
        _elapsedCooldown = 0f;
        _isCooldown = false;
        _animator.SetBool("Stunned", false);
        _eyesRenderer.material.SetInt("_IsEmissive", 0);
        foreach (var steam in _noseSteam) steam.SetFloat("SpawnRate", 0f);
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.DrawLine(transform.position, _chargingDest);
    }
}
