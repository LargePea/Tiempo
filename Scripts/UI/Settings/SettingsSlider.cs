using System;
using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem;
using UnityEngine.VFX;
using UnityEngine.Events;

public class SettingsSlider : Selectable
{
    [Header("Settings")]
    [SerializeField] private FloatSetting _setting;
    [SerializeField] private float _minValue;
    [SerializeField] private float _maxValue;
    [SerializeField] private float _sensitivity = 1f;

    [Header("Menu Control")]
    [SerializeField] private VisualEffect _bar;
    [SerializeField] private InputActionReference _backListener;
    [SerializeField] private GameObject _back;
    [SerializeField] private GameObject _basicMenuTextController;
    [SerializeField] private GameObject _selectedMenuTextController;   
    [SerializeField] private GameObject _basicMenuTextKeyboard;
    [SerializeField] private GameObject _selectedMenuTextKeyboard;

    [Header("Valve")]
    [SerializeField] private Transform _valve;
    [SerializeField] private float _spinSpeed = 1f;
    [SerializeField] private InputActionReference _movement;
    [SerializeField] private VisualEffect _effectAsset;

    [Header("Sounds")]
    [SerializeField] private UnityEvent IncreaseLiquid;
    [SerializeField] private UnityEvent DecreaseLiquid;
    [SerializeField] private UnityEvent PlayRotateSound;
    [SerializeField] private UnityEvent StopRotateSound;
    [SerializeField] private FMODEventReference _deselectSound;

    private bool _isSelected;
    private bool _playSound;

    private GameObject _defaultText;
    private GameObject _selectedText;

    protected override void Awake()
    {
        base.Awake();
        _valve.localRotation = Quaternion.Euler(0, _bar.GetFloat("Fill") * 360, 0);
    }

    private void Update()
    {
        if (_isSelected && _bar.GetFloat("Fill") != 1 && _bar.GetFloat("Fill") != 0) 
        {
            float input = _movement.action.ReadValue<Vector2>().y;
            if (input > 0)
            {
                _valve.Rotate(Vector3.up, -_spinSpeed * Time.unscaledDeltaTime);
                if (_playSound)
                {
                    _playSound = false;
                    PlayRotateSound.Invoke();
                }
            } 

            else if (input < 0)
            {
                _valve.Rotate(Vector3.up, _spinSpeed * Time.unscaledDeltaTime);
                if (_playSound)
                {
                    _playSound = false;
                    PlayRotateSound.Invoke();
                }
            }  
            else if(!_playSound)
            {
                _playSound = true;
                StopRotateSound.Invoke();
            }
        }
    }

    protected override void OnEnable()
    {
        UpdateUI();
        InputSystem.onDeviceChange += UpdateUI;
        base.OnEnable();
        if(_setting != null) _bar.SetFloat("Fill" , (1 / (_maxValue - _minValue) * (_setting.GetValue() - _minValue)));
    }

    protected override void OnDisable()
    {
        InputSystem.onDeviceChange -= UpdateUI;
        base.OnDisable();
        _defaultText.SetActive(false);
        _selectedText.SetActive(false);
    }

    private void UpdateUI(InputDevice device = null, InputDeviceChange change = InputDeviceChange.Disconnected)
    {
        _basicMenuTextController.SetActive(false);
        _selectedMenuTextController.SetActive(false);
        _basicMenuTextKeyboard.SetActive(false);
        _selectedMenuTextKeyboard.SetActive(false);


        _defaultText = Gamepad.all.Count == 0 ? _basicMenuTextKeyboard : _basicMenuTextController;
        _selectedText = Gamepad.all.Count == 0 ? _selectedMenuTextKeyboard : _selectedMenuTextController;

        _defaultText.SetActive(true);
        _selectedText.SetActive(false);
    }

    //toggle button's ability to navigate
    public void SelectButton()
    {
        if (_isSelected) return;

        _backListener.action.performed += DeselectButton;
        //disable back button
        _back.SetActive(false);

        //switchText
        _defaultText.SetActive(false);
        _selectedText.SetActive(true);  
        _isSelected = true;
        EventSystem.current.SetSelectedGameObject(gameObject);
        _effectAsset.Play();
    }

    public void DeselectButton(InputAction.CallbackContext callbackContext)
    {
        if (!_isSelected) return;

        _backListener.action.performed -= DeselectButton;
        EventSystem.current.SetSelectedGameObject(_valve.gameObject);
        _back.SetActive(true);
        _defaultText.SetActive(true);
        _selectedText.SetActive(false);
        _isSelected = false;
        _effectAsset.Play();
        _deselectSound.PlayOneShot();
    }

    public override void OnMove(AxisEventData eventData)
    {
        if (!_isSelected) 
        {
            base.OnMove(eventData);
            return;
        }

        float percentage;

        switch (eventData.moveDir)
        {
            //increase bar
            case MoveDirection.Up:
                IncreaseLiquid.Invoke();
                percentage = Mathf.Clamp01(_bar.GetFloat("Fill") + (_sensitivity / 100f));
                break;

            //Decrease bar
            case MoveDirection.Down:
                DecreaseLiquid.Invoke();
                percentage = Mathf.Clamp01(_bar.GetFloat("Fill") - (_sensitivity / 100f));
                break;

            default:
                percentage = _bar.GetFloat("Fill");
                break;
        }

        _bar.SetFloat("Fill", percentage);
        
        UpdateSetting(percentage);
    }

    protected virtual void UpdateSetting(float percentage)
    {      
        if(_setting != null) _setting.SetValue(_minValue + ((_maxValue - _minValue) * percentage));
    }
}
