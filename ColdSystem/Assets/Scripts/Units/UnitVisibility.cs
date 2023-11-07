using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class UnitVisibility : MonoBehaviour
{
    private bool _isVisible = true;
    private List<MeshRenderer> _meshRenderers = null;
    private List<SkinnedMeshRenderer> _skinnedMeshRenderers = null;
    private List<Canvas> _canvases = null;

    private void Awake()
    {
        _meshRenderers = GetComponentsInChildren<MeshRenderer>().ToList();
        _skinnedMeshRenderers = GetComponentsInChildren<SkinnedMeshRenderer>().ToList();
        _canvases = GetComponentsInChildren<Canvas>().ToList();
    }

    public void SetVisible(bool isVisible)
    {
        if (isVisible == _isVisible)
        {
            return;
        }

        _isVisible = isVisible;

        foreach (var renderer in _meshRenderers)
        {
            renderer.enabled = isVisible;
        }

        foreach (var renderer in _skinnedMeshRenderers)
        {
            renderer.enabled = isVisible;
        }

        foreach (var canvas in _canvases)
        {
            canvas.enabled = isVisible;
        }
    }
}
