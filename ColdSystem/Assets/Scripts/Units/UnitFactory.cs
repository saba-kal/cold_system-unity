using System.Collections.Generic;
using UnityEngine;

public static class UnitFactory
{
    public static Unit Create(
        Unit unitPrefab,
        List<BaseWeapon> weaponPrefabs,
        Transform parent,
        Vector3 worldPosition)
    {
        var unit = Object.Instantiate(unitPrefab, parent);
        unit.transform.position = worldPosition;

        var weaponMounts = unit.GetComponentsInChildren<WeaponMount>();

        if (weaponMounts.Length != weaponPrefabs.Count)
        {
            Debug.LogWarning("The number of weapon mounts does not match the number of selected weapons for unit " + unitPrefab.name);
        }

        for (var i = 0; i < weaponMounts.Length; i++)
        {
            var existingWeapon = weaponMounts[i].GetComponentInChildren<BaseWeapon>();
            if (existingWeapon != null && i < weaponPrefabs.Count)
            {
                Object.DestroyImmediate(existingWeapon.gameObject);
            }

            if (i < weaponPrefabs.Count)
            {
                var weapon = Object.Instantiate(weaponPrefabs[i], weaponMounts[i].transform);
                weapon.transform.localPosition = Vector3.zero;
            }
        }

        return unit;
    }
}
