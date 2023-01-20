using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.InputSystem;

public class Tutorials : MonoBehaviour
{
    [SerializeField] private RawImage _tutorialImage;
    [SerializeField] private Tutorial[] _tutorials;
    [SerializeField] private Animator _animator;
    [SerializeField] private float _hideDelay;


    private int _currentTutorial;
    private bool _tutorialOnDisplay;
    private WaitForSeconds _hideDelayWait;

    private void Awake()
    {
        _hideDelayWait = new WaitForSeconds(_hideDelay);
    }

    private void OnEnable()
    {
        if( _tutorialOnDisplay)
        {
            _currentTutorial--;
            StartTutorial();
        }
    }

    private void OnDisable()
    {
        foreach(var tutorial in _tutorials)
        {
            tutorial.TutorialButton.action.performed -= FinishTutorial;
        } 
    }

    public void StartTutorial()
    {
        DisplayTutorial();
        _currentTutorial++;
    }

    private void DisplayTutorial()
    {
        _animator.SetTrigger("ShowTut");
        _tutorials[_currentTutorial].TutorialObject.SetActive(true);
        _tutorialImage.texture = _tutorials[_currentTutorial].TutorialImage;
        _tutorialOnDisplay = true;
        _tutorials[_currentTutorial].TutorialButton.action.performed += FinishTutorial;
    }

    private void FinishTutorial(InputAction.CallbackContext _)
    {
        _tutorials[_currentTutorial - 1].TutorialObject.SetActive(false);
        _tutorialOnDisplay = false;
        _tutorials[_currentTutorial - 1].TutorialButton.action.performed -= FinishTutorial;
        StartCoroutine(DelayHide());
    }

    private IEnumerator DelayHide()
    {
        yield return _hideDelayWait;
        _animator.SetTrigger("Activated");
    }
}
