using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class ControlsSpriteSwitch : MonoBehaviour
{
    [SerializeField] private GameObject _keyboardSprites;
    [SerializeField] private GameObject _controllerSprites;

    private void OnEnable()
    {
        UpdateUI();
        InputSystem.onDeviceChange += UpdateUI;
    }


    private void OnDisable()
    {
        InputSystem.onDeviceChange -= UpdateUI;
    }

    private void UpdateUI(InputDevice device = null, InputDeviceChange change = InputDeviceChange.Disconnected) 
    {
        _keyboardSprites.SetActive(false);
        _controllerSprites.SetActive(false);

        if(Gamepad.all.Count != 0) _controllerSprites.SetActive(true);
        else _keyboardSprites.SetActive(true);
    }
}
