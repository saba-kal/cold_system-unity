using UnityEngine;

public class WeaponMount : MonoBehaviour
{
    [SerializeField] private float _maxVerticalGimbal = 50f;
    [SerializeField] private float _maxHorizontalGimbal = 5f;

    private BaseWeapon _weapon;
    private Unit _lookTarget;
    private Quaternion _baseRotation;

    private void Start()
    {
        _weapon = GetComponentInChildren<BaseWeapon>();
        if (_weapon == null)
        {
            Debug.LogWarning("Mech has an empty weapon mount.");
        }
        _baseRotation = transform.localRotation;
    }

    private void Update()
    {
        if (_weapon == null)
        {
            return;
        }

        if (_lookTarget != null)
        {
            RotateToFaceTarget();
        }
        else
        {
            _weapon.transform.localRotation = _baseRotation;
        }
    }

    public void SetLookTarget(Unit target)
    {
        _lookTarget = target;
    }

    private void RotateToFaceTarget()
    {
        _weapon.transform.LookAt(_lookTarget.GetFieldOfViewStartPosition());

        var eulerAngles = _weapon.transform.localEulerAngles;
        eulerAngles.x = Utils.ClampAngle(eulerAngles.x, -_maxVerticalGimbal, _maxVerticalGimbal);
        eulerAngles.y = Utils.ClampAngle(eulerAngles.y, -_maxHorizontalGimbal, _maxHorizontalGimbal);
        eulerAngles.z = 0f;

        _weapon.transform.localEulerAngles = eulerAngles;
    }

    private void OnDrawGizmos()
    {
        var bottomLeftDirection = new Vector3(
            -Mathf.Sin(_maxHorizontalGimbal * Mathf.Deg2Rad),
            -Mathf.Tan(_maxVerticalGimbal * Mathf.Deg2Rad),
            Mathf.Cos(_maxHorizontalGimbal * Mathf.Deg2Rad)).normalized;
        var bottomRightDirection = new Vector3(
            Mathf.Sin(_maxHorizontalGimbal * Mathf.Deg2Rad),
            -Mathf.Tan(_maxVerticalGimbal * Mathf.Deg2Rad),
            Mathf.Cos(_maxHorizontalGimbal * Mathf.Deg2Rad)).normalized;
        var topLeftDirection = new Vector3(
            -Mathf.Sin(_maxHorizontalGimbal * Mathf.Deg2Rad),
            Mathf.Tan(_maxVerticalGimbal * Mathf.Deg2Rad),
            Mathf.Cos(_maxHorizontalGimbal * Mathf.Deg2Rad)).normalized;
        var topRightDirection = new Vector3(
            Mathf.Sin(_maxHorizontalGimbal * Mathf.Deg2Rad),
            Mathf.Tan(_maxVerticalGimbal * Mathf.Deg2Rad),
            Mathf.Cos(_maxHorizontalGimbal * Mathf.Deg2Rad)).normalized;

        Gizmos.color = Color.red;
        Gizmos.DrawRay(transform.position, transform.TransformDirection(bottomRightDirection));
        Gizmos.DrawRay(transform.position, transform.TransformDirection(bottomLeftDirection));
        Gizmos.DrawRay(transform.position, transform.TransformDirection(topLeftDirection));
        Gizmos.DrawRay(transform.position, transform.TransformDirection(topRightDirection));
    }
}
