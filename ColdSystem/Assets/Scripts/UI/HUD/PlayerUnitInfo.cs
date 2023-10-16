using TMPro;
using UnityEngine;

public class PlayerUnitInfo : MonoBehaviour
{
    [SerializeField] private HealthBar _healthBar;
    [SerializeField] private TextMeshProUGUI _unitNumberText;
    [SerializeField] private GameObject _unitAttackModeIcon;
    [SerializeField] private GameObject _unitMoveModeIcon;
    [SerializeField] private GameObject _deadStateIcon;

    private Unit _unit;

    private void Update()
    {
        ToggleUnitModeIcons();
    }

    public void SetUnit(Unit unit, int number)
    {
        _unit = unit;
        _healthBar.SetHealth(unit.GetComponent<Health>());
        _unitNumberText.text = number.ToString();
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
}
