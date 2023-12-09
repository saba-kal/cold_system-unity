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
        foreach (var friendlyUnit in UnitFunctions.GetUnitsFriendlyTo(_type))
        {
            if (friendlyUnit == null) continue; //Unit is dead.

            foreach (var enemyUnit in UnitFunctions.GetUnitsEnemyTo(_type))
            {
                if (enemyUnit == null || enemyUnit.IsVisible) continue; //Unit is dead or already visible.

                if (UnitWeaponFireHeardByOpposingUnit(enemyUnit, friendlyUnit))
                {
                    enemyUnit.SetVisible(true);
                }
                else
                {
                    var enemyIsVisible = friendlyUnit.TargetIsInsideFieldOfView(enemyUnit);
                    enemyUnit.SetVisible(enemyIsVisible);
                }
            }
        }
    }

    private bool UnitWeaponFireHeardByOpposingUnit(Unit unit, Unit opposingUnit)
    {
        var fieldOfViewRange = opposingUnit.GetFieldOfView().ViewDistance;
        return unit.IsTimeSinceWeaponFireBelowRevealThreshold() &&
            (unit.transform.position - opposingUnit.transform.position).sqrMagnitude <= fieldOfViewRange * fieldOfViewRange;
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
