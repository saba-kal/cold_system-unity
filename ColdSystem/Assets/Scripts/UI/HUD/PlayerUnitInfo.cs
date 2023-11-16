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

    private Unit _unit;
    private UnitAbility _unitAbility;

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
            _deadStateIcon.SetActive(false);
        }
        else
        {
            _unitAttackModeIcon.SetActive(false);
            _unitMoveModeIcon.SetActive(false);
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
