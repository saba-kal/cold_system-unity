using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(MeshFilter))]
public class FogOfWar : MonoBehaviour
{
    [Header("Fog Settings")]
    [SerializeField] private Vector2 _fogSize = new Vector2(100, 100);
    [SerializeField][Range(0.1f, 10f)] private float _fogMeshCellSize = 1f;
    [SerializeField] private float _maxFogHeight = 100f;
    [SerializeField] private float _fogHeightOffset = 2f;
    [SerializeField] private LayerMask _wrapFogToLayers;

    [Header("Field of View Settings")]
    [SerializeField][Range(2, 20)] private int _horizontalRayCount = 8;
    [SerializeField][Range(2, 20)] private int _verticalRayCount = 8;
    [SerializeField] private LayerMask _raycastAganstLayers;

    private MeshFilter _meshFilter;

    private void Awake()
    {
        _meshFilter = GetComponent<MeshFilter>();
    }

    private void Update()
    {
        UpdateFogOfWar();
    }

    public void InitializeHeightAdjustedFog()
    {
        if (_meshFilter == null)
        {
            _meshFilter = GetComponent<MeshFilter>();
        }

        var meshDimensions = new Vector2Int(
            Mathf.RoundToInt(_fogSize.x / _fogMeshCellSize),
            Mathf.RoundToInt(_fogSize.y / _fogMeshCellSize));
        var vertices = new Vector3[(meshDimensions.x + 1) * (meshDimensions.y + 1)];
        var uvs = new Vector2[vertices.Length];
        var i = 0;
        for (int y = 0; y < meshDimensions.y + 1; y++)
        {
            for (int x = 0; x < meshDimensions.x + 1; x++)
            {
                var xPosition = x * _fogMeshCellSize;
                var zPosition = y * _fogMeshCellSize;
                vertices[i] = GetHeightAdjustedPointUsingRaycast(xPosition, zPosition);
                uvs[i] = new Vector2(1.0f - x / (float)meshDimensions.x, 1.0f - y / (float)meshDimensions.y);
                i++;
            }
        }

        var triangles = new int[meshDimensions.x * meshDimensions.y * 6];
        for (int ti = 0, vi = 0, y = 0; y < meshDimensions.y; y++, vi++)
        {
            for (int x = 0; x < meshDimensions.x; x++, ti += 6, vi++)
            {
                triangles[ti] = vi;
                triangles[ti + 3] = triangles[ti + 2] = vi + 1;
                triangles[ti + 4] = triangles[ti + 1] = vi + meshDimensions.x + 1;
                triangles[ti + 5] = vi + meshDimensions.y + 2;
            }
        }

        var mesh = new Mesh();
        mesh.name = "Fog of War Plane";
        mesh.vertices = vertices;
        mesh.triangles = triangles;
        mesh.RecalculateNormals();
        mesh.uv = uvs;

        _meshFilter.mesh = mesh;
    }

    private Vector3 GetHeightAdjustedPointUsingRaycast(float x, float z)
    {
        if (Physics.Raycast(
            new Vector3(x + transform.position.x, _maxFogHeight, z + transform.position.z),
            Vector3.down,
            out var hit,
            _maxFogHeight,
            _wrapFogToLayers))
        {
            return transform.InverseTransformPoint(hit.point) + new Vector3(0, _fogHeightOffset, 0);
        }

        return new Vector3(x, _fogHeightOffset, z);
    }

    public void UpdateFogOfWar()
    {
        if (PlayerUnitManager.Instance == null)
        {
            return;
        }

        var rayStartPositions = new List<Vector4>();
        var rayEndPositions = new List<Vector4>();
        foreach (var unit in PlayerUnitManager.Instance.GetUnits())
        {
            if (unit == null) continue; //Unit is dead.

            var rayCasts = GetProjectedRectangleFieldOfViewPoints(unit);
            rayStartPositions.Add(unit.transform.position);
            rayEndPositions.AddRange(rayCasts);
        }

        Shader.SetGlobalInt("_RayOriginCount", rayStartPositions.Count);
        Shader.SetGlobalInt("_RayCount", rayEndPositions.Count);
        Shader.SetGlobalVectorArray("_RayStartPositions", rayStartPositions);
        Shader.SetGlobalVectorArray("_RayEndPositions", rayEndPositions);
    }

    private List<Vector4> GetProjectedRectangleFieldOfViewPoints(Unit unit)
    {
        var points = new List<Vector4>();
        var stepAngleSizeX = unit.FieldOfViewAngle / _horizontalRayCount;
        var stepAngleSizeY = (unit.FieldOfViewDepressionAngle + unit.FieldOfViewElevationAngle) / _verticalRayCount;

        for (var ix = 0; ix < _horizontalRayCount; ix++)
        {
            var yawAngle = unit.transform.eulerAngles.y - unit.FieldOfViewAngle / 2f + stepAngleSizeX * ix;
            for (var iy = 0; iy < _verticalRayCount; iy++)
            {
                var pitchAngle = unit.transform.eulerAngles.x - unit.FieldOfViewDepressionAngle + stepAngleSizeY * iy;
                var rayCastInfo = GetFieldOfViewRaycast(unit.transform.position, yawAngle, pitchAngle, unit.GetRadius());
                Debug.DrawLine(unit.transform.position, rayCastInfo.Point, rayCastInfo.Hit ? Color.red : Color.white);
                points.Add(rayCastInfo.Point);
            }
        }

        return points;
    }

    private RayCastInfo GetFieldOfViewRaycast(Vector3 startPosition, float yawAngle, float pitchAngle, float maxDistance)
    {
        Vector3 direction = DirectionFromYawAngle(yawAngle) + DirectionFromPitchAngle(pitchAngle);
        var isHit = false;
        var point = startPosition + direction * maxDistance;
        var distance = maxDistance;

        if (Physics.Raycast(startPosition, direction, out var hit, maxDistance, _raycastAganstLayers))
        {
            isHit = true;
            point = hit.point;
            distance = hit.distance;
        }

        return new RayCastInfo(isHit, point, distance);
    }

    private Vector3 DirectionFromYawAngle(float angleDegrees)
    {
        return new Vector3(Mathf.Sin(angleDegrees * Mathf.Deg2Rad), 0, Mathf.Cos(angleDegrees * Mathf.Deg2Rad));
    }

    private Vector3 DirectionFromPitchAngle(float angleDegrees)
    {
        return new Vector3(0, Mathf.Tan(angleDegrees * Mathf.Deg2Rad), 0);
    }

    public struct RayCastInfo
    {
        public bool Hit { get; set; }
        public Vector3 Point { get; set; }
        public float Distance { get; set; }

        public RayCastInfo(bool hit, Vector3 point, float distance)
        {
            Hit = hit;
            Point = point;
            Distance = distance;
        }
    }
}