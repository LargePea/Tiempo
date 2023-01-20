using UnityEngine;
using UnityEngine.Events;

public class TriggerVolume : MonoBehaviour
{
    [SerializeField] private string _interactingTag = "Player";
    [SerializeField] private bool _filterCollisions = true;
    [SerializeField] private bool _activateOnce = false;
    [SerializeField] private bool _ghostActivatable = true;
 
    public UnityEvent<Collider> OnEntered = new UnityEvent<Collider>();
    public UnityEvent<Collider> OnExited = new UnityEvent<Collider>();
    private bool _activated = false;
    private bool _initalActivated = false;

    private void OnTriggerEnter(Collider other)
    {
        if ((_activateOnce && _activated) || (!_ghostActivatable && other.TryGetComponent(out Ghost _))) return;

        if (_filterCollisions)
        {
            if (other.CompareTag(_interactingTag))
            {
                OnEntered.Invoke(other);
                _activated = true;
            }
        }
        else
        {
            OnEntered.Invoke(other);
            _activated = true;
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (_activateOnce && _activated) return;

        if (_filterCollisions)
        {
            if (other.CompareTag(_interactingTag))
            {
                OnExited.Invoke(other);
            }
        }
        else
        {
            OnExited.Invoke(other);
        }
    }

    public void SaveState()
    {
        _initalActivated = _activated;
    }

    public void ReloadState()
    {
        _activated = _initalActivated;
    }
}
