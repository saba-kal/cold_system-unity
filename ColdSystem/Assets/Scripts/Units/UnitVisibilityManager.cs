using UnityEngine;

public class UnitVisibilityManager
{
    private readonly Unit[] _friendlyUnits;
    private readonly Unit[] _enemyUnits;

    public UnitVisibilityManager(Unit[] friendlyUnits, Unit[] enemyUnits)
    {
        _friendlyUnits = friendlyUnits;
        _enemyUnits = enemyUnits;
    }

    public void UpdateVisibileUnits()
    {

        foreach (var enemyUnit in _enemyUnits)
        {
            if (enemyUnit == null) continue; //Unit is dead.
            enemyUnit.SetVisible(false);
        }

        foreach (var frendlyUnit in _friendlyUnits)
        {
            if (frendlyUnit == null) continue; //Unit is dead.

            foreach (var enemyUnit in _enemyUnits)
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

    private bool UnitIsWithinFieldOfView(Unit unit, Unit opposingUnit)
    {
        return false;
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
