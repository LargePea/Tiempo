using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.SceneManagement;
using UnityEngine.InputSystem;
using UnityEngine.UI;
using TMPro;

public class AsyncLoadScene : MonoBehaviour
{
    [SerializeField] private Image _progressBar;
    [SerializeField] private float _flavourTextCycleTime;
    [SerializeField] private string[] _flavourTexts;
    [SerializeField] private TextMeshProUGUI _text;

    [SerializeField] private UnityEvent _startUp = new UnityEvent();
    [SerializeField] private string _sceneName;
    [SerializeField] private InputActionReference _confirm;

    private AsyncOperation sceneLoad;
    void Start()
    {
        _startUp.Invoke();
    }

    public void LoadScene()
    {
        StartCoroutine(LoadSceneAsync());
    }

    private IEnumerator LoadSceneAsync()
    {
        sceneLoad = SceneManager.LoadSceneAsync(_sceneName);

        //set inital flavour text
        float textCycle = 0;
        _text.text = _flavourTexts[Random.Range(0, _flavourTexts.Length)];

        while (!sceneLoad.isDone)
        {
            yield return null;

            textCycle += Time.deltaTime;
            if(textCycle > _flavourTextCycleTime)
            {
                int text = Random.Range(0, _flavourTexts.Length);
                //reroll text if its the same
                while(_text.text == _flavourTexts[text]) text = Random.Range(0, _flavourTexts.Length);
                _text.text = _flavourTexts[text];

                textCycle = 0;
            }

            _progressBar.fillAmount = Mathf.Clamp01(sceneLoad.progress / 0.9f);
        }
    }
}
