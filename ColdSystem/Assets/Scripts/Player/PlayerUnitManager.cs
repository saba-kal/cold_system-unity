using System.Collections.Generic;
using System.Linq;
using UnityEngine;


public class PlayerUnitManager : MonoBehaviour
{
    public delegate void AllPlayerUnitsDestroyed();
    public static event AllPlayerUnitsDestroyed OnAllPlayerUnitsDestroyed;

    public static PlayerUnitManager Instance { get; private set; }

    [SerializeField] private GameObject _selectedUnitIndicatorPrefab;

    private List<Unit> _playerUnits;

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

        _playerUnits = GetComponentsInChildren<Unit>().ToList();
    }

    private void OnEnable()
    {
        Unit.OnUnitDestroyed += OnUnitDeatroyed;
    }

    private void OnDisable()
    {
        Unit.OnUnitDestroyed -= OnUnitDeatroyed;
    }

    private void Start()
    {
        foreach (var unit in _playerUnits)
        {
            unit.Initialize(UnitType.Player);
            unit.SetHealthBarActive(false);
        }
    }

    public List<Unit> GetUnits()
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

    public void ActivateAbilities()
    {
        foreach (var unit in _playerUnits)
        {
            if (unit != null && unit.Selected)
            {
                unit.ActivateAbility();
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

        if (_playerUnits.Count == 0 || unitIndex >= _playerUnits.Count)
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

    private void OnUnitDeatroyed(Unit deadUnit)
    {
        if (deadUnit.Type != UnitType.Player)
        {
            return;
        }

        foreach (var unit in _playerUnits)
        {
            if (unit != null && unit != deadUnit) return;
        }

        OnAllPlayerUnitsDestroyed?.Invoke();
    }
}