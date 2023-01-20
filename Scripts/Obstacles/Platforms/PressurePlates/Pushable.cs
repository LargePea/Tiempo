using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class Pushable : MonoBehaviour, IActivatable, IRewindable
{
    [SerializeField] protected float _displacement;
    [SerializeField] protected float _speed;

    private List<Collider> _charactersOnTop = new List<Collider>();
    protected bool _wasTriggered = false;
    protected float _pointReachDistance = 0.05f;

    //Movement variables
    private IEnumerator _movingCo;
    protected Vector3 _startingPos;
    protected Rigidbody _rigidbody;
    protected bool _initalized = false;

    //activation variables
    public bool IsActivated { get; set; }
    public UnityEvent OnActivationUpdate { get; } = new UnityEvent();

    //rewinding variables
    public bool IsRewinding { get; set; }
    public bool IsRecording { get; set; }

    protected virtual void Awake()
    {
        if (!_initalized)
        {
            _rigidbody = GetComponent<Rigidbody>();
            _startingPos = transform.position;
        }
    }

    public void AddCharacterOnTop(Collider character)
    {
        _charactersOnTop.Add(character);
    }

    public void RemoveCharacterOnTop(Collider character)
    {
        _charactersOnTop.Remove(character);
    }
    public void ClearCharacters() // referenced in editor to clear when rewinding
    {
        _charactersOnTop.Clear();
        IsActivated = false;
    }

    public bool CheckCharactersOnTop()
    {
        //dont check colliders if there are none on top
        if (_charactersOnTop.Count == 0) return false;

        List<Collider> toBeRemoved = new List<Collider>();
        //secondary check for deactivated or null colliders
        foreach (Collider character in _charactersOnTop)
        {
            if (!character || !character.enabled || !character.gameObject.activeInHierarchy) toBeRemoved.Add(character);
        }

        foreach(Collider character in toBeRemoved)
        {
            _charactersOnTop.Remove(character);
        }

        //if there are no characters on top after secondary check of the weight, untrigger it
        if (_charactersOnTop.Count == 0) return false;
        //else trigger it
        else return true;
    }

    protected virtual void Update()
    {
        bool isTriggered = CheckCharactersOnTop();
        MoveObject(isTriggered);
    }

    protected virtual void MoveObject(bool isTriggered) { }

    protected void StartLerp()
    {
        if (_movingCo != null) StopCoroutine(_movingCo);
        _movingCo = LerpObject();
        StartCoroutine(_movingCo);
    }

    protected virtual IEnumerator LerpObject() { yield return null; }

    public virtual void PlayerDeath()
    {
        StopAllCoroutines();
        ClearCharacters();
        transform.position = _startingPos;
        IsActivated = false;
        _rigidbody.velocity = Vector3.zero;
        _rigidbody.angularVelocity = Vector3.zero;
        _wasTriggered = false;
    }

    public virtual void LockActivateable() { }

    public virtual void UnlockActivateable() { }
}
