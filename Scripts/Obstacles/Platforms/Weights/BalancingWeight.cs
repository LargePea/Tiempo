using System.Collections;
using UnityEngine;

public class BalancingWeight : Pushable
{
    [SerializeField] private AffectedWeight[] _affectedWeights;

    private float _targetDisplacement = 0f;
    public float TargetDisplacement => _startingPos.y + _targetDisplacement;

    protected override void MoveObject(bool isTriggered)
    {
        //update displacement only if the triggered is updated
        if(_wasTriggered == isTriggered) return;
        _wasTriggered = isTriggered;

        // add displacement if weight is triggered otherwise don't
        foreach(AffectedWeight affectedWeight in _affectedWeights)
        {
            float affectedWeightDisplacement = isTriggered ? affectedWeight.Displacement : -affectedWeight.Displacement;
            affectedWeight.Weight.UpdateDestinationAndMove(affectedWeightDisplacement);
        }
        float displacement = isTriggered ? _displacement : -_displacement;
        UpdateDestinationAndMove(displacement);
    }

    public void UpdateDestinationAndMove(float additionalOffset)
    {
        _targetDisplacement += additionalOffset;
        StartLerp();
    }

    protected override IEnumerator LerpObject()
    {
        //manually override activation when moving
        IsActivated = false;
        while(Vector3.Distance(transform.position, _startingPos + (Vector3.up * _targetDisplacement)) > _pointReachDistance)
        {
            if(IsRewinding) yield break;
            Vector3 direction = (_startingPos + (Vector3.up * _targetDisplacement) - transform.position).normalized;
            _rigidbody.velocity = direction * _speed;
            yield return CoroutineWaitTimes.WaitForFixedUpdate;
        }
        IsActivated = CheckBalanced();
        _rigidbody.velocity = Vector3.zero;
        OnActivationUpdate.Invoke();
    }

    private bool CheckBalanced()
    {
        //check if all connected weights are balanced
        foreach(AffectedWeight affectedWeight in _affectedWeights)
        {
            if(Mathf.Abs(affectedWeight.Weight.TargetDisplacement - transform.position.y) > 0.2)
            {
                return false;
            }
        }
        return true;
    }

    public override void PlayerDeath()
    {
        base.PlayerDeath();
        _targetDisplacement = 0f;
    }
}
