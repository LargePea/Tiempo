using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class Torogate : MonoBehaviour, ITorobotInteract
{
    [SerializeField] private Animator _animator;
    [SerializeField] private UnityEvent _onDoorOpened = new UnityEvent();

    private bool _initalIsOpened;

    public void Collide(Vector3 direction, float force)
    {
        _animator.SetBool("isOpened", true);
        _onDoorOpened.Invoke();
    }

    public void SaveState()
    {
        _initalIsOpened = _animator.GetBool("isOpened");
    }

    public void ReloadState()
    {
        _animator.SetBool("isOpened", _initalIsOpened);
    }
}
