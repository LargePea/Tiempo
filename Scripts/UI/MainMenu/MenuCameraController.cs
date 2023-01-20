using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Cinemachine;
using UnityEngine.Events;

public class MenuCameraController : MonoBehaviour
{
    [Header("Cameras")]
    [SerializeField] private CinemachineVirtualCamera _defaultCam;

    [Header("Transition")]
    [SerializeField] private float _duration = 1f;
    [SerializeField] private float _fadePoint = 0.75f;
    [SerializeField] private UnityEvent _onFadeToBlack = new UnityEvent();
    [SerializeField] private UnityEvent _onReachedTrackEnd = new UnityEvent();
    [SerializeField] private UnityEvent _onReachedTrackStart = new UnityEvent();

    private float _timeElapsed = 0f;
    private bool _flyToTarget;
    private bool _fadeToBlack = true;

    private CinemachineVirtualCamera _targetCam;
    private CinemachineTrackedDolly _targetTrack;

    //save position between scenes
    private static string _savedTargetCam = null;
    [SerializeField] private UnityEvent _onOverrideSceneStart = new UnityEvent();

    private void Start()
    {
        if(_savedTargetCam != null)
        {
            _onOverrideSceneStart.Invoke();
            GameObject _currentCam = GameObject.Find(_savedTargetCam);
            _currentCam.SetActive(true);
            _targetCam = _currentCam.GetComponent<CinemachineVirtualCamera>();
            _targetTrack = _targetCam.GetCinemachineComponent<CinemachineTrackedDolly>();
            _targetTrack.m_PathPosition = 1;
            ChooseCamMove(false);
        }
    }

    public void SaveScene()
    {
        _savedTargetCam = _targetCam.name;
    }

    public void SetTargetCam(CinemachineVirtualCamera targetCam)
    {
        _targetCam = targetCam;
        _targetTrack = _targetCam.GetCinemachineComponent<CinemachineTrackedDolly>();
    }

    public void ChooseCamMove(bool moveToTarget)
    {
        _flyToTarget = moveToTarget;
        _targetCam.Priority = 1;
        StartCoroutine(MoveCam());
    }

    private IEnumerator MoveCam()
    {
        while (_timeElapsed < _duration)
        {
            _timeElapsed += Time.deltaTime;

            _targetTrack.m_PathPosition = _flyToTarget ? _timeElapsed / _duration : 1 - (_timeElapsed / _duration);
            if (_fadeToBlack && _flyToTarget && _targetTrack.m_PathPosition > _fadePoint)
            {
                _fadeToBlack = false;
                _onFadeToBlack.Invoke();
            }
            yield return null;
        }

        if (_flyToTarget)
        {
            _onReachedTrackEnd.Invoke();
        }
        else
        {
            _onReachedTrackStart.Invoke();
            _targetCam.Priority = -1;
        } 

        _fadeToBlack = true;
        _timeElapsed = 0f;

        
    }
}
