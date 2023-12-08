using UnityEngine;


[RequireComponent(typeof(UnitMovement))]
public class Unit : MonoBehaviour
{
    public delegate void UnitDestroyed(Unit unit);
    public static event UnitDestroyed OnUnitDestroyed;

    public delegate void VisibilityChanged(Unit unit, bool isVisible);
    public static event VisibilityChanged OnUnitVisibilityChanged;

    public delegate void WeaponFired(Unit unit);
    public static event WeaponFired OnWeaponFired;

    public bool Selected { get; set; }
    public GameObject SelectedIndicator { get; set; }
    public UnitType Type { get; private set; }
    public bool IsVisible => _isVisible;
    public bool AutoAttackEnabled => _unitAutoAttack?.Enabled ?? false;

    private UnitMovement _unitMovement;
    private UnitAutoAttack _unitAutoAttack;
    private UnitVisibility _unitVisiblity;
    private UnitAbility _ability;
    private UnitTurret _turret;
    private UnitFieldOfView _fieldOfView;
    private EnemyUnitHud _enemyUnitHud;
    private Health _health;
    private bool _isVisible = true;

    private void Awake()
    {
        _unitMovement = GetComponent<UnitMovement>();
        _unitVisiblity = GetComponent<UnitVisibility>();
        _ability = GetComponent<UnitAbility>();
        _turret = GetComponent<UnitTurret>();
        _unitAutoAttack = GetComponent<UnitAutoAttack>();
        _fieldOfView = GetComponent<UnitFieldOfView>();
        _enemyUnitHud = GetComponentInChildren<EnemyUnitHud>();
        _health = GetComponent<Health>();
        if (_health != null)
        {
            _health.OnHealthLost += OnHealthLost;
        }
        if (_unitAutoAttack != null)
        {
            _unitAutoAttack.OnWeaponFired += OnAutoAttackWeaponFired;
        }
        ValidateUnitComponents();
    }

    public void Initialize(UnitType type)
    {
        gameObject.layer = UnitFunctions.GetUnitLayer(type);
        Type = type;
        _unitAutoAttack?.Initialize(type, _fieldOfView.GetFieldOfView()?.ViewDistance ?? 25f);
        _enemyUnitHud?.Initialize(this);
    }

    public void SetDestination(Vector3 destination)
    {
        _unitMovement.SetDestination(destination);
        _unitMovement.SetTargetUnit(null);
    }

    public void SetTargetUnit(Unit unit)
    {
        _unitMovement.SetTargetUnit(unit);
        _unitAutoAttack?.SetTargetUnit(unit);
    }

    public bool IsAttacking()
    {
        return _unitAutoAttack?.IsAttacking ?? false;
    }

    public void SetHealthBarActive(bool isActive)
    {
        GetComponentInChildren<HealthBar>()?.gameObject.SetActive(isActive);
    }

    public Vector3 GetFieldOfViewStartPosition()
    {
        return _fieldOfView?.GetFieldOfViewStartPosition() ?? transform.position;
    }

    public Vector3 GetFaceDirectionEulerAngles()
    {
        return _turret?.GetFaceDirectionEulerAngles() ?? transform.eulerAngles;
    }

    public Vector3 GetFaceDirection()
    {
        return _turret?.GetFaceDirection() ?? transform.forward;
    }

    public void SetVisible(bool visible)
    {
        if (_unitVisiblity == null)
        {
            _unitVisiblity = GetComponent<UnitVisibility>();
        }
        _unitVisiblity?.SetVisible(visible);
        if (_isVisible != visible)
        {
            OnUnitVisibilityChanged?.Invoke(this, visible);
        }
        _isVisible = visible;
    }

    public bool IsTimeSinceWeaponFireBelowRevealThreshold()
    {
        return _unitVisiblity?.IsTimeSinceWeaponFireBelowRevealThreshold() ?? false;
    }

    public void ActivateAbility()
    {
        _ability?.Activate();
    }

    public bool TargetIsInsideFieldOfView(Unit target)
    {
        return _fieldOfView?.TargetIsInsideFieldOfView(target) ?? false;
    }

    public FieldOfViewValues GetFieldOfView()
    {
        return _fieldOfView?.GetFieldOfView() ?? new FieldOfViewValues();
    }

    public void ToggleAutoAttack()
    {
        _unitAutoAttack?.ToggleEnabled();
    }

    private void OnHealthLost()
    {
        OnUnitDestroyed?.Invoke(this);
        Destroy(gameObject);
    }

    private void OnAutoAttackWeaponFired()
    {
        OnWeaponFired?.Invoke(this);
    }

    private void ValidateUnitComponents()
    {
        if (_unitMovement == null)
        {
            Debug.LogError($"Unit {gameObject.name} is missing a movement component");
        }
        if (_unitAutoAttack == null)
        {
            Debug.LogError($"Unit {gameObject.name} is missing an auto-attack component");
        }
        if (_fieldOfView == null)
        {
            Debug.LogError($"Unit {gameObject.name} is missing a field of view component");
        }
        if (_health == null)
        {
            Debug.LogError($"Unit {gameObject.name} is missing a health component");
        }
        if (_unitVisiblity == null)
        {
            Debug.LogError($"Unit {gameObject.name} is missing a visiblity component");
        }
    }
}
