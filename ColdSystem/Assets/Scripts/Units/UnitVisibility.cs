using System.Collections.Generic;
using System.Linq;
using UnityEngine;

[RequireComponent(typeof(Unit))]
public class UnitVisibility : MonoBehaviour
{
    [SerializeField] private float _onWeaponFiredRevealDuration = 5f;

    private Unit _unit;
    private bool _isVisible = true;
    private List<MeshRenderer> _meshRenderers = null;
    private List<SkinnedMeshRenderer> _skinnedMeshRenderers = null;
    private List<Canvas> _canvases = null;
    private List<Animator> _animators = null;
    private float _timeSinceLastWeaponFired = 100f;

    private void Awake()
    {
        _unit = GetComponent<Unit>();
        _animators = GetComponentsInChildren<Animator>().ToList();
        _meshRenderers = GetComponentsInChildren<MeshRenderer>().ToList();
        _skinnedMeshRenderers = GetComponentsInChildren<SkinnedMeshRenderer>().ToList();
        _canvases = GetComponentsInChildren<Canvas>().ToList();
    }

    private void OnEnable()
    {
        Unit.OnWeaponFired += OnWeaponFired;
    }

    private void OnDisable()
    {
        Unit.OnWeaponFired -= OnWeaponFired;
    }

    private void Update()
    {
        _timeSinceLastWeaponFired += Time.deltaTime;
    }

    public void SetVisible(bool isVisible)
    {
        if (isVisible == _isVisible)
        {
            return;
        }

        _isVisible = isVisible;

        if (FogOfWar.Instance != null && _unit.Type == UnitType.Enemy)
        {
            //Fog of war should show/hide enemy.
            ToggleRendererVisibility();
        }
    }

    public bool IsVisible()
    {
        return _isVisible;
    }

    public bool IsTimeSinceWeaponFireBelowRevealThreshold()
    {
        return _timeSinceLastWeaponFired < _onWeaponFiredRevealDuration;
    }

    private void OnWeaponFired(Unit unit)
    {
        if (unit == _unit)
        {
            _timeSinceLastWeaponFired = 0f;
        }
    }

    private void ToggleRendererVisibility()
    {
        if (_unit.Type == UnitType.Player)
        {
            return;
        }

        var newVisibility = _isVisible;

        foreach (var animator in _animators)
        {
            animator.enabled = newVisibility;
        }

        foreach (var renderer in _meshRenderers)
        {
            renderer.enabled = newVisibility;
        }

        foreach (var renderer in _skinnedMeshRenderers)
        {
            renderer.enabled = newVisibility;
        }

        foreach (var canvas in _canvases)
        {
            canvas.enabled = newVisibility;
        }
    }
}
