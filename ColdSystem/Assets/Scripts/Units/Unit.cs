using UnityEngine;


[RequireComponent(typeof(UnitMovement))]
public class Unit : MonoBehaviour
{
    public delegate void UnitDestroyed(Unit unit);
    public static event UnitDestroyed OnUnitDestroyed;

    public bool Selected { get; set; }
    public GameObject SelectedIndicator { get; set; }
    public UnitType Type { get; private set; }
    public float FieldOfViewDistance => _fieldOfViewDistance;
    public float FieldOfViewAngle => _fieldOfViewAngle;
    public float FieldOfViewDepressionAngle => _fieldOfViewDepressionAngle;
    public float FieldOfViewElevationAngle => _fieldOfViewElevationAngle;
    public bool IsVisible => _isVisible;

    [SerializeField] private Vector3 _fieldOfViewPosition;
    [SerializeField][Range(5f, 50f)] private float _fieldOfViewDistance = 20f;
    [SerializeField][Range(10f, 360f)] private float _fieldOfViewAngle = 120f;
    [SerializeField][Range(0f, 180f)] private float _fieldOfViewDepressionAngle = 60f;
    [SerializeField][Range(0f, 180f)] private float _fieldOfViewElevationAngle = 30f;
    [SerializeField] private bool _showFieldOfViewGizmo = false;

    private UnitMovement _unitMovement;
    private UnitAutoAttack _unitAutoAttack;
    private UnitVisibility _unitVisiblity;
    private UnitAbility _ability;
    private bool _isVisible = true;

    private void Awake()
    {
        _unitMovement = GetComponent<UnitMovement>();
        _unitAutoAttack = GetComponent<UnitAutoAttack>();
        _unitVisiblity = GetComponent<UnitVisibility>();
        _ability = GetComponent<UnitAbility>();
        var health = GetComponent<Health>();
        if (health != null)
        {
            health.OnHealthLost += OnHealthLost;
        }
    }

    public void Initialize(UnitType type)
    {
        gameObject.layer = UnitFunctions.GetUnitLayer(type);
        Type = type;
        _unitAutoAttack?.Initialize(type, _fieldOfViewDistance);
    }

    public void SetDestination(Vector3 destination)
    {
        _unitMovement.SetDestination(destination);
    }

    public bool IsAttacking()
    {
        return _unitAutoAttack?.IsAttacking() ?? false;
    }

    public void SetHealthBarActive(bool isActive)
    {
        GetComponentInChildren<HealthBar>()?.gameObject.SetActive(isActive);
    }

    public float GetRadius()
    {
        return _fieldOfViewDistance;
    }

    public Vector3 GetFieldOfViewStartPosition()
    {
        return transform.TransformPoint(_fieldOfViewPosition);
    }

    public Vector3 GetFaceDirectionEulerAngles()
    {
        if (_unitAutoAttack != null)
        {
            return _unitAutoAttack.GetFaceDirectionEulerAngles();
        }
        return transform.eulerAngles;
    }

    public void SetVisible(bool visible)
    {
        _isVisible = visible;
        if (_unitVisiblity == null)
        {
            _unitVisiblity = GetComponent<UnitVisibility>();
        }
        _unitVisiblity?.SetVisible(visible);
    }

    public void ActivateAbility()
    {
        _ability?.Activate();
    }

    private void OnHealthLost()
    {
        OnUnitDestroyed?.Invoke(this);
        Destroy(gameObject);
    }

    private void OnDrawGizmos()
    {
        if (!_showFieldOfViewGizmo)
        {
            return;
        }

        Gizmos.color = Color.blue;
        var bottomLeftDirection = new Vector3(
            Mathf.Sin((transform.eulerAngles.y - _fieldOfViewAngle / 2f) * Mathf.Deg2Rad),
            Mathf.Tan((transform.eulerAngles.x - _fieldOfViewDepressionAngle) * Mathf.Deg2Rad),
            Mathf.Cos((transform.eulerAngles.y - _fieldOfViewAngle / 2f) * Mathf.Deg2Rad)).normalized;
        var topLeftDirection = new Vector3(
            Mathf.Sin((transform.eulerAngles.y - _fieldOfViewAngle / 2f) * Mathf.Deg2Rad),
            Mathf.Tan((transform.eulerAngles.x + _fieldOfViewElevationAngle) * Mathf.Deg2Rad),
            Mathf.Cos((transform.eulerAngles.y - _fieldOfViewAngle / 2f) * Mathf.Deg2Rad)).normalized;
        var topRightDirection = new Vector3(
            Mathf.Sin((transform.eulerAngles.y + _fieldOfViewAngle / 2f) * Mathf.Deg2Rad),
            Mathf.Tan((transform.eulerAngles.x + _fieldOfViewElevationAngle) * Mathf.Deg2Rad),
            Mathf.Cos((transform.eulerAngles.y + _fieldOfViewAngle / 2f) * Mathf.Deg2Rad)).normalized;
        var bottomRightDirection = new Vector3(
            Mathf.Sin((transform.eulerAngles.y + _fieldOfViewAngle / 2f) * Mathf.Deg2Rad),
            Mathf.Tan((transform.eulerAngles.x - _fieldOfViewDepressionAngle) * Mathf.Deg2Rad),
            Mathf.Cos((transform.eulerAngles.y + _fieldOfViewAngle / 2f) * Mathf.Deg2Rad)).normalized;
        Gizmos.DrawLine(GetFieldOfViewStartPosition(), GetFieldOfViewStartPosition() + bottomLeftDirection * _fieldOfViewDistance);
        Gizmos.DrawLine(GetFieldOfViewStartPosition(), GetFieldOfViewStartPosition() + topLeftDirection * _fieldOfViewDistance);
        Gizmos.DrawLine(GetFieldOfViewStartPosition(), GetFieldOfViewStartPosition() + topRightDirection * _fieldOfViewDistance);
        Gizmos.DrawLine(GetFieldOfViewStartPosition(), GetFieldOfViewStartPosition() + bottomRightDirection * _fieldOfViewDistance);

        Gizmos.color = new Color(0, 1, 0, 0.2f);
        Gizmos.DrawSphere(GetFieldOfViewStartPosition(), _fieldOfViewDistance);
    }
}
