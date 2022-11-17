using UnityEngine;
using System;

[RequireComponent(typeof(PlayerMovementController))]
[RequireComponent(typeof(CapsuleCollider))]
public class Player : MonoBehaviour
{
    [SerializeField] private int _health = 100;


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

    private PlayerMovementController _cachedMovementController;
    public PlayerMovementController CachedMovementController
    {
        get
        {
            if (_cachedMovementController == null)
                _cachedMovementController = GetComponent<PlayerMovementController>();
            return _cachedMovementController;
        }
    }

    private CapsuleCollider _cachedPlayerCollider;
    public CapsuleCollider CachedPlayerCollider
    {
        get
        {
            if (_cachedPlayerCollider == null)
                _cachedPlayerCollider = GetComponent<CapsuleCollider>();
            return _cachedPlayerCollider;
        }
    }


    public void OnValidate()
    {
        if (_health <= 0)
            _health = 1;
    }


    public void AddDamage(int damage)
    {
        if (damage < 0)
            throw new Exception("Damage less then null");

        _health -= damage;

        if (_health <= 0)
        {
            Destroy(gameObject);
            PlayerEvents.CharacterDead.Invoke();
        }
            
            
    }
}