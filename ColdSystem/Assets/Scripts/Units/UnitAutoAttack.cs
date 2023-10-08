using UnityEngine;

public class UnitAutoAttack : MonoBehaviour
{
    [SerializeField] private GameObject _turret;
    [SerializeField] private float _turretRotationSpeed = 5f;
    [SerializeField] private float _range = 20f;
    [SerializeField] private bool _showRange = false;

    private BaseWeapon[] _weapons;
    private Unit[] _opposingUnits;

    private void Start()
    {
        _weapons = GetComponentsInChildren<BaseWeapon>();
    }

    private void Update()
    {
        var turretIsFacingTarget = false;
        var target = GetNearestOpposingUnit();
        if (target != null && _turret != null)
        {
            turretIsFacingTarget = RotatetToFaceTarget(target.gameObject);
        }
        else
        {
            ResetAllRotation();
        }
        SetWeaponsEnabled(turretIsFacingTarget);
    }

    public void SetOpposingUnits(Unit[] units)
    {
        _opposingUnits = units;
    }

    private Unit GetNearestOpposingUnit()
    {
        Unit nearestOpponent = null;
        var minDistanceSqr = _range * _range;
        foreach (var opposingUnit in _opposingUnits)
        {
            var distanceSqr = (opposingUnit.transform.position - transform.position).sqrMagnitude;
            if (distanceSqr < minDistanceSqr)
            {
                minDistanceSqr = distanceSqr;
                nearestOpponent = opposingUnit;
            }
        }

        return nearestOpponent;
    }

    private bool RotatetToFaceTarget(GameObject target)
    {
        var turretIsFacingTarget = RotateToFaceTarget(_turret, target, new Vector3(0, 1, 0), -360, 360);
        foreach (var weapon in _weapons)
        {
            RotateToFaceTarget(weapon.gameObject, target, new Vector3(1, 1, 1), -10, 10);
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

    private bool RotateToFaceTarget(GameObject objectToRotate, GameObject target, Vector3 axis, float minAngle, float maxAngle)
    {
        var targetDirection = target.transform.position - objectToRotate.transform.position;
        var desiredRotation = Quaternion.LookRotation(targetDirection);
        desiredRotation = Quaternion.Euler(Vector3.Scale(desiredRotation.eulerAngles, axis));
        var rotation = Quaternion.Slerp(objectToRotate.transform.rotation, desiredRotation, _turretRotationSpeed * Time.deltaTime);
        objectToRotate.transform.rotation = rotation;
        objectToRotate.transform.localRotation = Quaternion.Euler(
            ClampAngle(objectToRotate.transform.localRotation.eulerAngles.x, minAngle, maxAngle),
            ClampAngle(objectToRotate.transform.localRotation.eulerAngles.y, minAngle, maxAngle),
            ClampAngle(objectToRotate.transform.localRotation.eulerAngles.z, minAngle, maxAngle));
        return Mathf.Abs(_turret.transform.eulerAngles.y - desiredRotation.eulerAngles.y) < 1;
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

    private void OnDrawGizmos()
    {
        if (_showRange)
        {
            Gizmos.color = new Color(1, 0, 0, 0.1f);
            Gizmos.DrawSphere(transform.position, _range);
        }
    }
}
