using UnityEngine;

[RequireComponent(typeof(Unit))]
public class UnitFieldOfView : MonoBehaviour
{
    [SerializeField] private Vector3 _fieldOfViewPosition;
    [SerializeField][Range(5f, 50f)] private float _viewDistance = 20f;
    [SerializeField][Range(10f, 360f)] private float _angle = 120f;
    [SerializeField][Range(0f, 180f)] private float _depressionAngle = 60f;
    [SerializeField][Range(0f, 180f)] private float _elevationAngle = 30f;
    [SerializeField] private bool _showFieldOfViewGizmo = false;

    private Unit _unit;

    private void Awake()
    {
        _unit = GetComponent<Unit>();
    }

    public bool TargetIsInsideFieldOfView(Unit target)
    {
        var distanceSqr = (transform.position - target.transform.position).sqrMagnitude;
        var maxDistanceSqr = _viewDistance * _viewDistance;

        return distanceSqr < maxDistanceSqr &&
            TargetIsInsideFieldOfViewAngle(target) &&
            TargetIsInLineOfSight(target);
    }

    public Vector3 GetFieldOfViewStartPosition()
    {
        return transform.TransformPoint(_fieldOfViewPosition);
    }

    public FieldOfViewValues GetFieldOfView()
    {
        return new FieldOfViewValues
        {
            FieldOfViewPosition = _fieldOfViewPosition,
            ViewDistance = _viewDistance,
            Angle = _angle,
            DepressionAngle = _depressionAngle,
            ElevationAngle = _elevationAngle
        };
    }

    private bool TargetIsInsideFieldOfViewAngle(Unit target)
    {
        var directionToTarget = target.GetFieldOfViewStartPosition() - GetFieldOfViewStartPosition();
        var faceDirection = _unit.GetFaceDirection();

        var horizontalAngle = Vector3.Angle(
            new Vector3(directionToTarget.x, 0, directionToTarget.z),
            new Vector3(faceDirection.x, 0, faceDirection.z));
        if (horizontalAngle > _angle / 2f)
        {
            return false;
        }

        var verticalAngle = Vector3.Angle(
            directionToTarget,
            new Vector3(directionToTarget.x, 0, directionToTarget.z));
        var maxVerticalAngle = transform.position.y >= target.transform.position.y ? _depressionAngle : _elevationAngle;
        return verticalAngle <= maxVerticalAngle;
    }

    private bool TargetIsInLineOfSight(Unit target)
    {
        var layerMask = LayerMask.GetMask(
            Constants.GROUND_LAYER,
            Constants.OBSTACLE_LAYER,
            UnitFunctions.GetUnitLayerName(target.Type));
        var fromPosition = GetFieldOfViewStartPosition();
        var toPosition = target.GetFieldOfViewStartPosition();
        if (Physics.Raycast(fromPosition, toPosition - fromPosition, out var hitInfo, _viewDistance, layerMask))
        {
            return hitInfo.collider.gameObject.layer == UnitFunctions.GetUnitLayer(target.Type);
        }
        return false;
    }

    private void OnDrawGizmos()
    {
        if (!_showFieldOfViewGizmo)
        {
            return;
        }

        Gizmos.color = Color.blue;
        var bottomLeftDirection = new Vector3(
            Mathf.Sin((transform.eulerAngles.y - _angle / 2f) * Mathf.Deg2Rad),
            Mathf.Tan((transform.eulerAngles.x - _depressionAngle) * Mathf.Deg2Rad),
            Mathf.Cos((transform.eulerAngles.y - _angle / 2f) * Mathf.Deg2Rad)).normalized;
        var topLeftDirection = new Vector3(
            Mathf.Sin((transform.eulerAngles.y - _angle / 2f) * Mathf.Deg2Rad),
            Mathf.Tan((transform.eulerAngles.x + _elevationAngle) * Mathf.Deg2Rad),
            Mathf.Cos((transform.eulerAngles.y - _angle / 2f) * Mathf.Deg2Rad)).normalized;
        var topRightDirection = new Vector3(
            Mathf.Sin((transform.eulerAngles.y + _angle / 2f) * Mathf.Deg2Rad),
            Mathf.Tan((transform.eulerAngles.x + _elevationAngle) * Mathf.Deg2Rad),
            Mathf.Cos((transform.eulerAngles.y + _angle / 2f) * Mathf.Deg2Rad)).normalized;
        var bottomRightDirection = new Vector3(
            Mathf.Sin((transform.eulerAngles.y + _angle / 2f) * Mathf.Deg2Rad),
            Mathf.Tan((transform.eulerAngles.x - _depressionAngle) * Mathf.Deg2Rad),
            Mathf.Cos((transform.eulerAngles.y + _angle / 2f) * Mathf.Deg2Rad)).normalized;
        Gizmos.DrawLine(GetFieldOfViewStartPosition(), GetFieldOfViewStartPosition() + bottomLeftDirection * _viewDistance);
        Gizmos.DrawLine(GetFieldOfViewStartPosition(), GetFieldOfViewStartPosition() + topLeftDirection * _viewDistance);
        Gizmos.DrawLine(GetFieldOfViewStartPosition(), GetFieldOfViewStartPosition() + topRightDirection * _viewDistance);
        Gizmos.DrawLine(GetFieldOfViewStartPosition(), GetFieldOfViewStartPosition() + bottomRightDirection * _viewDistance);

        Gizmos.color = new Color(0, 1, 0, 0.2f);
        Gizmos.DrawSphere(GetFieldOfViewStartPosition(), _viewDistance);
    }
}

public class FieldOfViewValues
{
    public Vector3 FieldOfViewPosition { get; set; }
    public float ViewDistance { get; set; }
    public float Angle { get; set; }
    public float DepressionAngle { get; set; }
    public float ElevationAngle { get; set; }
}