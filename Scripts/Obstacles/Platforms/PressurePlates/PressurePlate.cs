using System.Collections;
using UnityEngine;

public class PressurePlate : Pushable
{
    [SerializeField] private GameObject _triggerVolume;
    [SerializeField] private Renderer _emmisionRenderer;
    [SerializeField] private GameObject _lights;
    [ColorUsage(true, true)]
    [SerializeField] private Color _lockedColor = Color.yellow;
    private Vector3 _endingPos;
    private Vector3 _destination;
    private Color _emmisionColor;
    private bool _initalLocked;
    private bool _locked;

    protected override void Awake()
    {
        base.Awake();
        _endingPos = _startingPos + (Vector3.up * _displacement);
        _pointReachDistance = 0.01f;
        _emmisionRenderer.material.EnableKeyword("_EMMISION");
        _emmisionColor = _emmisionRenderer.material.GetColor("_EmissionColor");
        _emmisionRenderer.material.SetColor("_EmissionColor", Color.black);
    }

    protected override void MoveObject(bool isTriggered)
    {
        IsActivated = isTriggered;
        //update displacement only if the triggered is updated
        if (_wasTriggered == isTriggered) return;
        _wasTriggered = isTriggered;

        if (IsActivated)
        {
            _emmisionRenderer.material.SetColor("_EmissionColor", _emmisionColor);
            _lights.SetActive(true);
        }
        else
        {
            _emmisionRenderer.material.SetColor("_EmissionColor", Color.black);
            _lights.SetActive(false);
        }

        OnActivationUpdate.Invoke();

        _destination = isTriggered ? _endingPos : _startingPos;
        StartLerp();
    }

    protected override IEnumerator LerpObject()
    {
        while (Vector3.Distance(transform.position, _destination) > _pointReachDistance)
        {
            Vector3 direction = (_destination - transform.position).normalized;
            _rigidbody.velocity = direction * _speed;
            yield return CoroutineWaitTimes.WaitForFixedUpdate;
        }

        _rigidbody.velocity = Vector3.zero;
    }

    public override void LockActivateable()
    {
        _rigidbody.isKinematic = true;
        transform.position = _endingPos;
        _emmisionRenderer.material.SetColor("_EmissionColor", _lockedColor);
        _triggerVolume.SetActive(false);
        _lights.SetActive(false);
        this.enabled = false;
    }

    public override void UnlockActivateable()
    {
        _triggerVolume.SetActive(true);
        _rigidbody.isKinematic = false;
        _emmisionRenderer.material.SetColor("_EmissionColor", Color.black);
    }

    public void SaveState()
    {

    }

    public void ReloadState()
    {

    }
}
