using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HealthUIManager : MonoBehaviour
{
    [SerializeField] private HealthBar[] _healthBars;
    [SerializeField] private Health _health;

    private int _currentHUDHealth;

    private void Awake()
    {
        _currentHUDHealth = _health.MaxHealth;
    }

    public void UpdateHealth(int newHealth)
    {
        //if new health is greater than current then heal
        if (newHealth > _currentHUDHealth)
        {
            for(int i = _currentHUDHealth; i < newHealth; i++)
            {
                _healthBars[i].Heal();
            }

            _healthBars[0].SetLastLife(false);
        }

        //if less then damage
        else
        {
            for (int i = _currentHUDHealth; i > newHealth; i--)
            {
                _healthBars[i - 1].Damage();
            }

            if (newHealth == 1) _healthBars[0].SetLastLife(true);
        }

        //update current
        _currentHUDHealth = newHealth;
    }

    public void PlayerDeath()
    {
        _currentHUDHealth = _health.MaxHealth;
        for(int i = 0; i < _healthBars.Length; i++)
        {
            StartCoroutine(_healthBars[i].ResetAnimator());
        }
    }
}
