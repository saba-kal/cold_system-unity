using UnityEngine;


public class FogOfWarShaderManager : MonoBehaviour
{
    [SerializeField] private Vector3 _lineStart;
    [SerializeField] private Vector3 _lineEnd;
    [SerializeField] private float _lineStartRadius;
    [SerializeField] private float _lineEndRadius;
    [SerializeField] private FieldOfViewMeshGenerator _fieldOfView;


    private void Start()
    {
        _fieldOfView = FindObjectOfType<FieldOfViewMeshGenerator>();
    }

    private void Update()
    {
        UpdateFogOfWarGlobals();
    }

    public void UpdateFogOfWarGlobals()
    {
        Shader.SetGlobalVector("_ExclusionPosition", transform.position);
        Shader.SetGlobalVector("_LineStart", _lineStart);
        Shader.SetGlobalVector("_LineEnd", _lineEnd);
        Shader.SetGlobalFloat("_RayStartRadius", _lineStartRadius);
        Shader.SetGlobalFloat("_RayEndRadius", _lineEndRadius);

        if (_fieldOfView != null)
        {
            var rayEndPositions = new Vector4[2000];
            for (var i = 0; i < _fieldOfView.GetRayEndPositions().Count && i < 2000; i++)
            {
                rayEndPositions[i] = _fieldOfView.GetRayEndPositions()[i];
            }
            Shader.SetGlobalInt("_RayOriginCount", 1);
            Shader.SetGlobalInt("_RayCount", _fieldOfView.GetRayEndPositions().Count);
            Shader.SetGlobalVectorArray("_RayStartPositions", new Vector4[] { _fieldOfView.transform.position });
            Shader.SetGlobalVectorArray("_RayEndPositions", rayEndPositions);
        }
        else
        {
            _fieldOfView = FindObjectOfType<FieldOfViewMeshGenerator>();
            Shader.SetGlobalInt("_RayOriginCount", 0);
            Shader.SetGlobalInt("_RayCount", 0);
        }
    }
}