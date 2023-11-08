using UnityEngine;
using UnityEngine.AI;

[RequireComponent(typeof(UnitMovement))]
public class MoveToRandomPointsOfInterest : MonoBehaviour
{
    private UnitMovement _unitMovement;

    private void Awake()
    {
        _unitMovement = GetComponent<UnitMovement>();
    }

    private void Update()
    {
        if (_unitMovement.DestinationReached())
        {
            _unitMovement.SetDestination(GetRandomLocation());
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
}
