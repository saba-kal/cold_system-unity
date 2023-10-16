using UnityEngine;

public class UnitAutoAttack : MonoBehaviour
{
    [SerializeField] private GameObject _turret;
    [SerializeField] private float _turretRotationSpeed = 5f;
    [SerializeField] private float _range = 20f;
    [SerializeField] private bool _showRange = false;

    private BaseWeapon[] _weapons;
    private Unit[] _opposingUnits;
    private Unit _target;
    private UnitType _type;

    private void Awake()
    {
        _weapons = GetComponentsInChildren<BaseWeapon>();
    }

    private void Update()
    {
        var turretIsFacingTarget = false;
        _target = GetNearestOpposingUnit();
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

    public void Initialize(Unit[] opposingUnits, UnitType type)
    {
        _type = type;
        _opposingUnits = opposingUnits;
        foreach (var weapon in _weapons)
        {
            weapon.Initialize(type);
        }
    }

    public bool IsAttacking()
    {
        return _target != null;
    }

    private Unit GetNearestOpposingUnit()
    {
        Unit nearestOpponent = null;
        var minDistanceSqr = _range * _range;
        foreach (var opposingUnit in _opposingUnits)
        {
            if (opposingUnit == null)
            {
                //Unit is dead.
                continue;
            }

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

    private void OnDrawGizmos()
    {
        if (_showRange)
        {
            Gizmos.color = new Color(1, 0, 0, 0.1f);
            Gizmos.DrawSphere(transform.position, _range);
        }
    }
}
