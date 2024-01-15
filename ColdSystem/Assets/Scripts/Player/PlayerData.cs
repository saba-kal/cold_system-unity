using System;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class PlayerData
{
    public List<PlayerUnitData> Units;

    public static PlayerData Load()
    {
        try
        {
            var playerDataJson = PlayerPrefs.GetString("PlayerData");
            return JsonUtility.FromJson<PlayerData>(playerDataJson);
        }
        catch
        {
            Debug.LogWarning("Unable to load player data. Either it does not exist yet, or the JSON parsing failed.");
            return null;
        }
    }

    public void Save()
    {
        PlayerPrefs.SetString("PlayerData", JsonUtility.ToJson(this));
    }
}

[Serializable]
public class PlayerUnitData
{
    public string UnitName;
    public string LeftWeaponName;
    public string RightWeaponName;
}
