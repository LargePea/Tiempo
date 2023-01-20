using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HealthBar : MonoBehaviour
{
    private Animator _animator;

    private void Awake()
    {
        _animator = GetComponent<Animator>();
    }


    public void Damage()
    {
        _animator.SetTrigger("Damaged");
    }

    public void Heal()
    {
        _animator.SetTrigger("Healed");
    }

    public void SetLastLife(bool IsLastLife)
    {
        _animator.SetBool("LastLife", IsLastLife);
    }

    public IEnumerator ResetAnimator()
    {
        _animator.SetTrigger("Reset");
        yield return null;
        _animator.ResetTrigger("Reset");
        _animator.ResetTrigger("Healed");
        _animator.ResetTrigger("Damaged");
        _animator.SetBool("LastLife", false);
    }
}
