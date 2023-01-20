using System.Collections;
using System.Collections.Generic;
using System;
using UnityEngine;
using UnityEngine.Events;

public class Checkpoint : MonoBehaviour
{
    [SerializeField] private bool _startingCheckPoint;
    [SerializeField] private Transform _spawnpoint;

    private static Checkpoint _currentCheckpoint;

    public Transform Spawnpoint => _spawnpoint;

    private void Start()
    {
        if(_startingCheckPoint)
        {
            SetCheckpoint();
            FindObjectOfType<PlayerController>().transform.position = _spawnpoint.position;
        }
    }
    public static void Respawn(GameObject player)
    {  
        if (!_currentCheckpoint) player.transform.position = Vector3.zero;
        else player.transform.SetPositionAndRotation(_currentCheckpoint.Spawnpoint.position, _currentCheckpoint.Spawnpoint.rotation);
            
    }

    public void SetCheckpoint()
    {
        FindObjectOfType<Elevator>().CheckpointUpdate();
        Debugger debugger = FindObjectOfType<Debugger>();
        debugger.CheckpointCounter = Array.IndexOf(debugger.Checkpoints, this);
        _currentCheckpoint = this;
    }
}
