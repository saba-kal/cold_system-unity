using UnityEngine;

public class UnitTurret : MonoBehaviour
{
    public bool IsFacingTarget { get; private set; }

    [SerializeField] private GameObject _turret;
    [SerializeField] private float _rotationSpeed = 25f;

    private Transform _target = null;

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

    private void ResetRotation()
    {
        _turret.transform.localRotation = Quaternion.RotateTowards(
            _turret.transform.localRotation,
            Quaternion.Euler(0, 0, 0),
            _rotationSpeed * Time.deltaTime);
        IsFacingTarget = false;
    }

    private void RotateToFaceTarget()
    {
        var originalRotation = _turret.transform.localRotation;
        _turret.transform.LookAt(_target);
        var rotation = Quaternion.RotateTowards(originalRotation, _turret.transform.localRotation, _rotationSpeed * Time.deltaTime);

        rotation = Quaternion.Euler(new Vector3(0, rotation.eulerAngles.y, 0));
        _turret.transform.localRotation = rotation;

        var targetDirection = _target.position - _turret.transform.position;
        var angleToTarget = Vector2.Angle(
            new Vector2(_turret.transform.forward.x, _turret.transform.forward.z),
            new Vector2(targetDirection.x, targetDirection.z));
        if (GetComponent<Unit>().Type == UnitType.Player)
        {
            Debug.Log(angleToTarget);
        }
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
