using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class WeaponManager : MonoBehaviour
{
    [Header("Only set these if testing weapons. By default, leave them alone.")]
    [SerializeField] private bool _weaponsActive = false;
    [SerializeField] private Unit _testTarget;

    private UnitTurret _turret;
    private List<BaseWeapon> _weapons;
    private List<WeaponMount> _mounts;
    private bool _fireWeapopnsSequentially = false;
    private float _timeSinceLastShot = 1f;
    private float _timeBetweenShots = 1f;
    private int _weaponIndex = 0;

    private void Awake()
    {
        _turret = GetComponent<UnitTurret>();
        if (_weaponsActive)
        {
            Initialize(UnitType.Player);
            if (_testTarget != null)
            {
                SetLookTarget(_testTarget);
            }
        }
    }

    private void Update()
    {
        if (_weapons == null || _weapons.Count == 0)
        {
            return;
        }

        if (_fireWeapopnsSequentially && _weaponsActive && _timeSinceLastShot >= _timeBetweenShots)
        {
            _weapons[_weaponIndex].Fire();
            _weaponIndex = (_weaponIndex + 1) % _weapons.Count;
            _timeSinceLastShot = 0;
        }

        _timeSinceLastShot += Time.deltaTime;
    }

    public void Initialize(UnitType type)
    {
        _weapons = GetComponentsInChildren<BaseWeapon>().ToList();
        foreach (var turretWeapon in _turret?.GetWeapons() ?? new List<BaseWeapon>())
        {
            if (!_weapons.Contains(turretWeapon))
            {
                _weapons.Add(turretWeapon);
            }
        }

        _mounts = GetComponentsInChildren<WeaponMount>().ToList();
        foreach (var turretMount in _turret?.GetWeaponMounts() ?? new List<WeaponMount>())
        {
            if (!_mounts.Contains(turretMount))
            {
                _mounts.Add(turretMount);
            }
        }

        if (_weapons == null || _weapons.Count == 0)
        {
            Debug.LogError("Weapon manager could not find any weapons to fire. Make sure weapons are added as child game objects.");
            return;
        }

        _timeBetweenShots = _weapons[0].TimeBetweenShots;
        _fireWeapopnsSequentially = _weapons.All(w => Mathf.Approximately(w.TimeBetweenShots, _timeBetweenShots));
        if (_fireWeapopnsSequentially)
        {
            _timeBetweenShots /= _weapons.Count;
        }

        SetWeaponsActive(_weaponsActive);

        foreach (var weapon in _weapons)
        {
            weapon.Initialize(type);
        }
    }

    public void SetWeaponsActive(bool weaponsActive)
    {
        _weaponsActive = weaponsActive;
        if (!_fireWeapopnsSequentially)
        {
            foreach (var weapon in _weapons)
            {
                weapon.SetEnabled(weaponsActive);
            }
        }
    }

    public void SetLookTarget(Unit target)
    {
        foreach (var mount in _mounts)
        {
            mount.SetLookTarget(target);
        }
    }

    public void SetOnWeaponFired(Action onWeaponFired)
    {
        foreach (var weapon in _weapons)
        {
            weapon.SetOnWeaponFired(onWeaponFired);
        }
    }
}