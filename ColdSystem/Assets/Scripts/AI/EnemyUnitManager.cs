using System.Collections.Generic;
using System.Linq;
using Unity.VisualScripting;
using UnityEngine;


public class EnemyUnitManager : MonoBehaviour
{
    public static EnemyUnitManager Instance { get; private set; }

    [SerializeField] private bool _addRandomMovementToUnits = false;

    private List<Unit> _enemyUnits;

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(this);
        }
        else
        {
            Instance = this;
        }

        _enemyUnits = GetComponentsInChildren<Unit>().ToList();
    }

    private void Start()
    {
        foreach (var unit in _enemyUnits)
        {
            SetUpUnit(unit);
        }
    }

    public List<Unit> GetUnits()
    {
        return _enemyUnits;
    }

    public void AddUnit(Unit unit)
    {
        SetUpUnit(unit);
        unit.transform.SetParent(transform, true);
        _enemyUnits.Add(unit);
    }

    private void SetUpUnit(Unit unit)
    {
        unit.Initialize(UnitType.Enemy);
        if (_addRandomMovementToUnits)
        {
            unit.AddComponent<MoveToRandomPointsOfInterest>();
        }
    }
}