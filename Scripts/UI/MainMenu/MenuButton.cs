using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using UnityEngine.InputSystem;
using UnityEngine.Events;

public class MenuButton : MonoBehaviour, ISelectHandler, IDeselectHandler
{
    [SerializeField] private Animator _animator;
    [SerializeField] private InputActionReference _pushAction;
    [SerializeField] private UnityEvent _onClick = new UnityEvent();

    public void OnSelect(BaseEventData eventData)
    {
        _animator.SetBool("Selected", true);
        _pushAction.action.performed += ButtonClicked;
    }

    public void OnDeselect(BaseEventData eventData)
    {
        _animator.SetBool("Selected", false);
        _pushAction.action.performed -= ButtonClicked;
    }

    protected void OnDisable()
    {
        _pushAction.action.performed -= ButtonClicked;
    }

    public void ButtonClicked(InputAction.CallbackContext callbackContext)
    {
        Debug.Log("Clicked");
        _onClick.Invoke();
    }
}
