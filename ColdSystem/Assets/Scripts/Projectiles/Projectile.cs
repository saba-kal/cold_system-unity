using UnityEngine;

[RequireComponent(typeof(Collider), typeof(Rigidbody))]
public class Projectile : MonoBehaviour
{
    [SerializeField] private float _lifeTime = 10f;

    private float _damage;

    private void Awake()
    {
        Destroy(gameObject, _lifeTime);
    }

    public void Initialize(Vector3 velocity, float damage, UnitType parentUnitType)
    {
        GetComponent<Rigidbody>().velocity = velocity;
        _damage = damage;
        gameObject.layer = UnitFunctions.GetProjectileLayer(parentUnitType);
    }

    private void OnCollisionEnter(Collision collision)
    {
        //TODO: on destroy effect.
        Destroy(gameObject);
    }
}