using System.Collections.Generic;
using UnityEngine;

public class LoadoutMenu : MonoBehaviour
{
    public static List<PlayerUnitLoadout> PlayerLoadout;

    [SerializeField] private List<Unit> _avialableUnits;
    [SerializeField] private List<BaseWeapon> _avialableWeapons;

    private MechLoadoutMenu[] _loadouts;
    private PlayerData _playerData;

    private void Start()
    {
        _loadouts = GetComponentsInChildren<MechLoadoutMenu>();
        _playerData = PlayerData.Load();

        for (var i = 0; i < _loadouts.Length; i++)
        {
            _loadouts[i].Initialize(_avialableUnits, _avialableWeapons, OnUnitLoadoutChange);
            if (_playerData != null && _playerData.Units != null && i < _playerData.Units.Count)
            {
                _loadouts[i].UpdateSelectionsFromUnitData(_playerData.Units[i]);
            }
        }
    }

    private void OnDisable()
    {
        PlayerLoadout = new List<PlayerUnitLoadout>();
        foreach (var loadout in _loadouts)
        {
            PlayerLoadout.Add(loadout.GetLoadout());
        }
    }

    private void OnUnitLoadoutChange()
    {
        if (_playerData == null)
        {
            _playerData = new PlayerData();
        }
        _playerData.Units = new List<PlayerUnitData>();

        foreach (var loadout in _loadouts)
        {
            var unitLoadout = loadout.GetLoadout();
            _playerData.Units.Add(new PlayerUnitData
            {
                UnitName = unitLoadout.Unit.name,
                LeftWeaponName = unitLoadout.LeftWeapon.name,
                RightWeaponName = unitLoadout.RightWeapon.name,
            });
        }

        _playerData.Save();
    }
}
