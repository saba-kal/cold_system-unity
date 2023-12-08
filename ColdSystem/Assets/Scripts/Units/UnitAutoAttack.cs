using System.Collections.Generic;
using UnityEngine;

public class UnitAutoAttack : MonoBehaviour
{
    public delegate void WeaponFired();
    public event WeaponFired OnWeaponFired;

    public bool IsAttacking { get; private set; }
    public bool Enabled => _enabled;

    [SerializeField] private float _turretRotationSpeed = 5f;

    private bool _enabled = true;
    private float _range = 20f;
    private BaseWeapon[] _weapons;
    private UnitTurret _turret;
    private Unit _target;
    private Unit _focusTarget; // Unlike the regular target, this one will be focused as long as it is visible.
    private UnitType _type;

    private void Awake()
    {
        _weapons = GetComponentsInChildren<BaseWeapon>();
        _turret = GetComponent<UnitTurret>();
        foreach (var weapon in _weapons)
        {
            weapon.SetOnWeaponFired(OnBaseWeaponFired);
        }
    }

    private void Update()
    {
        if (!_enabled)
        {
            IsAttacking = false;
            ResetAllRotation();
            SetWeaponsEnabled(false);
            return;
        }

        _target = GetNearestVisibleEnemy();
        if (_target != null && _turret != null)
        {
            RotatetToFaceTarget(_target.gameObject);
        }
        else
        {
            ResetAllRotation();
        }
        IsAttacking = _target != null;
        SetWeaponsEnabled(IsAttacking && (_turret?.IsFacingTarget ?? false));
    }

    public void Initialize(UnitType type, float range)
    {
        _type = type;
        _range = range;
        foreach (var weapon in _weapons)
        {
            weapon.Initialize(type);
        }
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
        foreach (var unit in opposingUnits)
        {
            if (unit == null || !unit.IsVisible) continue;

            var distanceSqr = (unit.transform.position - transform.position).sqrMagnitude;
            if (distanceSqr < maxDistanceSqr && distanceSqr < minimumDistanceSqr)
            {
                minimumDistanceSqr = distanceSqr;
                nearestUnit = unit;
            }
        }

        return nearestUnit;
    }

    private void RotatetToFaceTarget(GameObject target)
    {
        var targetPos = target.transform.position + new Vector3(0, 2, 0);
        _turret?.SetTarget(target.transform); ;
        foreach (var weapon in _weapons)
        {
            RotateToFaceTarget(weapon.gameObject, targetPos, new Vector3(1, 0, 0));
        }
    }

    private void ResetAllRotation()
    {
        _turret?.SetTarget(null);
        foreach (var weapon in _weapons)
        {
            ResetRotation(weapon.gameObject);
        }
    }

    private void ResetRotation(GameObject objectToRotate)
    {
        objectToRotate.transform.localRotation = Quaternion.Lerp(
            objectToRotate.transform.localRotation,
            Quaternion.Euler(0, 0, 0),
            _turretRotationSpeed * Time.deltaTime);
    }

    private bool RotateToFaceTarget(GameObject objectToRotate, Vector3 targetWorldPosition, Vector3 axis)
    {
        var originalRotation = objectToRotate.transform.localRotation;
        objectToRotate.transform.LookAt(targetWorldPosition);
        var rotation = Quaternion.Slerp(originalRotation, objectToRotate.transform.localRotation, _turretRotationSpeed * Time.deltaTime);
        rotation = Quaternion.Euler(Vector3.Scale(rotation.eulerAngles, axis));
        objectToRotate.transform.localRotation = rotation;
        return true;
    }

    private void SetWeaponsEnabled(bool enabled)
    {
        foreach (var weapon in _weapons)
        {
            weapon.SetEnabled(enabled);
        }
    }

    private void OnBaseWeaponFired()
    {
        OnWeaponFired?.Invoke();
    }
}
