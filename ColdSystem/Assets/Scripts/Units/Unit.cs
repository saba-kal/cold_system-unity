using UnityEngine;


[RequireComponent(typeof(UnitMovement))]
public class Unit : MonoBehaviour
{
    public delegate void UnitDestroyed(Unit unit);
    public event UnitDestroyed OnUnitDestroyed;
    public bool Selected { get; set; }
    public GameObject SelectedIndicator { get; set; }
    public UnitType Type { get; private set; }
    public float FieldOfViewAngle => _fieldOfViewAngle;
    public float FieldOfViewDepressionAngle => _fieldOfViewDepressionAngle;
    public float FieldOfViewElevationAngle => _fieldOfViewElevationAngle;

    [SerializeField][Range(10f, 360f)] private float _fieldOfViewAngle = 120f;
    [SerializeField][Range(0f, 180f)] private float _fieldOfViewDepressionAngle = 60f;
    [SerializeField][Range(0f, 180f)] private float _fieldOfViewElevationAngle = 30f;

    private UnitMovement _unitMovement;
    private UnitAutoAttack _unitAutoAttack;

    private void Awake()
    {
        _unitMovement = GetComponent<UnitMovement>();
        _unitAutoAttack = GetComponent<UnitAutoAttack>();
        var health = GetComponent<Health>();
        if (health != null)
        {
            health.OnHealthLost += OnHealthLost;
        }
    }

    public void Initialize(Unit[] opposingUnits, UnitType type)
    {
        gameObject.layer = UnitFunctions.GetUnitLayer(type);
        Type = type;
        _unitAutoAttack?.Initialize(opposingUnits, type);
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
        return _unitAutoAttack?.GetRange() ?? 0f;
    }

    private void OnHealthLost()
    {
        OnUnitDestroyed?.Invoke(this);
        Destroy(gameObject);
    }
}
