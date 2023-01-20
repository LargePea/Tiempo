using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.VFX;

public class RewindUI : MonoBehaviour
{
    [SerializeField] VisualEffect _rewindMaterial;
    [SerializeField] VisualEffect _diageticRewindMaterial;
    [SerializeField] CharacterRewind _characterRewind;

    private IEnumerator _decreaseCo;
    private float RewindDuration => _characterRewind.RewindDuration;
    private float _rewindElapsed;

    private void Awake()
    {
        _rewindMaterial.SetFloat("Fill", 1);
    }

    private void OnEnable()
    {
        if (_decreaseCo != null)
        {
            StartCoroutine(_decreaseCo);
        }
    }

    public void ToggleFill(bool isDecreasing)
    {
        if(_decreaseCo != null) StopCoroutine(_decreaseCo);

        if (isDecreasing) _decreaseCo = Decrease();
        else _decreaseCo = Increase();

        StartCoroutine(_decreaseCo);
    }

    private IEnumerator Decrease()
    {
        while(_rewindElapsed < RewindDuration)
        {
            _rewindElapsed += Time.deltaTime;
            _rewindMaterial.SetFloat("Fill", 1 - Mathf.Clamp01(_rewindElapsed / RewindDuration));
            _diageticRewindMaterial.SetFloat("Fill", 1 - Mathf.Clamp01(_rewindElapsed / RewindDuration));
            yield return null;
        }

        _decreaseCo = null;
    }

    private IEnumerator Increase()
    {
        while (_rewindElapsed > 0)
        {
            _rewindElapsed -= Time.deltaTime;
            _rewindMaterial.SetFloat("Fill", 1 - Mathf.Clamp01(_rewindElapsed / RewindDuration));
            _diageticRewindMaterial.SetFloat("Fill", 1 - Mathf.Clamp01(_rewindElapsed / RewindDuration));
            yield return null;
        }

        _decreaseCo = null;
    }

    public void StopChange()
    {
        if (_decreaseCo != null)
        {
            StopCoroutine(_decreaseCo);
            _decreaseCo = null;
        }
    }

    public void ResetBar()
    {
        _rewindMaterial.SetFloat("Fill", 1);
        _diageticRewindMaterial.SetFloat("Fill", 1);
        _rewindElapsed = 0;
    }
}
