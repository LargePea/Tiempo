using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.VFX;

public class PauseUI : MonoBehaviour
{
    [SerializeField] private VisualEffect[] _pauseBars;
    [SerializeField] private VisualEffect[] _diageticPauseBars;
    [SerializeField] private PausingController _pauseController;

    private int _pausedObjects = 0;
    private int _initalPausedObjects;
    public void UpdatePauseTimer()
    {
        _pausedObjects = Mathf.Clamp(_pauseController._pasuedLinkedList.Count, 0, _pauseBars.Length);
        for(int i = 0; i < _pauseBars.Length; i++)
        {
            _pauseBars[i].SetFloat("Fill", 1);
            _diageticPauseBars[i].SetFloat("Fill", 1);
        }
        for(int i = 0; i < _pausedObjects; i++)
        {
            _pauseBars[i].SetFloat("Fill", 0);
            _diageticPauseBars[i].SetFloat("Fill", 0);
        }
    }

    public void SaveState()
    {
        _initalPausedObjects = _pausedObjects;
    }

    public void ReloadState()
    {
        _pausedObjects = _initalPausedObjects;
        for (int i = 0; i < _pauseBars.Length; i++)
        {
            _pauseBars[i].SetFloat("Fill", 1);
            _diageticPauseBars[i].SetFloat("Fill", 1);
        }
        for (int i = 0; i < _pausedObjects; i++)
        {
            _pauseBars[i].SetFloat("Fill", 0);
            _diageticPauseBars[i].SetFloat("Fill", 0);
        }
    }
}
