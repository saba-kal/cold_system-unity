using UnityEngine;

public class FocusTargetIndicator : MonoBehaviour
{
    private Animation _animation;
    private Unit _unit;

    private void Awake()
    {
        _animation = GetComponent<Animation>();
    }

    private void OnEnable()
    {
        PlayerUnitManager.OnFocusTargetAquired += OnFocusTargetAquired;
    }

    private void OnDisable()
    {
        PlayerUnitManager.OnFocusTargetAquired -= OnFocusTargetAquired;
    }

    public void Initialize(Unit unit)
    {
        _unit = unit;
    }

    private void OnFocusTargetAquired(Unit targetUnit)
    {
        if (targetUnit == _unit)
        {
            if (_animation.isPlaying)
            {
                _animation.Stop();
            }
            _animation.Play();
        }
    }
}
