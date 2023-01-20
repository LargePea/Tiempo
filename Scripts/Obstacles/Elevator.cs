using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Elevator : MonoBehaviour
{
    [SerializeField] private float _speed;
    [SerializeField] private float[] _yLevels;
    [SerializeField] private GameObject _electricity;
    [SerializeField] private Transform _leftElectric;
    [SerializeField] private Transform _rightElectric;
    [SerializeField] private Transform[] _leftElectricTargets;
    [SerializeField] private Transform[] _rightElectricTargets;

    private bool _enabled;
    private int _nextLevel;
    private Vector3 _startPos;
    private Vector3 _endPos => new (_startPos.x, _yLevels[_nextLevel % _yLevels.Length], _startPos.z);
    private Rigidbody _rigidbody;

    private bool _initalEnabled;
    private int _initalLevel = 0;
    private Vector3 _respawnPosition;

    private void Awake()
    {
        _rigidbody = GetComponent<Rigidbody>();
        _respawnPosition = transform.position;
    }

    private void FixedUpdate()
    {
        if (_enabled)
        {
            if (Vector3.Distance(transform.position, _endPos) > 0.05f)
            {
                _rigidbody.velocity = Vector3.up * _speed;
            }
            else
            {
                _enabled = false;
                _rigidbody.velocity = Vector3.zero;
                _electricity.SetActive(true);
                _rightElectric.position = _rightElectricTargets[_nextLevel].position;
                _leftElectric.position = _leftElectricTargets[_nextLevel].position;
            }
        }

        if (!_rightElectricTargets[_initalLevel].gameObject.activeInHierarchy) _rightElectric.localPosition = Vector3.zero;
        if (!_leftElectricTargets[_initalLevel].gameObject.activeInHierarchy) _leftElectric.localPosition = Vector3.zero;
    }

    public void MoveElevator()
    {
        _enabled = true;
        _nextLevel++;
        _initalLevel = _nextLevel;
        _electricity.SetActive(false);
    }

    public void StartElevator()
    {
        _enabled = true;
        _initalLevel = _nextLevel;
    }

    public void SaveState()
    {
        _initalEnabled = _enabled;
    }

    public void ReloadState()
    {
        _enabled = _initalEnabled;
        if(!_enabled) _rigidbody.velocity = Vector3.zero;
    }

    public void CheckpointUpdate()
    {
        _respawnPosition = transform.position;
    }

    public void Respawn()
    {
        transform.position = _respawnPosition;
        _enabled = true;
        _nextLevel = _initalLevel;
    }
}
