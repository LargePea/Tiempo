using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class MainMenuStartUp : MonoBehaviour
{
    [SerializeField] private float _duration;
    [SerializeField] private bool _moveToLeft = true;

    private RectTransform _rectTransform;
    private Button button;

    Vector2 _startPos;
    Vector2 _endPos;

    private void Awake()
    {
        _rectTransform = GetComponent<RectTransform>();
        button = GetComponent<Button>();

        if(button) button.interactable = false;

        _startPos = _rectTransform.anchoredPosition;
        _endPos = new Vector2(_moveToLeft ? _startPos.x - _rectTransform.sizeDelta.x : _startPos.x + _rectTransform.sizeDelta.x, _startPos.y);

        BeginMove();
    }

    public void BeginMove()
    {
        StartCoroutine(MoveIn());
    }

    private IEnumerator MoveIn()
    {
        float elapsed = 0f;
        while(elapsed < _duration)
        {
            _rectTransform.anchoredPosition = Vector2.Lerp(_startPos, _endPos, elapsed / _duration);
            elapsed += Time.deltaTime;
            yield return null;
        }

        if (button) button.interactable = true;
    }
}
