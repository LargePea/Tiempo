using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

public class SkipCutscene : MonoBehaviour
{
    [SerializeField] private float _holdDuration = 2f;
    [SerializeField] private Image _image;
    [SerializeField] private GameObject _prompt;
    [SerializeField] private UnityEvent _onCompletedSkip = new UnityEvent();
    private float _heldDuration = 0;

    private bool _isHeld = false;

    public void OnStartSkip()
    {
        _isHeld = true;
    }

    public void OnStopSkip()
    {
        _isHeld = false;
        _heldDuration = 0;
        _image.fillAmount = 0;
    }

    private void Update()
    {
        if (_isHeld)
        {
            _heldDuration += Time.unscaledDeltaTime;
            _image.fillAmount = _heldDuration / _holdDuration;
            if(_image.fillAmount >= 1)
            {
                OnStopSkip();
                _onCompletedSkip.Invoke();
            }
        } 
    }
}
