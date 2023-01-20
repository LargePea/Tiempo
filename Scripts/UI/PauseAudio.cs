using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using FMOD.Studio;
using FMODUnity;

public class PauseAudio : MonoBehaviour
{
    private EventInstance BT;


    private void Awake()
    {
        BT = RuntimeManager.CreateInstance("snapshot:/TimePause");
        BT.start();
    }

    private void OnEnable()
    {
        BT.setParameterByName("TimePause", 1);
    }

    private void OnDisable()
    {
        BT.setParameterByName("TimePause", 0);
    }
}
