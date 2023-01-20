using System.Collections;
using UnityEngine;
using UnityEngine.Events;

public class Weight : PlatformController
{
    [SerializeField] private float _speed;
    [SerializeField] private float _displacement;
    [SerializeField] private float _pointReachedDistance = 0.05f;
    [SerializeField] private float _waitTime = 1;

    [Header("Audio")]
    [SerializeField] private GameObject _movingUp;
    public UnityEvent m_StartAudio;
    public UnityEvent m_StopAudio;



    private bool _isGoingToTargetPosition;
    private float _waitTimeElapsed;
    private Vector3 _targetPostion;
    private Vector3 _startingPosition;
    private Rigidbody _rigidbody;
    private bool _setUp = false;

    private bool _initalIsGoingToTargetPosition;
    private float _initalWaitTimeElapsed;
 
    protected override void Start()
    {
        if (!_setUp)
        {
            base.Start();
            //set up starting values
            _startingPosition = transform.position;
            _targetPostion = new Vector3(transform.position.x, transform.position.y + _displacement, transform.position.z);
            _isGoingToTargetPosition = true;
            _rigidbody = GetComponent<Rigidbody>();

            _setUp = true;
        }
        //begin weight movement
        StartCoroutine(Move());
    }

    private void OnEnable()
    {
        if (_rigidbody == null) return;
        //begin weight movement
        StartCoroutine(Move());
    }

    private IEnumerator Move()
    {
        Vector3 destination;
        m_StartAudio.Invoke();

        while (true)
        {
            _rigidbody.isKinematic = IsPaused || IsLocked;
            destination = _isGoingToTargetPosition ? _targetPostion : _startingPosition;
            Vector3 direction = (destination - transform.position).normalized;
            _rigidbody.velocity = direction * _speed;

            //wait after reaching destination and change location
            if (Vector3.Distance(transform.position, destination) < _pointReachedDistance)
            {
                // SFX
                m_StopAudio.Invoke();
    
                ToggleDestination();
                while (_waitTimeElapsed < _waitTime)
                {
                    _rigidbody.velocity = Vector3.zero;
                    _waitTimeElapsed += Time.fixedDeltaTime;

                    yield return CoroutineWaitTimes.WaitForFixedUpdate;
                }
                _waitTimeElapsed = 0;
                m_StartAudio.Invoke();
            }



            yield return CoroutineWaitTimes.WaitForFixedUpdate;
        }

    }

    public void ToggleDestination()
    {
        _isGoingToTargetPosition = !_isGoingToTargetPosition;
    }

    public override void SaveState()
    {
        base.SaveState();
        _initalWaitTimeElapsed = _waitTimeElapsed;
        _initalIsGoingToTargetPosition = _isGoingToTargetPosition;
    }

    public override void ReloadState()
    {
        base.ReloadState();
        _waitTimeElapsed = _initalWaitTimeElapsed;
        _isGoingToTargetPosition = _initalIsGoingToTargetPosition;
    }
}
