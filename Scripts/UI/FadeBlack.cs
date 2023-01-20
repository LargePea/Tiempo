using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Events;
using UnityEngine.SceneManagement;

public class FadeBlack : MonoBehaviour
{
    [SerializeField] private Image _blackScreen;
    [SerializeField] private float _fadeTime = 1f;
    [SerializeField] private UnityEvent _onFadedToBlack = new UnityEvent();
    [SerializeField] private GameObject _defaultMenu;
    [SerializeField] private GameObject _asyncLoadScreen;

    private bool _loadScene;
    private string _newSceneName;
    private bool _toggleMenu;

    private GameObject _targetMenu;
    private bool _loadAsync;

    public void FadeToBlack(bool fadeToBlack)
    {
        StartCoroutine(Fade(fadeToBlack));
    }

    private IEnumerator Fade(bool fadeToBlack)
    {
        float timeElapsed = 0f;
        while (timeElapsed <= _fadeTime)
        {
            float alpha = Mathf.Clamp01(fadeToBlack ? (timeElapsed / _fadeTime) : 1 - (timeElapsed / _fadeTime));
            _blackScreen.color = new Color(0, 0, 0, alpha);
            timeElapsed += Time.deltaTime;
            yield return null;
        }

        if (fadeToBlack)
        {
            if(_toggleMenu) _onFadedToBlack.Invoke();
        }
        if (_loadScene)
        {
            if(_loadAsync) _asyncLoadScreen.SetActive(true);
            else SceneManager.LoadScene(_newSceneName);
        }
    }

    public void SetAsync()
    {
        _loadAsync = true;
    }

    public void SetTargetMenu(GameObject menu)
    {
        _targetMenu = menu;
        _toggleMenu = true;
    }

    public void SetTargetScene(string scene)
    {
        _loadScene = true;
        _newSceneName = scene;
        _toggleMenu = false;
    }

    public void ToggleTargetMenuActive()
    {
        if(_targetMenu != null) _targetMenu.SetActive(!_targetMenu.activeSelf);
    }

    public void SetDefaultMenuActive(bool active)
    {
        _defaultMenu.SetActive(active);
    }
}
