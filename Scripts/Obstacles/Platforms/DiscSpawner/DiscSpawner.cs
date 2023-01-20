using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.VFX;

public class DiscSpawner : MonoBehaviour
{
    [Header("Spawning")]
    [SerializeField] private float _spawnTime;
    [SerializeField] private float _travelDistance;
    [SerializeField] private float _speed;
    [SerializeField] private GameObject _light;

    [Header("Spawner Reqs")]
    [SerializeField] private Projectile _disc;
    [SerializeField] private Animator _animator;
    [SerializeField] private Material _material;
    [SerializeField] private VisualEffect _steam;
    [SerializeField] private bool debug;

    [Header("SFX")]
    [SerializeField] private FMODEventReference _startSFX;
    public bool IsPlayerEnter { set; get; }

    private bool _canSpawn = true;
    private float _timeElapsed;

    //rewind variables
    private float _initalTimeElapsed;
    private bool _initalCanSpawn;

    public void SpawnDisc()
    {
        _canSpawn = false;
        _disc.OnArrival.AddListener(ReloadSlinger);
        _disc.Init(_travelDistance, _speed);

    }

    private void FixedUpdate()
    {
        if (!_canSpawn)
        {
            if (!_disc.CanMove) _canSpawn = true;
            return;
        }

        _timeElapsed += Time.fixedDeltaTime;
        if(_timeElapsed > _spawnTime - 0.5f && !_light.activeInHierarchy) _light.SetActive(true);

        if( _timeElapsed >= _spawnTime)
        {
            _timeElapsed = 0;
            _animator.SetTrigger("Spawn");
        }
    }

    public void PlaySteam()
    {
        _steam.Play();
    }

    public void TurnOffLight()
    {
        _light.SetActive(false);
    }

    private void ReloadSlinger()
    {
        _canSpawn = true;
        _disc.OnArrival.RemoveAllListeners();
    }

    public void SaveState()
    {
        _initalCanSpawn = _canSpawn;
        _initalTimeElapsed = _timeElapsed;
    }

    public void ReloadState()
    {
        _canSpawn = _initalCanSpawn;
        _timeElapsed = _initalTimeElapsed;
        if (!_canSpawn)
        {
            _disc.OnArrival.RemoveAllListeners();
            _disc.OnArrival.AddListener(ReloadSlinger);
        }
    }

    public void ResetSpawner()
    {
        _canSpawn = true;
        _timeElapsed = 0;
        _animator.ResetTrigger("Spawn");
        StartCoroutine(ResetAnimator());
    }

    private IEnumerator ResetAnimator()
    {
        _animator.SetTrigger("Reset");
        yield return null;
        _animator.ResetTrigger("Reset");
    }

    // SFX
    public void ShootSFX()
    {
        if (!IsPlayerEnter) return;
        _startSFX.PlayOneShotAtLocation(transform.position);
    }
}
