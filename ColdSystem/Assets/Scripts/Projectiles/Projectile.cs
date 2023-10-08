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

    public void Initialize(Vector3 velocity, float damage)
    {
        GetComponent<Rigidbody>().velocity = velocity;
        _damage = damage;
    }

    private void OnCollisionEnter(Collision collision)
    {
        Debug.Log($"Bullet hit! name: {collision.gameObject.name}");
        Destroy(gameObject);
    }
}