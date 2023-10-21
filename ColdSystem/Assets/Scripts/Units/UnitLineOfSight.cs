using UnityEngine;

public class UnitLineOfSight
{
    private readonly float _viewRadius = 20f;
    private readonly Unit _self;
    private readonly Unit[] _opposingUnits;

    public UnitLineOfSight(Unit self, Unit[] opposingUnits, float viewRadius)
    {
        _self = self;
        _opposingUnits = opposingUnits;
        _viewRadius = viewRadius;
    }

    public Unit GetNearestVisibleUnit()
    {
        var minimumDistanceSqr = float.MaxValue;
        var maxDistanceSqr = _viewRadius * _viewRadius;
        Unit nearestUnit = null;
        foreach (var unit in _opposingUnits)
        {
            if (unit == null)
            {
                //Unit is dead.
                continue;
            }

            var distanceSqr = (unit.transform.position - _self.transform.position).sqrMagnitude;
            if (distanceSqr < maxDistanceSqr && distanceSqr < minimumDistanceSqr && UnitIsInLineOfSight(unit))
            {
                minimumDistanceSqr = distanceSqr;
                nearestUnit = unit;
            }
        }

        return nearestUnit;
    }

    private bool UnitIsInLineOfSight(Unit unit)
    {
        var layerMask = ~UnitFunctions.GetUnitLayerMask(unit.Type);
        var fromPosition = _self.transform.position + new Vector3(0, 2, 0);
        var toPosition = (unit.transform.position - _self.transform.position) + new Vector3(0, 2, 0);
        return !Physics.Raycast(fromPosition, toPosition, _viewRadius, layerMask);
    }
}
