using UnityEngine;


public class EnemyUnitManager : MonoBehaviour
{
    [SerializeField] private PlayerUnitManager _playerUnitManager;

    private Unit[] _enemyUnits;

    private void Awake()
    {
        _enemyUnits = GetComponentsInChildren<Unit>();
    }

    private void Start()
    {
        foreach (var unit in _enemyUnits)
        {
            unit.Initialize(_playerUnitManager.GetUnits(), UnitType.Enemy);
        }
    }

    public Unit[] GetUnits()
    {
        return _enemyUnits;
    }
}