using System.Collections;
using UnityEngine;

public class IdleState : BaseState
{
    public IdleState(Turret turret) : base(turret)
    {
    }


    private IEnumerator WaitWhileCanAttackPlayer()
    {
        while (!_turret.CanAttackPlayer())
        {
            yield return null;
        }

        SwitchState<AttackState>();
        yield break;
    }


    public override IEnumerator HandleState()
    {
        Debug.Log("Idle");

        if (_turret.CachedAnimator.enabled)
            _turret.CachedAnimator.enabled = false;

        _turret.TurnToStartPosition();

        if(_turret.TargetPlayer != null)
            yield return _turret.StartCoroutine(WaitWhileCanAttackPlayer());

        Debug.Log("Stop idle state");
        yield break;
    }
}
