﻿using System.ComponentModel;
using UnityEngine;

public class ProjectileWeapon : BaseWeapon
{
    [SerializeField] private float _projectileSpeed = 50f;
    [SerializeField] private float _projectileDamage = 5f;
    [SerializeField] private float _timeBetweenShots = 1f;
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
        _timeSinceLastShot = _timeBetweenShots;
        _audioSource = GetComponent<AudioSource>();
    }

    private void Update()
    {
        if (_enabled && _timeSinceLastShot >= _timeBetweenShots)
        {
            var projectile = Instantiate(_projectilePrefab, _projectileSpawnPoint.transform.position, _projectileSpawnPoint.transform.rotation);
            var direction = _projectileSpawnPoint.transform.forward;
            if (_negateProjectileDirection)
            {
                direction = -direction;
            }
            projectile.Initialize(_projectileSpeed * direction, _projectileDamage, _parentUnitType, _onProjectileHitEffectPrefab);
            _timeSinceLastShot = 0;
            _audioSource?.Play();
            _onWeaponFired?.Invoke();
        }

        _timeSinceLastShot += Time.deltaTime;
    }
}