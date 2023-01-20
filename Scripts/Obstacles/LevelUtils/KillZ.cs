using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class KillZ : MonoBehaviour
{
    public UnityEvent OnPlayerDeath;
    public void Kill(Collider other)
    {
        //respawn the player
        if(other.TryGetComponent(out Health health))
        {
            OnPlayerDeath.Invoke();
            health.Damage(new DamageInfo() { Amount = 99999999 });
        }

        //TO:DO implement AI respawning
        else if(other.TryGetComponent(out EnemyMovement enemyMovement))
        {
            enemyMovement.Dead();
        }
    }
}
