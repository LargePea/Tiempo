using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class ControllerCheck : MonoBehaviour
{
    private static bool CheckedController = false;
    [SerializeField] private GameObject _mainMenu;

    private Animator _animator;

    private void Awake()
    {
        Cursor.lockState = CursorLockMode.Locked;

        if (CheckedController)
        {
            Continue();
            return;
        }

        CheckedController = true;
    }

    public void Continue()
    {
        _mainMenu.SetActive(true);
        gameObject.SetActive(false);
    }
}
