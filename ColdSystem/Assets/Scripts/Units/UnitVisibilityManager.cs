using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class UnitVisibilityManager : MonoBehaviour
{
    [SerializeField] private UnitType _type;

    private Dictionary<RevealedArea, float> _revealedArea2Durations = new Dictionary<RevealedArea, float>();

    private void OnEnable()
    {
        AreaRevealAbility.OnAreaRevealed += OnAreaRevealed;
    }

    private void OnDisable()
    {
        AreaRevealAbility.OnAreaRevealed -= OnAreaRevealed;
    }

    private void Update()
    {
        SetAllEnemyUnitsInvisible();
        SetEnemyUnitsInRevealedAreasVisible();
        SetEnemyUnitsInLineOfSightVisible();
        UpdateRevealedAreas();
    }

    public List<RevealedArea> GetRevealedAreas()
    {
        return _revealedArea2Durations.Keys.ToList();
    }

    private void SetAllEnemyUnitsInvisible()
    {
        foreach (var enemyUnit in UnitFunctions.GetUnitsEnemyTo(_type))
        {
            if (enemyUnit == null) continue; //Unit is dead.
            enemyUnit.SetVisible(false);
        }
    }

    private void SetEnemyUnitsInRevealedAreasVisible()
    {
        foreach (var enemyUnit in UnitFunctions.GetUnitsEnemyTo(_type))
        {
            if (enemyUnit == null || enemyUnit.IsVisible) continue; //Unit is dead or already visible.

            foreach (var revealedArea in _revealedArea2Durations.Keys)
            {
                if ((revealedArea.Position - enemyUnit.GetFieldOfViewStartPosition()).sqrMagnitude <= revealedArea.Radius * revealedArea.Radius)
                {
                    enemyUnit.SetVisible(true);
                }
            }
        }
    }

    private void SetEnemyUnitsInLineOfSightVisible()
    {
        foreach (var frendlyUnit in UnitFunctions.GetUnitsFriendlyTo(_type))
        {
            if (frendlyUnit == null) continue; //Unit is dead.

            foreach (var enemyUnit in UnitFunctions.GetUnitsEnemyTo(_type))
            {
                if (enemyUnit == null || enemyUnit.IsVisible) continue; //Unit is dead or already visible.

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

    private void UpdateRevealedAreas()
    {
        var areasToRemove = new List<RevealedArea>();
        var revealedAreas = _revealedArea2Durations.Keys.ToList();
        foreach (var revealedArea in revealedAreas)
        {
            _revealedArea2Durations[revealedArea] -= Time.deltaTime;
            if (_revealedArea2Durations[revealedArea] <= 0)
            {
                areasToRemove.Add(revealedArea);
            }
        }

        foreach (var area in areasToRemove)
        {
            _revealedArea2Durations.Remove(area);
        }
    }

    private void OnAreaRevealed(RevealedArea revealedArea)
    {
        _revealedArea2Durations.TryAdd(revealedArea, revealedArea.Duration);
    }
}
