using UnityEngine;

public static class UnitFunctions
{
    public static int GetUnitLayer(UnitType type)
    {
        switch (type)
        {
            case UnitType.Player:
                return LayerMask.NameToLayer(Constants.PLAYER_UNIT_LAYER);
            case UnitType.Enemy:
                return LayerMask.NameToLayer(Constants.ENEMY_UNIT_LAYER);
        }
        return 0;
    }

    public static string GetUnitLayerName(UnitType type)
    {
        switch (type)
        {
            case UnitType.Player:
                return Constants.PLAYER_UNIT_LAYER;
            case UnitType.Enemy:
                return Constants.ENEMY_UNIT_LAYER;
        }
        return string.Empty;
    }

    public static int GetProjectileLayer(UnitType type)
    {
        switch (type)
        {
            case UnitType.Player:
                return LayerMask.NameToLayer(Constants.PLAYER_PROJECTILE_LAYER);
            case UnitType.Enemy:
                return LayerMask.NameToLayer(Constants.ENEMY_PROJECTILE_LAYER);
        }
        return 0;
    }

    public static int GetUnitLayerMask(UnitType type)
    {
        switch (type)
        {
            case UnitType.Player:
                return LayerMask.GetMask(Constants.PLAYER_UNIT_LAYER);
            case UnitType.Enemy:
                return LayerMask.GetMask(Constants.ENEMY_UNIT_LAYER);
        }
        return 0;
    }
}
