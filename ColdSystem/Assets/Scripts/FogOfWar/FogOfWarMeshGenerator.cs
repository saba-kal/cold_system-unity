using UnityEngine;

[RequireComponent(typeof(MeshFilter))]
public class FogOfWarMeshGenerator : MonoBehaviour
{
    [SerializeField] private int _meshSizeX = 100;
    [SerializeField] private int _meshSizeY = 100;
    [SerializeField][Range(0.1f, 10f)] private float _fogMeshCellSize = 1f;
    [SerializeField] private float _rayStartHeight = 100f;
    [SerializeField] private float _rayMaxDistance = 95f;
    [SerializeField] private float _fogOfWarPlaneHeightOffset = 2f;
    [SerializeField] private LayerMask _raycastLayers;

    private MeshFilter _meshFilter;

    private void Awake()
    {
        _meshFilter = GetComponent<MeshFilter>();
    }

    public void InitializeHeightAdjustedFog()
    {
        if (_meshFilter == null)
        {
            _meshFilter = GetComponent<MeshFilter>();
        }

        var vertices = new Vector3[(_meshSizeX + 1) * (_meshSizeY + 1)];
        var uvs = new Vector2[vertices.Length];
        var i = 0;
        for (int y = 0; y <= _meshSizeX; y++)
        {
            for (int x = 0; x <= _meshSizeY; x++)
            {
                var xPosition = x * _fogMeshCellSize;
                var zPosition = y * _fogMeshCellSize;
                vertices[i] = GetFogOfPlaneVertex(xPosition, zPosition);
                uvs[i] = new Vector2(1.0f - x / (float)_meshSizeX, 1.0f - y / (float)_meshSizeY);
                i++;
            }
        }

        var triangles = new int[_meshSizeX * _meshSizeY * 6];
        for (int ti = 0, vi = 0, y = 0; y < _meshSizeY; y++, vi++)
        {
            for (int x = 0; x < _meshSizeX; x++, ti += 6, vi++)
            {
                triangles[ti] = vi;
                triangles[ti + 3] = triangles[ti + 2] = vi + 1;
                triangles[ti + 4] = triangles[ti + 1] = vi + _meshSizeX + 1;
                triangles[ti + 5] = vi + _meshSizeY + 2;
            }
        }

        var mesh = new Mesh();
        mesh.name = "Fog of War Plane";
        mesh.vertices = vertices;
        mesh.triangles = triangles;
        mesh.uv = uvs;

        _meshFilter.mesh = mesh;
    }

    private float GetFogOfPlaneVertexHeight(float x, float z)
    {
        if (Physics.Raycast(
            new Vector3(x + transform.position.x, _rayStartHeight, z + transform.position.z),
            Vector3.down,
            out var hit,
            _rayMaxDistance,
            _raycastLayers))
        {
            return hit.point.y + _fogOfWarPlaneHeightOffset;
        }
        return _fogOfWarPlaneHeightOffset;
    }

    private Vector3 GetFogOfPlaneVertex(float x, float z)
    {
        if (Physics.Raycast(
            new Vector3(x + transform.position.x, _rayStartHeight, z + transform.position.z),
            Vector3.down,
            out var hit,
            _rayMaxDistance,
            _raycastLayers))
        {
            return transform.InverseTransformPoint(hit.point + hit.normal * _fogOfWarPlaneHeightOffset);
        }

        return new Vector3(x, _fogOfWarPlaneHeightOffset, z);
    }
}
