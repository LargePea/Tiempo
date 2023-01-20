using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem;
using UnityEngine.UI;

public class MenuPanelController : MonoBehaviour
{
    [SerializeField] private Button[] _buttons;
    [SerializeField] private UnityEvent _onHidMenu = new UnityEvent();
    [SerializeField] private Animator _animator;
    [SerializeField] private PlayerInput _playerInput;
    [SerializeField] private GameObject _resumeListener;

    private GameObject _targetMenu;
    private bool _isActive = false;

    private void OnEnable()
    {
        if(_playerInput != null) _playerInput.ActivateInput();
        StartCoroutine(PanelActivationFallback());
    }

    private void OnDisable()
    {
        if (_playerInput != null) _playerInput.DeactivateInput();
        StopAllCoroutines();
    }

    public void SetPanelActive()
    {
        _isActive = true;
        foreach(var button in _buttons)
        {
            button.enabled = true;
            button.interactable = true;
        }
        if (_resumeListener != null) _resumeListener.SetActive(true);
    }

    private IEnumerator PanelActivationFallback()
    {
        yield return new WaitForSeconds(2);
        if(!_isActive) 
            foreach (var button in _buttons)
            {
                button.enabled = true;
                button.interactable = true;
            }
    }

    public void SetPanelDeactive()
    {
        _isActive = false;
        foreach (var button in _buttons)
        {
            button.enabled = false;
            button.interactable = false;
        }
    }

    public void SetImageActive()
    {
        gameObject.SetActive(true);
    }

    public void SetImageDeactive()
    {
        gameObject.SetActive(false);
    }

    public void HidMenu()
    {
        _onHidMenu.Invoke();
    }

    public void HideMenu()
    {
        _animator.SetTrigger("Hide");
        SetPanelDeactive();
    }

    public void SetTargetMenu(GameObject menu)
    {
        _targetMenu = menu;
    }

    public void ActivateTargetMenu()
    {
        _targetMenu.SetActive(true);
    }
}
