using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class Health: MonoBehaviour
{
    [Header("Health")]
    [SerializeField] private int _maxHealth = 3;    // current and default health
    [SerializeField] private float _regenSpeed = 10f;

    public bool IsAlive => _currentHealth > 0;          // check if the player is dead or not
    public int CurrentHealth => _currentHealth ;                        // to update health bar
    public int MaxHealth => _maxHealth ;
    [SerializeField] private int _currentHealth;
    private float _regenTimer;

    public UnityEvent<DamageInfo> OnDamage;
    public UnityEvent<DamageInfo> OnDeath;
    public UnityEvent<int> OnUpdateHealthBar;           // update ui
    public UnityEvent<int> OnSaveHealth;

    private Rewind _rewind;
    private float _initalRegenTime;
    private int _initalHealth;

    [Header("Respawning")]
    [SerializeField] Material _respawningShader;
    [SerializeField] private float _fadeTime = .25f;
    public UnityEvent OnRespawned;

    private void Start()
    {
        _rewind = GetComponent<Rewind>();
        _currentHealth = _maxHealth;
    }

    public void Damage(DamageInfo damageInfo)
    {
        // check if the player is dead or not
        if (!IsAlive) return;

        UpdateHealth(_currentHealth - damageInfo.Amount);    // set updating health value

        // do other action from event
        OnDamage.Invoke(damageInfo);
        _regenTimer = 0;
        

        // if current health = 0
        if (!IsAlive)
        {
            StartCoroutine(Respawning());
            OnDeath.Invoke(damageInfo);     // do action form event
        }
    }

    private void Update()
    {
        if (_rewind != null && _rewind.IsRewinding) return;
        if(_currentHealth < _maxHealth)
        {
            RegenHealth();
        }
    }

    private void RegenHealth()
    {
        if(_regenTimer < _regenSpeed)
        {
            _regenTimer += Time.deltaTime;
        }
        else
        {
            _regenTimer = 0;
            UpdateHealth(CurrentHealth + 1);
        }
    }
    
    public void UpdateHealth(int newHealth)
    {
        OnSaveHealth.Invoke(CurrentHealth);
        _currentHealth = Mathf.Clamp(newHealth, 0, _maxHealth);
        OnUpdateHealthBar.Invoke(CurrentHealth);
    }

    private IEnumerator Respawning()
    {
        if (_respawningShader == null) yield break;

        float timeElapsed = 0f;
        Color _rewindColor = _respawningShader.GetColor("_Color");
        _respawningShader.SetColor("_Color", Color.red);
        while (timeElapsed < _fadeTime)
        {
            timeElapsed += Time.deltaTime;
            _respawningShader.SetFloat("_FullScreenIntensity", Mathf.Lerp(0, 1, timeElapsed / _fadeTime));
            yield return null;
        }

        Checkpoint.Respawn(gameObject);
        OnRespawned.Invoke();
        _currentHealth = _maxHealth;
        UpdateHealth(_currentHealth);

        while (timeElapsed > 0)
        {
            timeElapsed -= Time.deltaTime;
            _respawningShader.SetFloat("_FullScreenIntensity", Mathf.Lerp(0, 1, timeElapsed / _fadeTime));
            yield return null;
        }
        _respawningShader.SetColor("_Color", _rewindColor);
    }

    public void SaveState()
    {
        _initalHealth = _currentHealth;
        _initalRegenTime = _regenTimer;
    }

    public void ReloadState()
    {
        _currentHealth = _initalHealth;
        _regenTimer = _initalRegenTime;
        OnUpdateHealthBar.Invoke(CurrentHealth);
    }
}
