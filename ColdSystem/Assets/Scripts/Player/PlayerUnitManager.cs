using FischlWorks_FogWar;
using Unity.VisualScripting;
using UnityEngine;


public class PlayerUnitManager : MonoBehaviour
{
    [SerializeField] private EnemyUnitManager _enemyUnitManager;
    [SerializeField] private GameObject _selectedUnitIndicatorPrefab;
    [SerializeField] private FieldOfViewMeshGenerator _fieldOfViewPrefab;

    private Unit[] _playerUnits;

    private void Awake()
    {
        _playerUnits = GetComponentsInChildren<Unit>();
    }

    private void Start()
    {
        var fogOfWar = csFogWar.Instance;
        foreach (var unit in _playerUnits)
        {
            unit.Initialize(_enemyUnitManager.GetUnits(), UnitType.Player);
            unit.SetHealthBarActive(false);
            unit.AddComponent<csFogVisibilityAgent>();
            unit.OnUnitDestroyed += OnUnitDeatroyed;
            fogOfWar?.AddFogRevealer(new csFogWar.FogRevealer(unit.transform, Mathf.RoundToInt(unit.GetRadius()), true));
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
        if (unitIndex < 0)
        {
            Debug.LogError($"Unit index out of range: {unitIndex}");
            return;
        }

        if (_playerUnits.Length == 0 || unitIndex >= _playerUnits.Length)
        {
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

    private void OnUnitDeatroyed(Unit unit)
    {
        var fogOfWar = csFogWar.Instance;
        if (fogOfWar != null)
        {
            fogOfWar.RemoveFogRevealer(unit.transform);
        }
    }
}