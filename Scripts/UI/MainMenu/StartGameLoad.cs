using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StartGameLoad : MonoBehaviour
{
    [SerializeField] private FloatSetting[] _settings;
    [SerializeField] private GammaSetting _gamma;

    private static bool _initialized = false;

    private void Awake()
    {
        if (_initialized) return;
        _initialized = true;
        //load in player pref settings
        foreach(var setting in _settings)
        {
            setting.Load();
        }

        _gamma.LoadBaseFiles();
    }
}
