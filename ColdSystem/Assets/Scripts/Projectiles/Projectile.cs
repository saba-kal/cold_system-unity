using UnityEngine;

[RequireComponent(typeof(Collider), typeof(Rigidbody))]
public class Projectile : MonoBehaviour
{
    [SerializeField] private float _lifeTime = 10f;

    private float _damage;
    private GameObject _onDestroyEffect;

    private void Awake()
    {
        Destroy(gameObject, _lifeTime);
    }

    public void Initialize(Vector3 velocity, float damage, UnitType parentUnitType, GameObject onDestroyEffect)
    {
        GetComponent<Rigidbody>().velocity = velocity;
        _damage = damage;
        gameObject.layer = UnitFunctions.GetProjectileLayer(parentUnitType);
        _onDestroyEffect = onDestroyEffect;
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (_onDestroyEffect != null)
        {
            var effect = Instantiate(_onDestroyEffect);
            if (collision.contacts.Length > 0)
            {
                effect.transform.position = collision.contacts[0].point;
            }
            else
            {
                effect.transform.position = transform.position;
            }
        }
        Destroy(gameObject);
    }
}