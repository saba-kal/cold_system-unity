using UnityEngine;
using UnityEngine.Rendering.HighDefinition;

[RequireComponent(typeof(LocalVolumetricFog))]
public class FogOfWarVolumeGenerator : MonoBehaviour
{
    [SerializeField][Range(0.1f, 5f)] private float _heightMapTileSize = 1f;
    [SerializeField] private float _minHeight = 0f;
    [SerializeField] private float _maxHeight = 30f;
    [SerializeField] private float _fogHeight = 4f;
    [SerializeField] private LayerMask _wrapFogToLayers;

    private Texture2D _texture;
    private LocalVolumetricFog _fog;

    private void Awake()
    {
        _fog = GetComponent<LocalVolumetricFog>();
    }

    private void Start()
    {
        WrapFogToTerrain();
    }

    public void WrapFogToTerrain()
    {
        if (_fog == null)
        {
            _fog = GetComponent<LocalVolumetricFog>();
        }

        var xSteps = Mathf.RoundToInt(_fog.parameters.size.x / _heightMapTileSize);
        var zSteps = Mathf.RoundToInt(_fog.parameters.size.z / _heightMapTileSize);

        _texture = new Texture2D(xSteps, zSteps);
        for (var i = 0; i < xSteps; i++)
        {
            for (var j = 0; j < zSteps; j++)
            {
                var x = i * _heightMapTileSize - _fog.parameters.size.x / 2;
                var z = j * _heightMapTileSize - _fog.parameters.size.z / 2;
                var rayStartPosition = new Vector3(x, _maxHeight, z) + transform.position;

                if (Physics.Raycast(rayStartPosition, Vector3.down, out var hit, _maxHeight + 100f, _wrapFogToLayers))
                {
                    var heightValue = Mathf.Clamp((hit.point.y - _minHeight) / (_maxHeight - _minHeight), 0, 1);
                    _texture.SetPixel(i, j, new Color(heightValue, heightValue, heightValue, 1));
                }
                else
                {
                    _texture.SetPixel(i, j, Color.black);
                }
            }
        }
        _texture.Apply();

        var material = _fog.parameters.materialMask;
        material?.SetTexture("_HeightMapTexture", _texture);
        material?.SetFloat("_MinHeight", _minHeight);
        material?.SetFloat("_MaxHeight", _maxHeight);
        material?.SetFloat("_FogHeight", _fogHeight);
    }

    public Texture2D GetHeightMap()
    {
        return _texture;
    }
}
