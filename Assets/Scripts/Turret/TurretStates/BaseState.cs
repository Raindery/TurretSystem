using System.Collections;
using UnityEngine;

public abstract class BaseState
{
    protected Turret _turret;
    protected Coroutine _stateCoroutine;


    public BaseState(Turret turret)
    {
        _turret = turret;
    }


    protected void SwitchState<T>()
        where T : BaseState
    {
        _turret.SwitchState<T>();
    }


    public void Start()
    {
        _stateCoroutine = _turret.StartCoroutine(HandleState());
    }

    public void Stop()
    {
        if (_stateCoroutine != null)
            _turret.StopCoroutine(_stateCoroutine);
    }

    public abstract IEnumerator HandleState();
}