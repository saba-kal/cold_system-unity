using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(MeshFilter))]
public class FieldOfViewMeshGenerator : MonoBehaviour
{
    [SerializeField] private int _meshResolution = 100;
    [SerializeField] private int _edgeResolveIterations = 5;
    [SerializeField] private float _edgeDistanceThreshold = 0.5f;
    [SerializeField] private LayerMask _collisionLayerMask;

    private float _radius = 20f;
    private MeshFilter _meshFilter;
    private Mesh _mesh;

    private void Awake()
    {
        _mesh = new Mesh();
        _mesh.name = "View mesh";
        _meshFilter = GetComponent<MeshFilter>();
        _meshFilter.mesh = _mesh;
    }

    private void LateUpdate()
    {
        DrawFieldOfView();
    }

    public void SetRadius(float radius)
    {
        _radius = radius;
    }

    private void DrawFieldOfView()
    {
        var viewPoints = GetCircleFieldOfViewPoints();
        var vertexCount = viewPoints.Count + 1;
        var vertices = new Vector3[vertexCount];
        var triangles = new int[(vertexCount - 2) * 3];

        vertices[0] = Vector3.zero;
        for (var i = 0; i < vertexCount - 1; i++)
        {
            vertices[i + 1] = transform.InverseTransformPoint(viewPoints[i]);

            if (i < vertexCount - 2)
            {
                triangles[i * 3] = 0;
                triangles[i * 3 + 1] = i + 1;
                triangles[i * 3 + 2] = i + 2;
            }
        }

        _mesh.Clear();
        _mesh.vertices = vertices;
        _mesh.triangles = triangles;
        _mesh.RecalculateNormals();
    }

    private List<Vector3> GetCircleFieldOfViewPoints()
    {
        var stepAngleSize = 360f / _meshResolution;
        var viewPoints = new List<Vector3>();
        var prevViewCast = new ViewCastInfo();
        for (int i = 0; i <= _meshResolution; i++)
        {
            var angle = stepAngleSize * i;
            var viewCastInfo = ViewCast(angle);

            if (i > 0 && prevViewCast.Hit != viewCastInfo.Hit)
            {
                var edgeDstThresholdExceeded = Mathf.Abs(prevViewCast.Distance - viewCastInfo.Distance) > _edgeDistanceThreshold;
                if (prevViewCast.Hit != viewCastInfo.Hit ||
                    (prevViewCast.Hit && viewCastInfo.Hit && edgeDstThresholdExceeded))
                {
                    var edge = FindEdge(prevViewCast, viewCastInfo);
                    if (edge.PointA != Vector3.zero)
                    {
                        viewPoints.Add(edge.PointA);
                    }
                    if (edge.PointB != Vector3.zero)
                    {
                        viewPoints.Add(edge.PointB);
                    }
                }
            }

            viewPoints.Add(viewCastInfo.Point);
            prevViewCast = viewCastInfo;
        }

        return viewPoints;
    }

    private EdgeInfo FindEdge(ViewCastInfo minViewCast, ViewCastInfo maxViewCast)
    {
        var minAngle = minViewCast.Angle;
        var maxAngle = maxViewCast.Angle;
        var minPoint = Vector3.zero;
        var maxPoint = Vector3.zero;

        for (var i = 0; i < _edgeResolveIterations; i++)
        {
            var angle = (minAngle + maxAngle) / 2f;
            var viewCastInfo = ViewCast(angle);

            var edgeDstThresholdExceeded = Mathf.Abs(minViewCast.Distance - viewCastInfo.Distance) > _edgeDistanceThreshold;
            if (viewCastInfo.Hit == minViewCast.Hit && !edgeDstThresholdExceeded)
            {
                minAngle = angle;
                minPoint = viewCastInfo.Point;
            }
            else
            {

                maxAngle = angle;
                maxPoint = viewCastInfo.Point;
            }
        }

        return new EdgeInfo(minPoint, maxPoint);
    }

    private ViewCastInfo ViewCast(float angle)
    {
        Vector3 direction = DirectionFromAngle(angle);
        if (Physics.Raycast(transform.position, direction, out var hit, _radius, _collisionLayerMask))
        {
            return new ViewCastInfo(true, hit.point, hit.distance, angle);
        }
        return new ViewCastInfo(false, transform.position + direction * _radius, _radius, angle);
    }

    private Vector3 DirectionFromAngle(float angleDegrees)
    {
        return new Vector3(Mathf.Sin(angleDegrees * Mathf.Deg2Rad), 0, Mathf.Cos(angleDegrees * Mathf.Deg2Rad));
    }

    public struct ViewCastInfo
    {
        public bool Hit { get; set; }
        public Vector3 Point { get; set; }
        public float Distance { get; set; }
        public float Angle { get; set; }

        public ViewCastInfo(bool hit, Vector3 point, float distance, float angle)
        {
            Hit = hit;
            Point = point;
            Distance = distance;
            Angle = angle;
        }
    }

    public struct EdgeInfo
    {
        public Vector3 PointA { get; set; }
        public Vector3 PointB { get; set; }

        public EdgeInfo(Vector3 pointA, Vector3 pointB)
        {
            PointA = pointA;
            PointB = pointB;
        }
    }
}