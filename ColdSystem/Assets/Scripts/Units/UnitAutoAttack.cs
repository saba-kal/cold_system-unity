using UnityEngine;

public class UnitAutoAttack : MonoBehaviour
{
    [SerializeField] private GameObject _turret;
    [SerializeField] private float _turretRotationSpeed = 5f;
    [SerializeField] private float _range = 20f;
    [SerializeField] private bool _showRange = false;

    private BaseWeapon[] _weapons;
    private UnitLineOfSight _unitLineOfSight;
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
        _target = _unitLineOfSight.GetNearestVisibleUnit();
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
        _unitLineOfSight = new UnitLineOfSight(GetComponent<Unit>(), opposingUnits, _range);
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
