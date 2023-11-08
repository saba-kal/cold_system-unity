using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class FogOfWar : MonoBehaviour
{
    public static FogOfWar Instance { get; private set; }

    [Header("Fog Settings")]
    [SerializeField] private Material _fogMaterial;
    [SerializeField] private Vector2 _fogChunkCount = new Vector2(5, 5);
    [SerializeField] private Vector2 _fogChunkSize = new Vector2(100, 100);
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
        if (Instance != null && Instance != this)
        {
            Destroy(this);
        }
        else
        {
            Instance = this;
        }

        _meshFilter = GetComponent<MeshFilter>();
    }

    private void Update()
    {
        UpdateFogOfWar();
    }

    public void InitializeHeightAdjustedFog()
    {
        if (_fogMaterial == null)
        {
            Debug.LogError("Fog is missing material.");
        }

        //Destroy existing chunks.
        for (var i = transform.childCount; i > 0; --i)
        {
            DestroyImmediate(transform.GetChild(0).gameObject);
        }

        var totalChunkCount = 0;
        for (var x = 0; x < _fogChunkCount.x; x++)
        {
            for (var y = 0; y < _fogChunkCount.y; y++)
            {
                CreateFogChunk(new Vector3(x * _fogChunkSize.x, 0, y * _fogChunkSize.y));
                totalChunkCount++;
                if (totalChunkCount >= 100)
                {
                    Debug.LogError("Reached maximum chunk count of 100. Unable to create more fog chunks.");
                    return;
                }
            }
        }
    }

    public void CreateFogChunk(Vector3 chunkPosition)
    {
        if (_meshFilter == null)
        {
            _meshFilter = GetComponent<MeshFilter>();
        }

        var meshDimensions = new Vector2Int(
            Mathf.RoundToInt(_fogChunkSize.x / _fogMeshCellSize),
            Mathf.RoundToInt(_fogChunkSize.y / _fogMeshCellSize));
        var vertices = new Vector3[(meshDimensions.x + 1) * (meshDimensions.y + 1)];
        var uvs = new Vector2[vertices.Length];
        var i = 0;
        for (int y = 0; y < meshDimensions.y + 1; y++)
        {
            for (int x = 0; x < meshDimensions.x + 1; x++)
            {
                var xPosition = x * _fogMeshCellSize;
                var zPosition = y * _fogMeshCellSize;
                vertices[i] = GetHeightAdjustedPointUsingRaycast(xPosition, zPosition, chunkPosition);
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

        var fogObject = new GameObject($"FogChunk_{chunkPosition.x}_{chunkPosition.z}");
        fogObject.transform.parent = transform;
        fogObject.transform.localPosition = chunkPosition;

        var meshFilter = fogObject.AddComponent<MeshFilter>();
        meshFilter.mesh = mesh;

        var meshRenderer = fogObject.AddComponent<MeshRenderer>();
        meshRenderer.material = _fogMaterial;
        meshRenderer.shadowCastingMode = UnityEngine.Rendering.ShadowCastingMode.Off;
    }

    private Vector3 GetHeightAdjustedPointUsingRaycast(float x, float z, Vector3 chunkPosition)
    {
        if (Physics.Raycast(
            new Vector3(x + chunkPosition.x, _maxFogHeight, z + chunkPosition.z) + transform.position,
            Vector3.down,
            out var hit,
            _maxFogHeight,
            _wrapFogToLayers))
        {
            return hit.point + new Vector3(0, _fogHeightOffset, 0) - transform.position - chunkPosition;
        }

        return new Vector3(x, _fogHeightOffset, z);
    }

    public void UpdateFogOfWar()
    {
        if (PlayerUnitManager.Instance == null)
        {
            return;
        }

        var rayStartPositions = new Vector4[5];
        var rayEndPositions = new Vector4[2000];
        var units = PlayerUnitManager.Instance.GetUnits().Where(u => u != null).ToList();
        var rayOriginCount = 0;
        var totalRayCount = 0;
        var rayOffset = 0;
        for (var i = 0; i < rayStartPositions.Length && i < units.Count; i++)
        {
            var unit = units[i];

            rayOriginCount++;

            var rayCasts = GetProjectedRectangleFieldOfViewPoints(unit);
            rayStartPositions[i] = unit.GetFieldOfViewStartPosition();
            rayOffset = rayCasts.Count;

            for (var j = 0; j < rayCasts.Count; j++)
            {
                rayEndPositions[totalRayCount + j] = rayCasts[j];
            }
            totalRayCount += rayOffset;
        }

        Shader.SetGlobalInt("_RayOriginCount", rayOriginCount);
        Shader.SetGlobalInt("_RayCount", totalRayCount);
        Shader.SetGlobalInt("_RayOffset", rayOffset);
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
            var unitEulerAngles = unit.GetFaceDirectionEulerAngles();
            var yawAngle = unitEulerAngles.y - unit.FieldOfViewAngle / 2f + stepAngleSizeX * ix;
            for (var iy = 0; iy < _verticalRayCount; iy++)
            {
                var pitchAngle = unitEulerAngles.x - unit.FieldOfViewDepressionAngle + stepAngleSizeY * iy;
                var rayCastInfo = GetFieldOfViewRaycast(unit.GetFieldOfViewStartPosition(), yawAngle, pitchAngle, unit.FieldOfViewDistance);
                Debug.DrawLine(unit.GetFieldOfViewStartPosition(), rayCastInfo.Point, rayCastInfo.Hit ? Color.red : Color.white);
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