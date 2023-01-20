using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.Events;

public class EnemyController : PooledObject
{
    [Header("Enemy")]
    [SerializeField] protected float _moveSpeed = 1f;           // AI speed
    [SerializeField] protected float _chaseSpeed = 2f;
    [SerializeField] protected float _visionRadius = 4f;        // AI chase range
    [SerializeField] protected float _attackDistance = 3f;      // AI attack range
    [SerializeField] protected LayerMask _targetMask;           // target layout
    [SerializeField] protected LayerMask _occlusionMask;        // avoid layout
    protected Targetable _myTargetble;                          // AI self
    protected Targetable _target;
    public Targetable Target => _target;// objects
    public bool IsAI = false;

    [Header("Patrol Point")]
    [SerializeField] protected float _patrolPointReachedDistance = 1f;
    [SerializeField] protected float _patrolSpeed = 0.5f;
    [SerializeField] protected float _viewHalfAngle = 70f;
    [SerializeField] protected PatrolPoint[] _patrolPoints;

    [Header("Death")]
    [SerializeField] protected PooledParticles _deathParticle;

    // protected properties
    protected Vector3 Position => transform.position;
    protected bool _isTarget => _target != null && _target.IsTargetable;
    protected float TargetDistance => Vector3.Distance(_target.AimPosition, _myTargetble.AimPosition);
    protected bool IsTargetVisible => VisiblityHelper.TestVisibility(_myTargetble.AimPosition, _target.AimPosition, _visionRadius, transform.forward, _viewHalfAngle, _occlusionMask);

    public PatrolPoint[] PatrolPoint { get { return _patrolPoints; } set { _patrolPoints = value; } }

    protected CharacterMovement3D _characterMovement3D;
    
    // public event
    public UnityEvent<EnemyController> OnDeath;

    // methods
    public virtual void Init() { }

}
