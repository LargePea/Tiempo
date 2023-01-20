using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.SceneManagement;

public class Debugger : MonoBehaviour
{
    [SerializeField] private Checkpoint[] _checkpoints;
    [SerializeField] private bool _enableDebug = true;
    public UnityEvent LoadScene = new UnityEvent();
    public int CheckpointCounter { get; set; }
    public Checkpoint[] Checkpoints { get { return _checkpoints; } }
    private Checkpoint _currentCheckPoint => _checkpoints[Mathf.Abs(CheckpointCounter % _checkpoints.Length)];

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Alpha0) && _enableDebug)
        {
            //reload gameplay scene
            LoadScene.Invoke();
        }
        else if (Input.GetKeyDown(KeyCode.Alpha1) && _enableDebug)
        {
            //decrease current checkpoint
            CheckpointCounter--;
            _currentCheckPoint.SetCheckpoint();
            Checkpoint.Respawn(gameObject);
        }
        else if (Input.GetKeyDown(KeyCode.Alpha2) && _enableDebug)
        {
            //increase current checkpoint
            CheckpointCounter++;
            _currentCheckPoint.SetCheckpoint();
            Checkpoint.Respawn(gameObject);
        }
        else if (Input.GetKeyDown(KeyCode.Alpha4) && _enableDebug)
        {
            SceneManager.LoadScene("Tiempo");
        }
        else if (Input.GetKeyDown(KeyCode.Alpha5) && _enableDebug)
        {
            if(Time.timeScale != 0) Time.timeScale = 0;
            else Time.timeScale = 1;
        }
    }
}
