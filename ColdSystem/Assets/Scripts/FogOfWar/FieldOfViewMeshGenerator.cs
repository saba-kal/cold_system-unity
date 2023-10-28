using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(MeshFilter))]
public class FieldOfViewMeshGenerator : MonoBehaviour
{
    [SerializeField][Range(1, 100)] private int _meshResolution = 50;
    [SerializeField][Range(0, 20)] private int _edgeResolveIterations = 5;
    [SerializeField][Range(0f, 360f)] private float _edgeDistanceThreshold = 0.5f;
    [SerializeField][Range(0f, 360f)] private float _viewAngle = 90f;
    [SerializeField][Range(0f, 180f)] private float _depressionAngle = 60f;
    [SerializeField][Range(0f, 180f)] private float _elevationAngle = 30f;
    [SerializeField][Range(0f, 180f)] private float _wrapHeightOffset = 3f;
    [SerializeField][Range(1, 30)] private int _wrapResolution = 10;
    [SerializeField] private FieldOfViewType _type;
    [SerializeField] private LayerMask _collisionLayerMask;

    private List<Vector4> _rayEndPositions = new List<Vector4>();

    public enum FieldOfViewType
    {
        Flat = 0,
        Pyramid = 1,
        PyramidLines = 2,
    }

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
        switch (_type)
        {
            case FieldOfViewType.Flat:
                DrawFlatFieldOfView();
                break;
            case FieldOfViewType.Pyramid:
                DrawPyramidFieldOfView();
                break;
            case FieldOfViewType.PyramidLines:
                DrawPyramidFieldOfViewLines();
                break;
        }
    }

    public void SetRadius(float radius)
    {
        _radius = radius;
    }

    public List<Vector4> GetRayEndPositions() { return _rayEndPositions; }

    private void DrawFlatFieldOfView()
    {
        var viewPoints = GetCircleFieldOfViewPoints();
        var vertices = new Vector3[_wrapResolution * viewPoints.Count + 1];
        var sliceCount = (viewPoints.Count - 1);
        var triangles = new int[sliceCount * (_wrapResolution - 1) * 6 + (sliceCount * 3)];

        vertices[0] = Vector3.zero;
        var vi = 1;
        var ti = 0;
        for (var i = 0; i < viewPoints.Count; i++)
        {
            for (var j = 0; j < _wrapResolution; j++, vi++)
            {
                var worldPosition = Vector3.Lerp(transform.position, viewPoints[i], (j + 1) / (float)_wrapResolution);
                worldPosition = CastPositionDown(worldPosition);
                vertices[vi] = transform.InverseTransformPoint(worldPosition);

                if (j < _wrapResolution - 1 && i < viewPoints.Count - 1)
                {
                    triangles[ti] = vi;
                    triangles[ti + 1] = vi + 1;
                    triangles[ti + 2] = vi + _wrapResolution;

                    triangles[ti + 3] = vi + _wrapResolution;
                    triangles[ti + 4] = vi + 1;
                    triangles[ti + 5] = vi + _wrapResolution + 1;
                    ti += 6;
                }
            }
        }

        for (var i = 0; i < sliceCount; i++)
        {
            triangles[ti] = 0;
            triangles[ti + 1] = i * _wrapResolution + 1;
            triangles[ti + 2] = (i + 1) * _wrapResolution + 1;
            ti += 3;
        }

        _mesh.Clear();
        _mesh.vertices = vertices;
        _mesh.triangles = triangles;
    }

    private List<Vector3> GetCircleFieldOfViewPoints()
    {
        var stepAngleSize = _viewAngle / _meshResolution;
        var viewPoints = new List<Vector3>();
        var prevViewCast = new ViewCastInfo();
        for (int i = 0; i <= _meshResolution; i++)
        {
            var angle = transform.eulerAngles.y - _viewAngle / 2f + stepAngleSize * i;
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

    private void DrawPyramidFieldOfView()
    {
        var viewPoints = GetProjectedRectangleFieldOfViewPoints();
        var vertexCount = viewPoints.Count + 1;
        var vertices = new Vector3[vertexCount];

        vertices[0] = Vector3.zero;
        for (var i = 0; i < vertexCount - 1; i++)
        {
            vertices[i + 1] = transform.InverseTransformPoint(viewPoints[i].Point);
        }

        var triangles = new int[_meshResolution * _meshResolution * 6 + 3 * _meshResolution];
        // 6 * (n - 1) * (n - 1) = 
        // + 24 * _meshResolution
        //Pyramid base
        int ti = 0;
        int vi = 1;
        for (int y = 0; y < _meshResolution; y++, vi++)
        {
            for (int x = 0; x < _meshResolution; x++, ti += 6, vi++)
            {
                triangles[ti] = vi;
                triangles[ti + 3] = triangles[ti + 2] = vi + 1;
                triangles[ti + 4] = triangles[ti + 1] = vi + _meshResolution + 1;
                triangles[ti + 5] = vi + _meshResolution + 2;
            }
        }


        //Pyramid sides
        for (int i = 1; i <= _meshResolution; i++, ti += 3)
        {
            triangles[ti] = 0;
            triangles[ti + 1] = i;
            triangles[ti + 2] = i + 2;
        }

        _mesh.Clear();
        _mesh.vertices = vertices;
        _mesh.triangles = triangles;
    }

    private void DrawPyramidFieldOfViewLines()
    {
        var viewPoints = GetProjectedRectangleFieldOfViewPoints();
        _rayEndPositions = new List<Vector4>();
        foreach (var point in viewPoints)
        {
            _rayEndPositions.Add(point.Point);
        }
    }

    private List<ViewCastInfo> GetProjectedRectangleFieldOfViewPoints()
    {
        var points = new List<ViewCastInfo>();
        var stepAngleSizeX = _viewAngle / _meshResolution;
        var stepAngleSizeY = (_depressionAngle + _elevationAngle) / _meshResolution;

        for (var ix = 0; ix <= _meshResolution; ix++)
        {
            var yawAngle = transform.eulerAngles.y - _viewAngle / 2f + stepAngleSizeX * ix;
            for (var iy = 0; iy <= _meshResolution; iy++)
            {
                var pitchAngle = transform.eulerAngles.x - _depressionAngle + stepAngleSizeY * iy;
                var viewCastInfo = ViewCast(yawAngle, pitchAngle);
                points.Add(viewCastInfo);
            }
        }

        return points;
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
        Vector3 direction = DirectionFromYawAngle(angle);
        if (Physics.Raycast(transform.position, direction, out var hit, _radius, _collisionLayerMask))
        {
            return new ViewCastInfo(true, hit.point, hit.distance, angle);
        }
        return new ViewCastInfo(false, transform.position + direction * _radius, _radius, angle);
    }

    private ViewCastInfo ViewCast(float yawAngle, float pitchAngle)
    {
        Vector3 direction = DirectionFromYawAngle(yawAngle) + DirectionFromPitchAngle(pitchAngle);
        if (Physics.Raycast(transform.position, direction, out var hit, _radius, _collisionLayerMask))
        {
            return new ViewCastInfo(true, hit.point, hit.distance, yawAngle);
        }
        return new ViewCastInfo(false, transform.position + direction * _radius, _radius, yawAngle);
    }

    private Vector3 DirectionFromYawAngle(float angleDegrees)
    {
        return new Vector3(Mathf.Sin(angleDegrees * Mathf.Deg2Rad), 0, Mathf.Cos(angleDegrees * Mathf.Deg2Rad));
    }

    private Vector3 DirectionFromPitchAngle(float angleDegrees)
    {
        return new Vector3(0, Mathf.Tan(angleDegrees * Mathf.Deg2Rad), 0);
    }

    private Vector3 CastPositionDown(Vector3 position)
    {
        if (_wrapResolution <= 1)
        {
            return position;
        }

        if (Physics.Raycast(position, Vector3.down, out var hit, 100f, _collisionLayerMask))
        {
            return hit.point + new Vector3(0, _wrapHeightOffset, 0);
        }

        return position;
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