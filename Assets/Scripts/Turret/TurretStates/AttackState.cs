using System.Collections;
using UnityEngine;

public class AttackState : BaseState
{
    private readonly float _startAttackDuration;
    private readonly float _shootDuration;

    public AttackState(Turret turret, float attackDuration, float shootDuration) : base(turret)
    {
        _startAttackDuration = attackDuration;
        _shootDuration = shootDuration;
    }


    private IEnumerator AttackPlayer()
    {
        _turret.FollowToPlayer();
        yield return new WaitForSeconds(_startAttackDuration);

        while (_turret.TargetPlayer != null && _turret.CanAttackPlayer())
        {
            Debug.Log("Attack!");
            _turret.Shoot();
            _turret.PlayState("Attack", false);
            yield return new WaitForSeconds(_shootDuration);
            yield return null;
        }

        SwitchState<AlertState>();
        yield break;
    }


    public override IEnumerator HandleState()
    {
        Debug.Log("Attack");

        if (!_turret.CachedAnimator.enabled)
            _turret.CachedAnimator.enabled = true;

        yield return new WaitForSeconds(_turret.CachedAnimator.GetCurrentAnimatorStateInfo(0).length);
        yield return _turret.StartCoroutine(AttackPlayer());


        Debug.Log("Stop attack state");
        yield break;
    }
}
