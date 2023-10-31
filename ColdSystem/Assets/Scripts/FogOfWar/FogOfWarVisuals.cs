using UnityEngine;
using UnityEngine.VFX;

[RequireComponent(typeof(VisualEffect))]
public class FogOfWarVisuals : MonoBehaviour
{
    [SerializeField][Range(0, 500f)] private float _mapSizeX = 100f;
    [SerializeField][Range(0, 500f)] private float _mapSizeY = 100f;
    [SerializeField][Range(0.1f, 5f)] private float _heightMapTileSize = 1f;
    [SerializeField] private float _minHeight = 0f;
    [SerializeField] private float _maxHeight = 30f;
    [SerializeField] private float _fogHeightOffset = 4f;
    [SerializeField] private float _terrainSlopOffset = 1f;
    [SerializeField] private LayerMask _wrapFogToLayers;

    private VisualEffect _visualEffect;
    private Texture2D _texture;

    public Texture2D Texture => _texture;

    private void Awake()
    {
        _visualEffect = GetComponent<VisualEffect>();
    }

    private void Start()
    {
        InitializeFog();
    }

    public void InitializeFog()
    {
        if (_visualEffect == null)
        {
            _visualEffect = GetComponent<VisualEffect>();
        }

        var xSteps = Mathf.RoundToInt(_mapSizeX / _heightMapTileSize);
        var zSteps = Mathf.RoundToInt(_mapSizeY / _heightMapTileSize);

        _texture = new Texture2D(xSteps, zSteps);
        for (var i = 0; i < xSteps; i++)
        {
            for (var j = 0; j < zSteps; j++)
            {
                var x = i * _heightMapTileSize;
                var z = j * _heightMapTileSize;
                var rayStartPosition = new Vector3(x, _maxHeight, z) + transform.position;

                if (Physics.Raycast(rayStartPosition, Vector3.down, out var hit, _maxHeight + 100f, _wrapFogToLayers))
                {
                    var hitHeight = hit.point.y + new Vector3(hit.normal.x, 0, hit.normal.z).magnitude * _terrainSlopOffset;
                    var heightValue = (hitHeight - _minHeight) / (_maxHeight - _minHeight);
                    heightValue = Mathf.Clamp(heightValue, 0, 1);
                    _texture.SetPixel(i, j, new Color(heightValue, heightValue, heightValue, 1));
                }
                else
                {
                    _texture.SetPixel(i, j, Color.black);
                }
            }
        }
        _texture.Apply();

        _visualEffect.SetVector2("MapBounds", new Vector2(_mapSizeX, _mapSizeY));
        _visualEffect.SetFloat("MaxHeight", _maxHeight);
        _visualEffect.SetFloat("HeightOffset", _fogHeightOffset);
        _visualEffect.SetTexture("HeightMap", _texture);
    }
}
