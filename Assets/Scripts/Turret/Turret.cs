using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

[RequireComponent(typeof(SphereCollider))]
[RequireComponent(typeof(Animator))]
public class Turret : MonoBehaviour
{
    [Header("General")]
    [SerializeField] private float _alertTime;
    [SerializeField] private float _attackDuration;
    [SerializeField] private float _attackDistantion = 1f;
    [SerializeField] private float _shootDuration = 1f;
    [SerializeField] private float _speedProjectile = 1f;
    [SerializeField] private TurretProjectile _turretProjectile;
    [SerializeField] private Transform _shootPoint;
    [SerializeField] private LayerMask _interactionLayers;

    private BaseState _currentState;
    private Player _targetPlayer;
    private Quaternion _startRotation;
    private List<BaseState> _allAvailableStates;


    public Player TargetPlayer { get => _targetPlayer; }


    private Transform _cachedTransform;
    public Transform CachedTransform
    {
        get
        {
            if (_cachedTransform == null)
                _cachedTransform = transform;
            return _cachedTransform;
        }
    }

    private SphereCollider _cachedGetPlayerTrigger;
    public SphereCollider CachedGetPlayerTrigger
    {
        get
        {
            if (_cachedGetPlayerTrigger == null)
                _cachedGetPlayerTrigger = GetComponent<SphereCollider>();
            return _cachedGetPlayerTrigger;
        }
    }

    private Animator _cachedAnimator;
    public Animator CachedAnimator
    {
        get
        {
            if (_cachedAnimator == null)
                _cachedAnimator = GetComponent<Animator>();
            return _cachedAnimator;
        }
    }


    private void OnValidate()
    {
        if (_attackDistantion > CachedGetPlayerTrigger.radius)
            CachedGetPlayerTrigger.radius = _attackDistantion;

        if (_shootPoint == null)
            throw new ArgumentNullException(nameof(_shootPoint), $"Is null at {gameObject.name}");

        if (_turretProjectile == null)
            throw new ArgumentNullException(nameof(_turretProjectile), $"Is null at {gameObject.name}");
    }

    private void Awake()
    {
        _allAvailableStates = new List<BaseState>()
        {
            new IdleState(this),
            new AttackState(this, _attackDuration, _shootDuration),
            new AlertState(this, _alertTime)
        };

        _startRotation = CachedTransform.rotation;
    }

    private void Update()
    {
        if (_targetPlayer != null)
        {
            Debug.DrawRay(CachedTransform.position, _targetPlayer.CachedTransform.position - CachedTransform.position, Color.red, Time.deltaTime);
            
        }
            
    }

    private void OnTriggerEnter(Collider other)
    {
        if(other.TryGetComponent<Player>(out Player player))
        {
            _targetPlayer = player;
            SwitchState<IdleState>();
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.TryGetComponent<Player>(out _))
        {
            if (_currentState != null)
            {
                _currentState = null;
            }

            _targetPlayer = null;
        }
    }


    private Vector3 GetDistanceToPlayer()
    {
        return _targetPlayer.CachedTransform.position - CachedTransform.position;
    }


    public void SwitchState<T>()
        where T : BaseState
    {
        if(_currentState != null)
            _currentState.Stop();

        BaseState state = _allAvailableStates.FirstOrDefault(s => s is T);
        _currentState = state;
        _currentState.Start();
    }

    public void Shoot()
    {
        if (_targetPlayer == null)
            return;

        Quaternion projectileRotation = Quaternion.LookRotation(_shootPoint.position - _targetPlayer.CachedTransform.position); 
        var t = Instantiate(_turretProjectile, _shootPoint.position, projectileRotation);
        t.StartProjectile(GetDistanceToPlayer().normalized, _speedProjectile);
    }

    public bool CanAttackPlayer()
    {
        if (_targetPlayer == null)
            return false;

        bool result;

        Vector3 distanceVector = GetDistanceToPlayer();
        bool inAttackRange = distanceVector.magnitude <= _attackDistantion;
        result = inAttackRange;

        if (inAttackRange)
        {
            Ray ray = new Ray(CachedTransform.position, distanceVector);
            bool playerNotBehindObstacle = 
                Physics.Raycast(ray, out RaycastHit hitInfo, Mathf.Infinity, _interactionLayers) && hitInfo.collider.TryGetComponent<Player>(out _);

            Debug.Log(hitInfo.collider.gameObject.name);
            result = inAttackRange && playerNotBehindObstacle;
        }

        return result;
    }

    public void TurnToPlayer()
    {
        Quaternion newRotation = CachedTransform.rotation;
        Quaternion lookRotation = Quaternion.LookRotation(CachedTransform.position - _targetPlayer.CachedTransform.position);
        newRotation.y = lookRotation.y;
        newRotation.w = lookRotation.w;
        CachedTransform.rotation = newRotation;
    }

    public void TurnToStartPosition()
    {
        CachedTransform.rotation = _startRotation;
    }

    public Coroutine FollowToPlayer()
    {
        return StartCoroutine(FollowToPlayerCoroutine());
    }
    private IEnumerator FollowToPlayerCoroutine()
    {
        while (TargetPlayer != null && CanAttackPlayer())
        {
            TurnToPlayer();
            yield return null;
        }

        yield break;
    }

    public Coroutine PlayState(string trigger, bool waitEnd = true, float speed = 1f)
    {
        return StartCoroutine(PlayStateCoroutine(trigger, waitEnd, speed));
    }
    private IEnumerator PlayStateCoroutine(string trigger, bool waitEnd = true, float speed = 1f)
    {
        AnimatorStateInfo currentState;
        CachedAnimator.SetTrigger(trigger);
        yield return new WaitForSeconds(0.1f);
        currentState = CachedAnimator.GetCurrentAnimatorStateInfo(0);
        CachedAnimator.speed = speed;

        if (waitEnd)
            yield return new WaitForSeconds(currentState.length);
    }
}
