using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Unit), typeof(WeaponManager))]
public class UnitAutoAttack : MonoBehaviour
{
    public delegate void WeaponFired();
    public event WeaponFired OnWeaponFired;

    public bool IsAttacking { get; private set; }
    public bool Enabled => _enabled;

    private bool _enabled = true;
    private float _range = 20f;
    private WeaponManager _weaponManager;
    private UnitTurret _turret;
    private Unit _unit;
    private Unit _target;
    private Unit _focusTarget; // Unlike the regular target, this one will be focused as long as it is visible.
    private UnitType _type;

    private void Awake()
    {
        _weaponManager = GetComponent<WeaponManager>();
        _turret = GetComponent<UnitTurret>();
        _unit = GetComponent<Unit>();
    }

    private void Update()
    {
        if (!_enabled)
        {
            IsAttacking = false;
            _turret?.SetTarget(null);
            _weaponManager.SetLookTarget(null);
            _weaponManager.SetWeaponsActive(false);
            return;
        }

        _target = GetNearestVisibleEnemy();
        if (_target != null && _turret != null)
        {
            _turret?.SetTarget(_target.transform);
            _weaponManager.SetLookTarget(_target);
        }
        else
        {
            _turret?.SetTarget(null);
            _weaponManager.SetLookTarget(null);
        }
        IsAttacking = _target != null;
        _weaponManager.SetWeaponsActive(IsAttacking && (_turret?.IsFacingTarget ?? false));
    }

    public void Initialize(UnitType type, float range)
    {
        _type = type;
        _range = range;
        _weaponManager.Initialize(type);
        _weaponManager.SetOnWeaponFired(OnBaseWeaponFired);
    }

    public Vector3 GetFaceDirectionEulerAngles()
    {
        if (_turret == null)
        {
            return transform.eulerAngles;
        }

        return _turret.transform.eulerAngles;
    }

    public bool UnitCanBeAttacked(Unit unit)
    {
        var distanceSqr = (unit.transform.position - transform.position).sqrMagnitude;
        return distanceSqr < _range * _range && unit.IsVisible;
    }

    public void SetTargetUnit(Unit unit)
    {
        _focusTarget = unit;
    }

    public void ToggleEnabled()
    {
        _enabled = !_enabled;
    }

    public void SetEnabled(bool enabled)
    {
        _enabled = enabled;
    }

    private Unit GetNearestVisibleEnemy()
    {
        var maxDistanceSqr = _range * _range;
        if (_focusTarget != null && _focusTarget.IsVisible &&
            (_focusTarget.transform.position - transform.position).sqrMagnitude < maxDistanceSqr)
        {
            return _focusTarget;
        }

        //Focus target is no longer visible. Remove the focus.
        if (_focusTarget != null && !_focusTarget.IsVisible)
        {
            _focusTarget = null;
        }

        List<Unit> opposingUnits = null;
        switch (_type)
        {
            case UnitType.Enemy:
                opposingUnits = PlayerUnitManager.Instance?.GetUnits();
                break;
            case UnitType.Player:
                opposingUnits = EnemyUnitManager.Instance?.GetUnits();
                break;
        }
        if (opposingUnits == null)
        {
            return null;
        }


        var minimumDistanceSqr = float.MaxValue;
        Unit nearestUnit = null;
        foreach (var opposingUnit in opposingUnits)
        {
            if (opposingUnit == null || !opposingUnit.IsVisible) continue;

            var distanceSqr = (opposingUnit.transform.position - transform.position).sqrMagnitude;
            if (distanceSqr < maxDistanceSqr && distanceSqr < minimumDistanceSqr &&
                _unit.TargetIsInLineOfSight(opposingUnit))
            {
                minimumDistanceSqr = distanceSqr;
                nearestUnit = opposingUnit;
            }
        }

        return nearestUnit;
    }

    private void OnBaseWeaponFired()
    {
        OnWeaponFired?.Invoke();
    }
}
