using TMPro;
using UnityEngine;

public class PlayerUnitInfo : MonoBehaviour
{
    [SerializeField] private HealthBar _healthBar;
    [SerializeField] private HealthBar _abilityBar;
    [SerializeField] private TextMeshProUGUI _unitNumberText;
    [SerializeField] private GameObject _unitAttackModeIcon;
    [SerializeField] private GameObject _unitMoveModeIcon;
    [SerializeField] private GameObject _deadStateIcon;
    [SerializeField] private GameObject _attackDisabledIcon;
    [SerializeField] private GameObject _spottedIndicator;

    private Unit _unit;
    private UnitAbility _unitAbility;

    private void OnEnable()
    {
        Unit.OnUnitVisibilityChanged += OnUnitVisibilityChanged;
    }

    private void OnDisable()
    {
        Unit.OnUnitVisibilityChanged -= OnUnitVisibilityChanged;
    }

    private void OnUnitVisibilityChanged(Unit unit, bool isVisible)
    {
        if (unit == _unit)
        {
            _spottedIndicator.SetActive(isVisible);
        }
    }

    private void Start()
    {
        _spottedIndicator.SetActive(false);
        _attackDisabledIcon.SetActive(false);
    }

    private void Update()
    {
        ToggleUnitModeIcons();
        UpdateAbilityBar();
    }

    public void SetUnit(Unit unit, int number)
    {
        _unit = unit;
        _healthBar.SetHealth(unit.GetComponent<Health>());
        _unitNumberText.text = number.ToString();
        _unitAbility = unit.GetComponent<UnitAbility>();
        if (_unitAbility == null)
        {
            _abilityBar.SetHealth(100, 0);
        }
        ToggleUnitModeIcons();
    }

    private void ToggleUnitModeIcons()
    {
        if (_unit != null)
        {
            _unitAttackModeIcon.SetActive(_unit.IsAttacking());
            _unitMoveModeIcon.SetActive(!_unit.IsAttacking());
            _attackDisabledIcon.SetActive(!_unit.AutoAttackEnabled);
            _deadStateIcon.SetActive(false);
        }
        else
        {
            _unitAttackModeIcon.SetActive(false);
            _unitMoveModeIcon.SetActive(false);
            _attackDisabledIcon.SetActive(false);
            _deadStateIcon.SetActive(true);
        }
    }

    private void UpdateAbilityBar()
    {
        if (_unitAbility == null)
        {
            return;
        }

        _abilityBar.SetHealth(_unitAbility.CoolDown, _unitAbility.TimeSinceLastActivation);
    }
}
