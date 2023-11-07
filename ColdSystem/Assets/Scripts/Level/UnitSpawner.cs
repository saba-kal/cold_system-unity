using System.Collections.Generic;
using UnityEngine;

public class UnitSpawner : MonoBehaviour
{
    [SerializeField] private Unit _unitPrefab;
    [SerializeField] private List<Vector3> _spawnLocations = new List<Vector3>();
    [SerializeField] private bool _spawnOnStart;

    private void Start()
    {
        if (_spawnLocations.Count == 0)
        {
            _spawnLocations.Add(transform.position);
        }

        if (_spawnOnStart)
        {
            SpawnUnits(_spawnLocations.Count);
        }
    }

    public void SpawnUnits(int count)
    {
        var enemyUnitManager = EnemyUnitManager.Instance;
        if (enemyUnitManager == null)
        {
            Debug.LogError("Cannot spawn units in level without an enemy unit manager.");
            return;
        }

        if (count == 0)
        {
            return;
        }

        if (count > _spawnLocations.Count)
        {
            count = _spawnLocations.Count;
        }

        var randomSpawnLocations = _spawnLocations.ChooseRandomElements(count);
        foreach (var spawnLocation in randomSpawnLocations)
        {
            var unit = Instantiate(_unitPrefab, transform.TransformPoint(spawnLocation), Quaternion.identity);
            enemyUnitManager.AddUnit(unit);
        }
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = new Color(1, 1, 0.3f, 0.5f);
        if (_spawnLocations == null || _spawnLocations.Count == 0)
        {
            Gizmos.DrawSphere(transform.position, 1f);
        }
        else
        {
            foreach (var spawnLocation in _spawnLocations)
            {
                Gizmos.DrawSphere(transform.position + spawnLocation, 1f);
            }
        }
    }
}
