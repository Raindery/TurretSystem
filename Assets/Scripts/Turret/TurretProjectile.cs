using UnityEngine;

[RequireComponent(typeof(CapsuleCollider))]
[RequireComponent(typeof(Rigidbody))]
public class TurretProjectile : MonoBehaviour
{
    [SerializeField] private int _damage;


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

    private CapsuleCollider _cachedCollider;
    public CapsuleCollider CachedCollider
    {
        get
        {
            if (_cachedCollider == null)
                _cachedCollider = GetComponent<CapsuleCollider>();
            return _cachedCollider;
        }
    }

    private Rigidbody _cachedRigidbody;
    public Rigidbody CachedRigidbody
    {
        get
        {
            if (_cachedRigidbody == null)
                _cachedRigidbody = GetComponent<Rigidbody>();
            return _cachedRigidbody;
        }
    }


    private void OnTriggerEnter(Collider other)
    {
        if(other.TryGetComponent<Player>(out Player player))
        {
            player.AddDamage(_damage);
            Destroy(gameObject);
        }

        if (other.gameObject.layer == LayerMask.NameToLayer("Obstacle"))
            Destroy(gameObject);
    }


    public void StartProjectile(Vector3 direction, float speed)
    {
        CachedRigidbody.AddForce(direction * speed, ForceMode.VelocityChange);
        Destroy(gameObject, 10f);
    }
}
