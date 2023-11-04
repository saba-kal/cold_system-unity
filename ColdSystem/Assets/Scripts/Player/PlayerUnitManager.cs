using FischlWorks_FogWar;
using UnityEngine;


public class PlayerUnitManager : MonoBehaviour
{
    public static PlayerUnitManager Instance { get; private set; }

    [SerializeField] private EnemyUnitManager _enemyUnitManager;
    [SerializeField] private GameObject _selectedUnitIndicatorPrefab;

    private Unit[] _playerUnits;
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

        _playerUnits = GetComponentsInChildren<Unit>();
    }

    private void Start()
    {
        var fogOfWar = csFogWar.Instance;
        _visibilityManager = new UnitVisibilityManager(_playerUnits, EnemyUnitManager.Instance?.GetUnits() ?? new Unit[0]);
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