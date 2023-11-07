using UnityEngine;

public class UnitVisibilityManager
{
    private readonly UnitType _type;

    public UnitVisibilityManager(UnitType type)
    {
        _type = type;
    }

    public void UpdateVisibileUnits()
    {

        foreach (var enemyUnit in UnitFunctions.GetUnitsEnemyTo(_type))
        {
            if (enemyUnit == null) continue; //Unit is dead.
            enemyUnit.SetVisible(false);
        }

        foreach (var frendlyUnit in UnitFunctions.GetUnitsFriendlyTo(_type))
        {
            if (frendlyUnit == null) continue; //Unit is dead.

            foreach (var enemyUnit in UnitFunctions.GetUnitsEnemyTo(_type))
            {
                if (enemyUnit == null) continue; //Unit is dead.

                var distanceSqr = (frendlyUnit.transform.position - enemyUnit.transform.position).sqrMagnitude;
                var maxDistanceSqr = frendlyUnit.FieldOfViewDistance * frendlyUnit.FieldOfViewDistance;
                if (distanceSqr < maxDistanceSqr && UnitHasLineOfSight(frendlyUnit, enemyUnit))
                {
                    enemyUnit.SetVisible(true);
                }
            }
        }
    }

    private bool UnitHasLineOfSight(Unit unit, Unit opposingUnit)
    {
        var layerMask = LayerMask.GetMask(
            Constants.GROUND_LAYER,
            Constants.OBSTACLE_LAYER,
            UnitFunctions.GetUnitLayerName(opposingUnit.Type));
        var fromPosition = unit.GetFieldOfViewStartPosition();
        var toPosition = opposingUnit.GetFieldOfViewStartPosition();
        if (Physics.Raycast(fromPosition, toPosition - fromPosition, out var hitInfo, unit.FieldOfViewDistance, layerMask))
        {
            return hitInfo.collider.gameObject.layer == UnitFunctions.GetUnitLayer(opposingUnit.Type);
        }
        return false;
    }
}
