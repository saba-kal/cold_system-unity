using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class UnitTurret : MonoBehaviour
{
    public bool IsFacingTarget { get; private set; }

    [SerializeField] private GameObject _turret;
    [SerializeField] private float _rotationSpeed = 25f;

    private Transform _target = null;
    private Transform _turretParent = null;
    private Vector3 _turretLocalPosition = Vector3.zero;

    private void Start()
    {
        // Turret rotation needs to be independent of mech rotation. Therefore, we need to remove it as a child of the mech.
        _turretLocalPosition = _turret.transform.localPosition;
        _turretParent = _turret.transform.parent;
        _turret.transform.parent = null;
    }

    private void Update()
    {
        if (_target == null)
        {
            ResetRotation();
        }
        else
        {
            RotateToFaceTarget();
        }

        _turret.transform.position = _turretParent.position + _turretLocalPosition;
    }

    public void SetTarget(Transform target)
    {
        _target = target;
    }

    public Vector3 GetFaceDirectionEulerAngles()
    {
        return _turret.transform.eulerAngles;
    }

    public Vector3 GetFaceDirection()
    {
        return _turret.transform.forward;
    }

    public void DestroyTurret()
    {
        Destroy(_turret);
    }

    public List<BaseWeapon> GetWeapons()
    {
        return _turret.GetComponentsInChildren<BaseWeapon>().ToList();
    }

    public List<WeaponMount> GetWeaponMounts()
    {
        return _turret.GetComponentsInChildren<WeaponMount>().ToList();
    }

    private void ResetRotation()
    {
        _turret.transform.rotation = Quaternion.RotateTowards(
            _turret.transform.rotation,
            _turretParent.rotation,
            _rotationSpeed * Time.deltaTime);
        IsFacingTarget = false;
    }

    private void RotateToFaceTarget()
    {
        var originalRotation = _turret.transform.rotation;
        _turret.transform.LookAt(_target);
        var rotation = Quaternion.RotateTowards(originalRotation, _turret.transform.rotation, _rotationSpeed * Time.deltaTime);

        rotation = Quaternion.Euler(new Vector3(0, rotation.eulerAngles.y, 0));
        _turret.transform.rotation = rotation;

        var targetDirection = _target.position - _turret.transform.position;
        var angleToTarget = Vector2.Angle(
            new Vector2(_turret.transform.forward.x, _turret.transform.forward.z),
            new Vector2(targetDirection.x, targetDirection.z));
        IsFacingTarget = angleToTarget < 10;
    }

    private void OnDrawGizmos()
    {
        if (IsFacingTarget && _turret != null && _target != null)
        {
            Gizmos.color = Color.green;
            Gizmos.DrawLine(_turret.transform.position, _target.position);
        }
    }
}
