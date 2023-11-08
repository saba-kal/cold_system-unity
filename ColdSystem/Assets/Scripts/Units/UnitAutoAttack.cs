using System.Collections.Generic;
using UnityEngine;

public class UnitAutoAttack : MonoBehaviour
{
    [SerializeField] private GameObject _turret;
    [SerializeField] private float _turretRotationSpeed = 5f;

    private float _range = 20f;
    private BaseWeapon[] _weapons;
    private Unit _target;
    private UnitType _type;

    private void Awake()
    {
        _weapons = GetComponentsInChildren<BaseWeapon>();
    }

    private void Update()
    {
        var turretIsFacingTarget = false;
        _target = GetNearestVisibleEnemy();
        if (_target != null && _turret != null)
        {
            turretIsFacingTarget = RotatetToFaceTarget(_target.gameObject);
        }
        else
        {
            ResetAllRotation();
        }
        SetWeaponsEnabled(turretIsFacingTarget);
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

    public bool IsAttacking()
    {
        return _target != null;
    }

    public float GetRange() => _range;

    public Vector3 GetFaceDirectionEulerAngles()
    {
        if (_turret == null)
        {
            return transform.eulerAngles;
        }

        return _turret.transform.eulerAngles;
    }

    private Unit GetNearestVisibleEnemy()
    {
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
        var maxDistanceSqr = _range * _range;
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

    private bool RotatetToFaceTarget(GameObject target)
    {
        var targetPos = target.transform.position + new Vector3(0, 2, 0);
        var turretIsFacingTarget = RotateToFaceTarget(_turret, targetPos, new Vector3(0, 1, 0));
        foreach (var weapon in _weapons)
        {
            RotateToFaceTarget(weapon.gameObject, targetPos, new Vector3(1, 0, 0));
        }

        return turretIsFacingTarget;
    }

    private void ResetAllRotation()
    {
        ResetRotation(_turret);
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

    private float ClampAngle(float angle, float minAngle, float maxAngle)
    {
        if (angle > 180)
        {
            angle -= 360;
        }
        return Mathf.Clamp(angle, minAngle, maxAngle);
    }

    private void SetWeaponsEnabled(bool enabled)
    {
        foreach (var weapon in _weapons)
        {
            weapon.SetEnabled(enabled);
        }
    }
}
