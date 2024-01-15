using System;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;

public class MechLoadoutMenu : MonoBehaviour
{
    [SerializeField] private TMP_Dropdown _bodyTypeDropdown;
    [SerializeField] private TMP_Dropdown _leftWeaponDropdown;
    [SerializeField] private TMP_Dropdown _rightWeaponDropdown;

    private List<Unit> _units = new List<Unit>();
    private List<BaseWeapon> _weapons = new List<BaseWeapon>();

    public void Initialize(
        List<Unit> units,
        List<BaseWeapon> weapons,
        Action onChange)
    {
        _bodyTypeDropdown.ClearOptions();
        _leftWeaponDropdown.ClearOptions();
        _rightWeaponDropdown.ClearOptions();

        _units = units;
        _weapons = weapons;

        foreach (var unit in _units)
        {
            _bodyTypeDropdown.options.Add(new TMP_Dropdown.OptionData(unit.name));
        }
        foreach (var weapon in _weapons)
        {
            _leftWeaponDropdown.options.Add(new TMP_Dropdown.OptionData(weapon.name));
            _rightWeaponDropdown.options.Add(new TMP_Dropdown.OptionData(weapon.name));
        }

        _bodyTypeDropdown.onValueChanged.AddListener(_ => onChange());
        _leftWeaponDropdown.onValueChanged.AddListener(_ => onChange());
        _rightWeaponDropdown.onValueChanged.AddListener(_ => onChange());
    }

    public void UpdateSelectionsFromUnitData(PlayerUnitData playerUnitData)
    {
        var bodyTypeIndex = _bodyTypeDropdown.options.FindIndex(option => option.text == playerUnitData.UnitName);
        if (bodyTypeIndex != -1)
        {
            _bodyTypeDropdown.SetValueWithoutNotify(bodyTypeIndex);
            _bodyTypeDropdown.RefreshShownValue();
        }

        var leftWeaponIndex = _leftWeaponDropdown.options.FindIndex(option => option.text == playerUnitData.LeftWeaponName);
        if (leftWeaponIndex != -1)
        {
            _leftWeaponDropdown.SetValueWithoutNotify(leftWeaponIndex);
            _leftWeaponDropdown.RefreshShownValue();
        }

        var rightWeaponIndex = _rightWeaponDropdown.options.FindIndex(option => option.text == playerUnitData.RightWeaponName);
        if (rightWeaponIndex != -1)
        {
            _rightWeaponDropdown.SetValueWithoutNotify(rightWeaponIndex);
            _rightWeaponDropdown.RefreshShownValue();
        }
    }

    public PlayerUnitLoadout GetLoadout()
    {
        return new PlayerUnitLoadout
        {
            Unit = _units.FirstOrDefault(u => u.name == _bodyTypeDropdown.options[_bodyTypeDropdown.value].text),
            LeftWeapon = _weapons.FirstOrDefault(u => u.name == _leftWeaponDropdown.options[_leftWeaponDropdown.value].text),
            RightWeapon = _weapons.FirstOrDefault(u => u.name == _rightWeaponDropdown.options[_rightWeaponDropdown.value].text),
        };
    }
}