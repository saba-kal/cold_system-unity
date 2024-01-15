using System.Collections;
using System.ComponentModel;
using UnityEngine;

public class ProjectileWeapon : BaseWeapon
{
    [SerializeField] private float _projectileSpeed = 50f;
    [SerializeField] private float _projectileDamage = 5f;
    [SerializeField] private float _spreadFactor = 0.02f;
    [SerializeField] private int _burstCount = 1;
    [SerializeField] private float _timeBetweenBurstShots = 0.5f;
    [SerializeField] private Transform _projectileSpawnPoint;
    [SerializeField] private Projectile _projectilePrefab;
    [SerializeField] private GameObject _onProjectileHitEffectPrefab;
    [SerializeField]
    [Description("Usefull for when the weapon is mirrored, and therefore the forward direction is backwards.")]
    private bool _negateProjectileDirection = false;

    private float _timeSinceLastShot = 1f;
    private AudioSource _audioSource;

    private void Start()
    {
        _timeSinceLastShot = TimeBetweenShots;
        _audioSource = GetComponent<AudioSource>();
    }

    private void Update()
    {
        if (_enabled && _timeSinceLastShot >= TimeBetweenShots)
        {
            Fire();
        }

        _timeSinceLastShot += Time.deltaTime;
    }

    public override void Fire()
    {
        if (_burstCount <= 1)
        {
            SpawnProjectile();
        }
        else if (_timeBetweenBurstShots < 0.05f)
        {
            for (var i = 0; i < _burstCount; i++)
            {
                SpawnProjectile();
            }
        }
        else
        {
            StartCoroutine(SpawnProjectilesInBurst());
        }
    }


    private IEnumerator SpawnProjectilesInBurst()
    {
        for (var i = 0; i < _burstCount; i++)
        {
            SpawnProjectile();
            yield return new WaitForSeconds(_timeBetweenBurstShots);
        }
    }

    private void SpawnProjectile()
    {
        if (_projectileSpawnPoint == null)
        {
            Debug.LogError("How?");
        }

        var projectile = Instantiate(_projectilePrefab, _projectileSpawnPoint.transform.position, _projectileSpawnPoint.transform.rotation);
        var direction = _projectileSpawnPoint.transform.forward;
        if (_negateProjectileDirection)
        {
            direction = -direction;
        }
        direction.x += Random.Range(-_spreadFactor, _spreadFactor);
        direction.y += Random.Range(-_spreadFactor, _spreadFactor);
        direction.z += Random.Range(-_spreadFactor, _spreadFactor);

        projectile.Initialize(_projectileSpeed * direction, _projectileDamage, _parentUnitType, _onProjectileHitEffectPrefab);
        _timeSinceLastShot = 0;
        _audioSource?.Play();
        _onWeaponFired?.Invoke();
    }
}