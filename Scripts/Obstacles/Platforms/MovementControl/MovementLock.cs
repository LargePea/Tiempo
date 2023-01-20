using UnityEngine;
using System.Collections;
using UnityEngine.Events;

public class MovementLock : MonoBehaviour
{
    [SerializeField] private GameObject[] _requiredActivatables;
    [SerializeField] private GameObject[] _lockedShaders;
    [SerializeField] private Renderer[] _dissolveFirst;
    [SerializeField] private Renderer[] _dissolveSecond;
    [SerializeField] private Renderer[] _dissolveThird;
    [SerializeField] private float _dissolveTotalDuration = 2f;

    private float _timeElapsed;

    private bool _initalUnlocked;
    private float _initalTimeElapsed;

    private ILockable _lockedObject;
    public UnityEvent OnUnlocked = new UnityEvent();
    public bool IsUnlocked;

    private void Awake()
    {
        _lockedObject = GetComponent<ILockable>();
        if(_lockedObject == null) this.enabled = false;

        if(_requiredActivatables.Length == 0)
        {
            if(_lockedObject != null) _lockedObject.Unlock();
            IsUnlocked = true;
            _timeElapsed = _dissolveTotalDuration;
            enabled = false;
        }
        else
        {
            if (_lockedObject != null) _lockedObject.Lock();
            foreach (GameObject shader in _lockedShaders) shader.SetActive(true);
            foreach (GameObject activatableObject in _requiredActivatables)
            {
                if(activatableObject.TryGetComponent(out IActivatable activatable))
                {
                    activatable.OnActivationUpdate.AddListener(CheckUnlocked);
                }
            }
        }
    }

    private void CheckUnlocked()
    {
        foreach(GameObject activatableObject in _requiredActivatables)
        {
            if(activatableObject.activeInHierarchy && activatableObject.TryGetComponent(out IActivatable activatable))
            {
                if (!activatable.IsActivated) return;
            }
        }

        if (!IsUnlocked)
        {
            IsUnlocked = true;
            StartCoroutine(Dissolve());
        }
    }

    private IEnumerator Dissolve()
    {
        bool unlocking = false;
        while(_timeElapsed < _dissolveTotalDuration)
        {
            unlocking = true;
            _timeElapsed += Time.deltaTime;
            if (_timeElapsed < _dissolveTotalDuration / 3)
            {
                foreach(Renderer renderer in _dissolveFirst) 
                    foreach(Material material in renderer.materials) material.SetFloat("_Dissolve", Mathf.Clamp01(_timeElapsed / (_dissolveTotalDuration / 3)));
            }
            else if (_timeElapsed < 2 * _dissolveTotalDuration / 3)
            {
                foreach (Renderer renderer in _dissolveSecond) 
                    foreach(Material material in renderer.materials) material.SetFloat("_Dissolve", Mathf.Clamp01((_timeElapsed - (_dissolveTotalDuration / 3)) / (_dissolveTotalDuration / 3)));
            }
            else
            {
                foreach (Renderer renderer in _dissolveThird)
                    foreach (Material material in renderer.materials) material.SetFloat("_Dissolve", Mathf.Clamp01((_timeElapsed -  ((2 * _dissolveTotalDuration) / 3)) / (_dissolveTotalDuration / 3)));
            }
            yield return null;
        }

        foreach(GameObject shader in _lockedShaders) shader.SetActive(false);
        yield return null; 

        _lockedObject.Unlock();
        foreach(GameObject activateableObject in _requiredActivatables) activateableObject.GetComponent<IActivatable>().LockActivateable();
        if(unlocking) OnUnlocked?.Invoke();
    }

    public void SaveState()
    {
        _initalTimeElapsed = _timeElapsed;
        _initalUnlocked = IsUnlocked;
    }

    public void ReloadState()
    {
        if(_requiredActivatables.Length != 0)
        {
            foreach (GameObject shader in _lockedShaders) shader.SetActive(true);
            foreach (Renderer renderer in _dissolveFirst)
                foreach (Material material in renderer.materials) material.SetFloat("_Dissolve", 0);
            foreach (Renderer renderer in _dissolveSecond)
                foreach (Material material in renderer.materials) material.SetFloat("_Dissolve", 0);
        }


        if (!_initalUnlocked)
        {
            _timeElapsed = 0;
            IsUnlocked = _initalUnlocked;
            foreach (GameObject activateableObject in _requiredActivatables)
            {
                activateableObject.TryGetComponent(out IActivatable activatable);
                activatable.UnlockActivateable();
            }
        }
        else
        {
            _timeElapsed = _initalTimeElapsed;
            IsUnlocked = _initalUnlocked;

            StartCoroutine(Dissolve());
        }
    }
}
