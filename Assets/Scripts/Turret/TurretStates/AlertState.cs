using System.Collections;
using UnityEngine;

public class AlertState : BaseState
{
    private const float ALERT_INCREMENT_TIME_VALUE = 0.2f;


    private readonly float _alertTime;


    public AlertState(Turret turret, float alertTime) : base(turret)
    {
        _alertTime = alertTime;
    }


    private IEnumerator AlertAction()
    {
        float alertTime = 0f;
        while (alertTime <= _alertTime && _turret.TargetPlayer != null)
        {
            if (_turret.CanAttackPlayer())
            {
                SwitchState<AttackState>();
                yield break;
            }

            _turret.PlayState("Alert", false);

            yield return new WaitForSeconds(ALERT_INCREMENT_TIME_VALUE);
            alertTime += ALERT_INCREMENT_TIME_VALUE;
            yield return null;
        }

        _turret.CachedAnimator.ResetTrigger("Alert");
        yield return _turret.PlayState("Off");
        SwitchState<IdleState>();
        yield break;
    }


    public override IEnumerator HandleState()
    {
        yield return _turret.StartCoroutine(AlertAction());
        yield break;
    }
}
