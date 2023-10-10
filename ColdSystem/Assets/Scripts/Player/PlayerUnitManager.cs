using UnityEngine;


public class PlayerUnitManager : MonoBehaviour
{
    [SerializeField] private EnemyUnitManager _enemyUnitManager;
    [SerializeField] private GameObject _selectedUnitIndicatorPrefab;

    private Unit[] _playerUnits;

    private void Awake()
    {
        _playerUnits = GetComponentsInChildren<Unit>();
    }

    private void Start()
    {
        foreach (var unit in _playerUnits)
        {
            unit.Initialize(_enemyUnitManager.GetUnits(), UnitType.Player);
        }
    }

    public Unit[] GetUnits()
    {
        return _playerUnits;
    }

    public void SetDestination(Vector3 destination)
    {
        foreach (var unit in _playerUnits)
        {
            if (unit != null && unit.Selected)
            {
                unit.SetDestination(destination);
            }
        }
    }

    public void SetUnitSelected(int unitIndex, bool selected)
    {
        if (unitIndex < 0 || unitIndex >= _playerUnits.Length)
        {
            Debug.LogError($"Unit index out of range: {unitIndex}");
            return;
        }

        var unit = _playerUnits[unitIndex];
        if (unit == null)
        {
            return;
        }

        unit.Selected = selected;
        if (unit.SelectedIndicator == null)
        {
            unit.SelectedIndicator = Instantiate(_selectedUnitIndicatorPrefab);
            unit.SelectedIndicator.transform.SetParent(unit.transform);
            unit.SelectedIndicator.transform.localPosition = Vector3.zero;
        }
        unit.SelectedIndicator.SetActive(selected);
    }
}