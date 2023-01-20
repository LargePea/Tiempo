using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AnimationWeight : MonoBehaviour
{
    [SerializeField] private float _dampTime = 0.1f;

    private bool _changeWeight;
    private int _increaseWeight;
    private float _elapsedTime = 0f;
    private Animator _animator;

    private void Awake()
    {
        _animator = GetComponent<Animator>();
    }

    public void TimeRewind()
    {
        _animator.SetTrigger("TimeRewind");
        ChangeWeight(1);
    }

    public void TimePause()
    {
        _animator.SetTrigger("TimePause");
        ChangeWeight(1);
    }

    public void ChangeWeight(int increase)
    {
        _increaseWeight = increase;
        _changeWeight = true;
    }

    private void Update()
    {
        if (_changeWeight)
        {
            _elapsedTime += Time.deltaTime;

            _animator.SetLayerWeight(1, Mathf.Clamp01(_increaseWeight == 1 ? _elapsedTime / _dampTime : 1 - _elapsedTime / _dampTime));

            //stop adjusting weight if finished adjusting
            if(_animator.GetLayerWeight(1) == 0 || _animator.GetLayerWeight(1) == 1)
            {
                _changeWeight = false;
                _elapsedTime = 0f;
            }

        }
    }
}
