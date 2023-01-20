using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using TMPro;
using UnityEngine.EventSystems;

public class ControlsController : MonoBehaviour
{
    [Header("Paddles")]
    [SerializeField] private GameObject _rightTrigger;
    [SerializeField] private GameObject _leftTrigger;

    private float _maxTriggerRotation = -15f;

    [SerializeField] private GameObject _rightBumper;
    [SerializeField] private GameObject _leftBumper;

    private Vector3 _rightBumperPosition;
    private Vector3 _leftBumperPosition;

    [Header("JoySticks")]
    [SerializeField] private GameObject _rightJoystick;
    [SerializeField] private GameObject _leftJoystick;
    [SerializeField] private GameObject _dPad;

    private float _maxJoyStickRotation = 25f;
    private float _maxDPadRotation = 10f;

    private Vector3 _rightJoystickPosition;
    private Vector3 _leftJoystickPosition;

    [Header("Buttons")]
    [SerializeField] private GameObject _aButton;
    [SerializeField] private GameObject _bButton;
    [SerializeField] private GameObject _yButton;
    [SerializeField] private GameObject _xButton;
    [SerializeField] private GameObject _startButton;

    [Header("Controls Text")]
    [SerializeField] private TextMeshPro _jump;
    [SerializeField] private TextMeshPro _longJump;
    [SerializeField] private TextMeshPro _characterMovement;
    [SerializeField] private TextMeshPro _rewind;
    [SerializeField] private TextMeshPro _pause;
    [SerializeField] private TextMeshPro _cameraMovement;

    [Header("Controls Sprite")]
    [SerializeField] private ControlsSpritesContainer _controllerSprites;
    [SerializeField] private ControlsSpritesContainer _keyboardSprites;

    private PlayerInput _playerInput;

    private ControlsSpritesContainer _currentContainer;
    private void Awake()
    {
        _rightBumperPosition = _rightBumper.transform.localPosition;
        _leftBumperPosition = _leftBumper.transform.localPosition;
        _rightJoystickPosition = _rightJoystick.transform.localPosition;
        _leftJoystickPosition = _leftJoystick.transform.localPosition;

        _playerInput = GetComponent<PlayerInput>();

    }

    private void OnEnable()
    {
        _playerInput.ActivateInput();
        UpdateUI();
        InputSystem.onDeviceChange += UpdateUI;
    }

    private void OnDisable()
    {
        InputSystem.onDeviceChange -= UpdateUI;
        _playerInput.DeactivateInput();
        ResetTexts();
        _currentContainer.gameObject.SetActive(false);
    }

    private void UpdateUI(InputDevice device = null, InputDeviceChange change = InputDeviceChange.Disconnected)
    {
        ResetTexts();
        _currentContainer = Gamepad.all.Count == 0 ? _keyboardSprites : _controllerSprites;
        _playerInput.SwitchCurrentControlScheme(Gamepad.all.Count == 0 ? "Keyboard" : "Controller", Gamepad.all.Count != 0 ? Gamepad.current : Keyboard.current, Mouse.current);
        _currentContainer.gameObject.SetActive(true);
    }

    public void OnRightTrigger(InputValue value)
    {
        _rightTrigger.transform.localRotation = Quaternion.Euler(new Vector3(value.Get<float>() * _maxTriggerRotation, 0, 0));

        if (value.Get<float>() != 0) UpdateText(_pause, _currentContainer.PauseSprite, Color.white);
        else UpdateText(_pause, _currentContainer.PauseSprite, Color.black);
    }

    public void OnLeftTrigger(InputValue value)
    {
        _leftTrigger.transform.localRotation = Quaternion.Euler(new Vector3(value.Get<float>() * _maxTriggerRotation, 0, 0));
        if (value.Get<float>() != 0) UpdateText(_rewind, _currentContainer.RewindSprite, Color.white);
        else UpdateText(_rewind, _currentContainer.RewindSprite, Color.black);
    }

    public void OnRightBumper(InputValue value)
    {
        _rightBumper.transform.localPosition = value.Get<float>() == 0 ? _rightBumperPosition : _rightBumperPosition + new Vector3(0, -0.00751149f, 0);
    }

    public void OnLeftBumper(InputValue value)
    {
        _leftBumper.transform.localPosition = value.Get<float>() == 0 ? _leftBumperPosition : _leftBumperPosition + new Vector3(0, -0.00751149f, 0);
    }

    public void OnAButton(InputValue value)
    {
        ButtonPress(_aButton, value.Get<float>() == 0 ? new Vector3(0, 0, 0.0025346f) : new Vector3(0, 0, -0.0025346f));
        if (value.Get<float>() != 0)
        {
            UpdateText(_jump, _currentContainer.JumpSprite, Color.white);
            UpdateText(_longJump, _currentContainer.LongJumpSprite, Color.white);
        }

        else
        {
            UpdateText(_jump, _currentContainer.JumpSprite, Color.black);
            UpdateText(_longJump, _currentContainer.LongJumpSprite, Color.black);
        }
            
    }
    public void OnBButton(InputValue value)
    {
        ButtonPress(_bButton, value.Get<float>() == 0 ? new Vector3(0, 0, 0.0025346f) : new Vector3(0, 0, -0.0025346f));
    }
    public void OnXButton(InputValue value)
    {
        ButtonPress(_xButton, value.Get<float>() == 0 ? new Vector3(0, 0, 0.0025346f) : new Vector3(0, 0, -0.0025346f));
    }
    public void OnYButton(InputValue value)
    {
        ButtonPress(_yButton, value.Get<float>() == 0 ? new Vector3(0, 0, 0.0025346f) : new Vector3(0, 0, -0.0025346f));
    }

    public void OnRightJoystick(InputValue value)
    {
        Vector2 inputValue = value.Get<Vector2>().normalized;
        _rightJoystick.transform.localRotation = Quaternion.Euler(new Vector3(-inputValue.y * _maxJoyStickRotation, -inputValue.x * _maxJoyStickRotation, 0));
        if (inputValue.magnitude > 0.1) UpdateText(_cameraMovement, _currentContainer.CameraMovementSprite, Color.white);
        else UpdateText(_cameraMovement, _currentContainer.CameraMovementSprite, Color.black);
    }

    public void OnLeftJoystick(InputValue value)
    {
        Vector2 inputValue = value.Get<Vector2>().normalized;
        _leftJoystick.transform.localRotation = Quaternion.Euler(new Vector3(-inputValue.y * _maxJoyStickRotation, -inputValue.x * _maxJoyStickRotation, 0));

        if (inputValue.magnitude > 0.1) UpdateText(_characterMovement, _currentContainer.CharacterMovementSprite, Color.white);
        else UpdateText(_characterMovement, _currentContainer.CharacterMovementSprite, Color.black);
    }

    public void OnRightJoystickPress(InputValue value)
    {
        _rightJoystick.transform.localPosition = value.Get<float>() == 0 ? _rightJoystickPosition : _rightJoystickPosition + new Vector3(0, 0, -0.00116412f);
    }
    
    public void OnLeftJoystickPress(InputValue value)
    {
        _leftJoystick.transform.localPosition = value.Get<float>() == 0 ? _leftJoystickPosition : _leftJoystickPosition + new Vector3(0, 0, -0.00116412f);
    }

    public void OnDPad(InputValue value)
    {
        Vector2 inputValue = value.Get<Vector2>();
        _dPad.transform.localRotation = Quaternion.Euler(-inputValue.y * _maxDPadRotation, -inputValue.x * _maxDPadRotation, 0);
    }

    public void OnStartButton(InputValue value)
    {
        ButtonPress(_startButton, value.Get<float>() == 0 ? new Vector3(0, 0, 0.00146562f) : new Vector3(0, 0, -0.00146562f));
    }

    private void ButtonPress(GameObject button, Vector3 offset)
    {
        button.transform.localPosition += offset;
    }

    private void UpdateText(TextMeshPro text, SpriteRenderer image, Color color)
    {
        text.color = color;
        text.outlineColor = color;
        text.faceColor = color;
        image.color = color;
    }

    public void ResetTexts()
    {
        if(_currentContainer == null) return;

        UpdateText(_jump, _currentContainer.JumpSprite, Color.black);
        UpdateText(_longJump, _currentContainer.LongJumpSprite, Color.black);
        UpdateText(_cameraMovement, _currentContainer.CameraMovementSprite, Color.black);
        UpdateText(_characterMovement, _currentContainer.CharacterMovementSprite, Color.black);
        UpdateText(_pause, _currentContainer.PauseSprite, Color.black);
        UpdateText(_rewind, _currentContainer.RewindSprite, Color.black);

        _controllerSprites.gameObject.SetActive(false);
        _keyboardSprites.gameObject.SetActive(false);   
    }
}
