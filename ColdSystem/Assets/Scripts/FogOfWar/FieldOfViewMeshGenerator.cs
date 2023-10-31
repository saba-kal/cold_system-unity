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
    [SerializeField] private GameObject _tinyBoxPrefab;

    private List<Vector4> _rayEndPositions = new List<Vector4>();
    private List<GameObject> _tinyBoxes = new List<GameObject>();
    private ViewCastInfo[,] _previousCasts;

    public Texture2D VisionTexture { get; private set; }


    public enum FieldOfViewType
    {
        Flat = 0,
        Pyramid = 1,
        PyramidLines = 2,
        TinyBoxes = 3,
        MarchingSquares = 4,
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
            case FieldOfViewType.TinyBoxes:
                DrawTinyBoxesForEachRay();
                break;
            case FieldOfViewType.MarchingSquares:
                DrawPyramidMarchingSquares();
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
        var vertexCount = viewPoints.GetLength(0) * viewPoints.GetLength(1) + 1;
        var vertices = new Vector3[vertexCount];

        vertices[0] = Vector3.zero;
        var vi = 1;
        for (var i = 0; i < viewPoints.GetLength(0); i++)
        {
            for (var j = 0; j < viewPoints.GetLength(1); j++)
            {
                if (vi < vertexCount)
                {
                    vertices[vi] = transform.InverseTransformPoint(viewPoints[i, j].Point);
                    vi++;
                }
            }
        }

        var triangles = new int[_meshResolution * _meshResolution * 6 + 3 * _meshResolution];
        // 6 * (n - 1) * (n - 1) = 
        // + 24 * _meshResolution
        //Pyramid base
        int ti = 0;
        vi = 1;
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

    private void DrawTinyBoxesForEachRay()
    {
        var viewPoints = GetProjectedRectangleFieldOfViewPoints();
        var hitPoints = new List<ViewCastInfo>();
        for (var i = 0; i < viewPoints.GetLength(0); i++)
        {
            for (var j = 0; j < viewPoints.GetLength(1); j++)
            {
                if (viewPoints[i, j].Hit)
                {
                    hitPoints.Add(viewPoints[i, j]);
                }
            }
        }

        var pointIntex = 0;
        foreach (var box in GetTinyBoxes(hitPoints.Count))
        {
            box.transform.position = hitPoints[pointIntex].Point;
            box.transform.localScale = Vector3.one * hitPoints[pointIntex].Distance / 10f;
            box.transform.rotation = Quaternion.LookRotation(transform.position - hitPoints[pointIntex].Point);
            pointIntex++;
        }
    }

    private void DrawPyramidMarchingSquares()
    {
        var viewPoints = GetProjectedRectangleFieldOfViewPoints();
        var vertexIndex = 0;
        var vertices = new List<Vector3>();
        var triangles = new List<int>();

        VisionTexture = new Texture2D(_meshResolution, _meshResolution);

        for (var i = 0; i < viewPoints.GetLength(0); i++)
        {
            for (var j = 0; j < viewPoints.GetLength(1); j++)
            {
                VisionTexture.SetPixel(i, j, viewPoints[i, j].Hit ? Color.white : Color.black);

                if (i >= _meshResolution - 1 || j >= _meshResolution - 1)
                {
                    continue;
                }

                var bottomLeftPoint = viewPoints[i, j];
                var topLeftPoint = viewPoints[i, j + 1];
                var bottomRightPoint = viewPoints[i + 1, j];
                var topRightPoint = viewPoints[i + 1, j + 1];

                var triangulationResult =
                    TriangulateSquare(
                        topLeftPoint,
                        topRightPoint,
                        bottomLeftPoint,
                        bottomRightPoint,
                        ref vertexIndex, i, j);

                vertices.AddRange(triangulationResult.Vertices);
                triangles.AddRange(triangulationResult.Triangles);
            }
        }

        VisionTexture.Apply();

        _mesh.Clear();


        _mesh.vertices = vertices.ToArray();
        _mesh.triangles = triangles.ToArray();
    }

    private ViewCastInfo[,] GetProjectedRectangleFieldOfViewPoints()
    {
        var points = new ViewCastInfo[_meshResolution + 1, _meshResolution + 1];
        var stepAngleSizeX = _viewAngle / _meshResolution;
        var stepAngleSizeY = (_depressionAngle + _elevationAngle) / _meshResolution;

        for (var ix = 0; ix <= _meshResolution; ix++)
        {
            var yawAngle = transform.eulerAngles.y - _viewAngle / 2f + stepAngleSizeX * ix;
            for (var iy = 0; iy <= _meshResolution; iy++)
            {
                var pitchAngle = transform.eulerAngles.x - _depressionAngle + stepAngleSizeY * iy;

                ViewCastInfo? previousCast = null;
                if (_previousCasts != null && ix < _previousCasts.GetLength(0) && iy < _previousCasts.GetLength(1))
                {
                    previousCast = _previousCasts[ix, iy];
                }

                var viewCastInfo = ViewCast(yawAngle, pitchAngle, previousCast);
                points[ix, iy] = viewCastInfo;
            }
        }

        _previousCasts = points;
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

    private ViewCastInfo ViewCast(float yawAngle, float pitchAngle, ViewCastInfo? previousViewCast)
    {
        Vector3 direction = DirectionFromYawAngle(yawAngle) + DirectionFromPitchAngle(pitchAngle);
        var isHit = false;
        var point = transform.position + direction * _radius;
        var distance = _radius;

        if (Physics.Raycast(transform.position, direction, out var hit, _radius, _collisionLayerMask))
        {
            isHit = true;
            point = hit.point;
            distance = hit.distance;
        }

        if (previousViewCast != null)
        {

            if ((previousViewCast.Value.Point - point).sqrMagnitude < 0.1f)
            {
                point = previousViewCast.Value.Point;
            }
            else
            {
                point = Vector3.Lerp(previousViewCast.Value.Point, point, Time.deltaTime * 20);
            }
        }


        return new ViewCastInfo(isHit, point, distance, yawAngle);
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

    private IEnumerable<GameObject> GetTinyBoxes(int count)
    {
        var currentBoxCount = _tinyBoxes.Count;
        for (var i = 0; i < count || i < currentBoxCount; i++)
        {
            if (i >= currentBoxCount)
            {
                var tinyBox = Instantiate(_tinyBoxPrefab);
                _tinyBoxes.Add(tinyBox);
                yield return tinyBox;
            }
            else if (i >= count && i < currentBoxCount)
            {
                _tinyBoxes[i].SetActive(false);
            }
            else
            {
                _tinyBoxes[i].SetActive(true);
                yield return _tinyBoxes[i];
            }
        }
    }

    private TriangulationResult TriangulateSquare(
        ViewCastInfo topLeftPoint,
        ViewCastInfo topRightPoint,
        ViewCastInfo bottomLeftPoint,
        ViewCastInfo bottomRightPoint,
        ref int vertexIndex,
        int i, int j)
    {
        var configurationCase =
            topLeftPoint.HitInt << 3 |
            topRightPoint.HitInt << 2 |
            bottomRightPoint.HitInt << 1 |
            bottomLeftPoint.HitInt;

        var topLeftVertex = transform.InverseTransformPoint(topLeftPoint.Point) + new Vector3(0, 1, 1);
        var topRightVertex = transform.InverseTransformPoint(topRightPoint.Point) + new Vector3(0, 1, 1);
        var bottomLeftVertex = transform.InverseTransformPoint(bottomLeftPoint.Point) + new Vector3(0, 1, 1);
        var bottomRightVertex = transform.InverseTransformPoint(bottomRightPoint.Point) + new Vector3(0, 1, 1);

        var vertices = new List<Vector3>();
        var triangles = new List<int>();
        switch (configurationCase)
        {
            // no points
            case 0:
                break;

            case 1:
                vertices.Add(bottomLeftVertex);
                vertices.Add(Vector3.Lerp(bottomLeftVertex, topLeftVertex, 0.5f));
                vertices.Add(Vector3.Lerp(bottomLeftVertex, bottomRightVertex, 0.5f));

                triangles.Add(vertexIndex);
                triangles.Add(vertexIndex + 1);
                triangles.Add(vertexIndex + 2);

                vertexIndex += 3;
                break;
            case 2:
                vertices.Add(bottomRightVertex);
                vertices.Add(Vector3.Lerp(bottomLeftVertex, bottomRightVertex, 0.5f));
                vertices.Add(Vector3.Lerp(topRightVertex, bottomRightVertex, 0.5f));

                triangles.Add(vertexIndex);
                triangles.Add(vertexIndex + 1);
                triangles.Add(vertexIndex + 2);

                vertexIndex += 3;
                break;
            case 3:
                vertices.Add(Vector3.Lerp(topLeftVertex, bottomLeftVertex, 0.5f));
                vertices.Add(Vector3.Lerp(topRightVertex, bottomRightVertex, 0.5f));
                vertices.Add(bottomRightVertex);
                vertices.Add(bottomLeftVertex);

                triangles.Add(vertexIndex);
                triangles.Add(vertexIndex + 1);
                triangles.Add(vertexIndex + 2);

                triangles.Add(vertexIndex + 2);
                triangles.Add(vertexIndex + 3);
                triangles.Add(vertexIndex);

                vertexIndex += 4;
                break;
            case 4:
                vertices.Add(topRightVertex);
                vertices.Add(Vector3.Lerp(topRightVertex, bottomRightVertex, 0.5f));
                vertices.Add(Vector3.Lerp(topLeftVertex, topRightVertex, 0.5f));

                triangles.Add(vertexIndex);
                triangles.Add(vertexIndex + 1);
                triangles.Add(vertexIndex + 2);

                vertexIndex += 3;
                break;
            case 5:
                vertices.Add(topRightVertex);
                vertices.Add(Vector3.Lerp(topRightVertex, bottomRightVertex, 0.5f));
                vertices.Add(Vector3.Lerp(topLeftVertex, topRightVertex, 0.5f));
                vertices.Add(Vector3.Lerp(topLeftVertex, bottomLeftVertex, 0.5f));
                vertices.Add(Vector3.Lerp(bottomLeftVertex, bottomRightVertex, 0.5f));
                vertices.Add(bottomLeftVertex);

                triangles.Add(vertexIndex);
                triangles.Add(vertexIndex + 1);
                triangles.Add(vertexIndex + 2);

                triangles.Add(vertexIndex + 2);
                triangles.Add(vertexIndex + 1);
                triangles.Add(vertexIndex + 4);

                triangles.Add(vertexIndex + 2);
                triangles.Add(vertexIndex + 1);
                triangles.Add(vertexIndex + 4);

                triangles.Add(vertexIndex + 5);
                triangles.Add(vertexIndex + 4);
                triangles.Add(vertexIndex + 3);

                vertexIndex += 6;
                break;
            case 6:
                vertices.Add(Vector3.Lerp(topLeftVertex, topRightVertex, 0.5f));
                vertices.Add(topRightVertex);
                vertices.Add(bottomRightVertex);
                vertices.Add(Vector3.Lerp(bottomLeftVertex, bottomRightVertex, 0.5f));

                triangles.Add(vertexIndex);
                triangles.Add(vertexIndex + 1);
                triangles.Add(vertexIndex + 2);

                triangles.Add(vertexIndex);
                triangles.Add(vertexIndex + 2);
                triangles.Add(vertexIndex + 3);

                vertexIndex += 4;
                break;
            case 7:
                vertices.Add(topRightVertex);
                vertices.Add(bottomRightVertex);
                vertices.Add(bottomLeftVertex);
                vertices.Add(Vector3.Lerp(topLeftVertex, bottomLeftVertex, 0.5f));
                vertices.Add(Vector3.Lerp(topLeftVertex, topRightVertex, 0.5f));

                triangles.Add(vertexIndex);
                triangles.Add(vertexIndex + 1);
                triangles.Add(vertexIndex + 4);

                triangles.Add(vertexIndex + 1);
                triangles.Add(vertexIndex + 3);
                triangles.Add(vertexIndex + 4);

                triangles.Add(vertexIndex + 1);
                triangles.Add(vertexIndex + 2);
                triangles.Add(vertexIndex + 3);

                vertexIndex += 5;
                break;
            case 8:
                vertices.Add(topLeftVertex);
                vertices.Add(Vector3.Lerp(topLeftVertex, topRightVertex, 0.5f));
                vertices.Add(Vector3.Lerp(topLeftVertex, bottomLeftVertex, 0.5f));

                triangles.Add(vertexIndex);
                triangles.Add(vertexIndex + 1);
                triangles.Add(vertexIndex + 2);

                vertexIndex += 3;
                break;
            case 9:
                vertices.Add(topLeftVertex);
                vertices.Add(Vector3.Lerp(topLeftVertex, topRightVertex, 0.5f));
                vertices.Add(bottomLeftVertex);
                vertices.Add(Vector3.Lerp(bottomLeftVertex, bottomRightVertex, 0.5f));

                triangles.Add(vertexIndex);
                triangles.Add(vertexIndex + 1);
                triangles.Add(vertexIndex + 2);

                triangles.Add(vertexIndex);
                triangles.Add(vertexIndex + 2);
                triangles.Add(vertexIndex + 3);

                vertexIndex += 4;
                break;
            case 10:
                vertices.Add(topLeftVertex);
                vertices.Add(Vector3.Lerp(topLeftVertex, topRightVertex, 0.5f));
                vertices.Add(Vector3.Lerp(topRightVertex, bottomRightVertex, 0.5f));
                vertices.Add(bottomRightVertex);
                vertices.Add(Vector3.Lerp(bottomLeftVertex, bottomRightVertex, 0.5f));
                vertices.Add(Vector3.Lerp(topLeftVertex, bottomLeftVertex, 0.5f));

                triangles.Add(vertexIndex);
                triangles.Add(vertexIndex + 1);
                triangles.Add(vertexIndex + 5);

                triangles.Add(vertexIndex + 1);
                triangles.Add(vertexIndex + 2);
                triangles.Add(vertexIndex + 5);

                triangles.Add(vertexIndex + 2);
                triangles.Add(vertexIndex + 4);
                triangles.Add(vertexIndex + 5);

                triangles.Add(vertexIndex + 2);
                triangles.Add(vertexIndex + 3);
                triangles.Add(vertexIndex + 4);

                vertexIndex += 6;
                break;
            case 11:
                vertices.Add(topLeftVertex);
                vertices.Add(Vector3.Lerp(topLeftVertex, topRightVertex, 0.5f));
                vertices.Add(Vector3.Lerp(topRightVertex, bottomRightVertex, 0.5f));
                vertices.Add(bottomRightVertex);
                vertices.Add(bottomLeftVertex);

                triangles.Add(vertexIndex);
                triangles.Add(vertexIndex + 1);
                triangles.Add(vertexIndex + 4);

                triangles.Add(vertexIndex + 1);
                triangles.Add(vertexIndex + 2);
                triangles.Add(vertexIndex + 4);

                triangles.Add(vertexIndex + 2);
                triangles.Add(vertexIndex + 3);
                triangles.Add(vertexIndex + 4);

                vertexIndex += 5;
                break;
            case 12:
                vertices.Add(topLeftVertex);
                vertices.Add(topRightVertex);
                vertices.Add(Vector3.Lerp(topRightVertex, bottomRightVertex, 0.5f));
                vertices.Add(Vector3.Lerp(topLeftVertex, bottomLeftVertex, 0.5f));

                triangles.Add(vertexIndex);
                triangles.Add(vertexIndex + 1);
                triangles.Add(vertexIndex + 2);

                triangles.Add(vertexIndex);
                triangles.Add(vertexIndex + 2);
                triangles.Add(vertexIndex + 3);

                vertexIndex += 4;
                break;
            case 13:
                vertices.Add(topLeftVertex);
                vertices.Add(topRightVertex);
                vertices.Add(Vector3.Lerp(topRightVertex, bottomRightVertex, 0.5f));
                vertices.Add(Vector3.Lerp(bottomLeftVertex, bottomRightVertex, 0.5f));
                vertices.Add(bottomLeftVertex);

                triangles.Add(vertexIndex);
                triangles.Add(vertexIndex + 1);
                triangles.Add(vertexIndex + 2);

                triangles.Add(vertexIndex + 2);
                triangles.Add(vertexIndex + 3);
                triangles.Add(vertexIndex);

                triangles.Add(vertexIndex + 3);
                triangles.Add(vertexIndex + 4);
                triangles.Add(vertexIndex);

                vertexIndex += 5;
                break;
            case 14:
                vertices.Add(topLeftVertex);
                vertices.Add(topRightVertex);
                vertices.Add(bottomRightVertex);
                vertices.Add(Vector3.Lerp(bottomLeftVertex, bottomRightVertex, 0.5f));
                vertices.Add(Vector3.Lerp(topLeftVertex, bottomLeftVertex, 0.5f));

                triangles.Add(vertexIndex);
                triangles.Add(vertexIndex + 1);
                triangles.Add(vertexIndex + 4);

                triangles.Add(vertexIndex + 1);
                triangles.Add(vertexIndex + 3);
                triangles.Add(vertexIndex + 4);

                triangles.Add(vertexIndex + 1);
                triangles.Add(vertexIndex + 2);
                triangles.Add(vertexIndex + 3);

                vertexIndex += 5;
                break;

            case 15:
                vertices.Add(topLeftVertex);
                vertices.Add(topRightVertex);
                vertices.Add(bottomRightVertex);
                vertices.Add(bottomLeftVertex);

                triangles.Add(vertexIndex);
                triangles.Add(vertexIndex + 1);
                triangles.Add(vertexIndex + 2);

                triangles.Add(vertexIndex + 2);
                triangles.Add(vertexIndex + 3);
                triangles.Add(vertexIndex);

                vertexIndex += 4;
                break;
        }

        return new TriangulationResult(vertices, triangles);
    }

    public struct ViewCastInfo
    {
        public bool Hit { get; set; }
        public Vector3 Point { get; set; }
        public float Distance { get; set; }
        public float Angle { get; set; }
        public int HitInt => Hit ? 1 : 0;

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

    public struct TriangulationResult
    {
        public List<Vector3> Vertices;
        public List<int> Triangles;

        public TriangulationResult(
            List<Vector3> vertices,
            List<int> triangles)
        {
            Vertices = vertices;
            Triangles = triangles;
        }
    }

    private static class CubeMeshInfo
    {
        public static Vector3[] vertices = {
            new Vector3 (0, 0, 0),
            new Vector3 (0.5f, 0, 0),
            new Vector3 (0.5f, 0.5f, 0),
            new Vector3 (0, 0.5f, 0),
            new Vector3 (0, 0.5f, 0.5f),
            new Vector3 (0.5f, 0.5f, 0.5f),
            new Vector3 (0.5f, 0, 0.5f),
            new Vector3 (0, 0, 0.5f),
        };
        public static int[] triangles = {
         0, 2, 1, //face front
         0, 3, 2,
         2, 3, 4, //face top
         2, 4, 5,
         1, 2, 5, //face right
         1, 5, 6,
         0, 7, 4, //face left
         0, 4, 3,
         5, 4, 7, //face back
         5, 7, 6,
         0, 6, 7, //face bottom
         0, 1, 6
         };
    }
}