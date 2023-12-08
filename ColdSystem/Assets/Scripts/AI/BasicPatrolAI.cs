using UnityEngine;
using UnityEngine.AI;

[RequireComponent(typeof(UnitMovement))]
public class BasicPatrolAI : MonoBehaviour
{
    private AIState _state = AIState.Wander;
    private UnitMovement _unitMovement;
    private Unit _persueTarget;

    private void Awake()
    {
        _unitMovement = GetComponent<UnitMovement>();
    }

    private void Update()
    {
        switch (_state)
        {
            case AIState.Wander:
                Wander();
                break;
            case AIState.Persue:
                Persue();
                break;
        }
    }

    private void Wander()
    {
        if (_unitMovement.DestinationReached())
        {
            _unitMovement.SetDestination(GetRandomLocation());
        }

        var visibleEnemy = GetVisiblePlayerUnit();
        if (visibleEnemy != null)
        {
            _unitMovement.SetDestination(transform.position);
            _state = AIState.Persue;
            _persueTarget = visibleEnemy;
        }
    }

    private void Persue()
    {
        if (_persueTarget == null || !_persueTarget.IsVisible)
        {
            _state = AIState.Wander;
            _unitMovement.SetTargetUnit(null);
        }
        else
        {
            _unitMovement.SetTargetUnit(_persueTarget);
        }
    }

    private Vector3 GetRandomLocation()
    {
        var navMeshData = NavMesh.CalculateTriangulation();

        // Pick the first indice of a random triangle in the nav mesh
        var t = Random.Range(0, navMeshData.indices.Length - 3);

        // Select a random point on it
        var point = Vector3.Lerp(navMeshData.vertices[navMeshData.indices[t]], navMeshData.vertices[navMeshData.indices[t + 1]], Random.value);
        Vector3.Lerp(point, navMeshData.vertices[navMeshData.indices[t + 2]], Random.value);

        return point;
    }

    private Unit GetVisiblePlayerUnit()
    {
        if (PlayerUnitManager.Instance == null)
        {
            return null;
        }

        Unit nearestVisibleUnit = null;
        var minDistanceSqr = float.MaxValue;
        foreach (var unit in PlayerUnitManager.Instance.GetUnits())
        {
            if (unit == null || !unit.IsVisible) continue;

            var distanceSqr = (transform.position - unit.transform.position).sqrMagnitude;
            if (distanceSqr < minDistanceSqr)
            {
                nearestVisibleUnit = unit;
                minDistanceSqr = distanceSqr;
            }
        }

        return nearestVisibleUnit;
    }
}

public enum AIState
{
    Wander = 1,
    Persue = 2
}
