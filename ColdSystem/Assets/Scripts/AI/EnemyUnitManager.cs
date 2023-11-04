using Unity.VisualScripting;
using UnityEngine;


public class EnemyUnitManager : MonoBehaviour
{
    public static EnemyUnitManager Instance { get; private set; }

    private Unit[] _enemyUnits;
    private UnitVisibilityManager _visibilityManager;

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

        _enemyUnits = GetComponentsInChildren<Unit>();
    }

    private void Start()
    {
        _visibilityManager = new UnitVisibilityManager(_enemyUnits, PlayerUnitManager.Instance?.GetUnits() ?? new Unit[0]);
        foreach (var unit in _enemyUnits)
        {
            var unitVisibility = unit.AddComponent<UnitVisibility>();
            unitVisibility.SetVisible(false);
            unit.Initialize(UnitType.Enemy);
        }
    }

    private void Update()
    {
        _visibilityManager.UpdateVisibileUnits();
    }

    public Unit[] GetUnits()
    {
        return _enemyUnits;
    }
}