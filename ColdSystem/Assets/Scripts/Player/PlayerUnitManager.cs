using System.Collections.Generic;
using System.Linq;
using UnityEngine;


public class PlayerUnitManager : MonoBehaviour
{
    public static PlayerUnitManager Instance { get; private set; }

    [SerializeField] private GameObject _selectedUnitIndicatorPrefab;

    private List<Unit> _playerUnits;
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

        _playerUnits = GetComponentsInChildren<Unit>().ToList();
    }

    private void Start()
    {
        _visibilityManager = new UnitVisibilityManager(UnitType.Player);
        foreach (var unit in _playerUnits)
        {
            unit.Initialize(UnitType.Player);
            unit.SetHealthBarActive(false);
            unit.OnUnitDestroyed += OnUnitDeatroyed;
        }
    }

    private void Update()
    {
        _visibilityManager.UpdateVisibileUnits();
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

    private void OnUnitDeatroyed(Unit unit)
    {
        return; //Maybe do something in future.
    }
}