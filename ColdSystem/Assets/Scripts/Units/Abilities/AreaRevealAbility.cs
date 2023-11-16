
using System.Collections;
using UnityEngine;

[RequireComponent(typeof(Unit))]
public class AreaRevealAbility : UnitAbility
{
    public delegate void AreaRevealed(RevealedArea revealedArea);
    public static event AreaRevealed OnAreaRevealed;

    [SerializeField] private float _radius = 30f;
    [SerializeField] private float _duration = 20f;
    [SerializeField] private GameObject _effectPrefab;

    private UnitType _targetUnitType = UnitType.Enemy;
    private GameObject _effect = null;

    protected override void Start()
    {
        base.Start();

        var unit = GetComponent<Unit>();
        _targetUnitType = UnitFunctions.GetUnitTypeEnemyTo(unit.Type);
        if (_effectPrefab != null)
        {
            _effect = Instantiate(_effectPrefab);
            _effect.SetActive(false);
        }
    }

    protected override void ExecuteAbility()
    {
        OnAreaRevealed?.Invoke(new RevealedArea
        {
            Position = transform.position,
            Radius = _radius,
            Duration = _duration,
            TargetUnitType = _targetUnitType
        });
        if (_effect != null)
        {
            _effect.SetActive(true);
            _effect.transform.position = transform.position;
            _effect.transform.localScale = Vector3.one * _radius * 2;
            StartCoroutine(HideEffectAfterTime());
        }
    }

    private IEnumerator HideEffectAfterTime()
    {
        yield return new WaitForSeconds(_duration);
        _effect.SetActive(false);
    }
}

public struct RevealedArea
{
    public Vector3 Position;
    public float Radius;
    public float Duration;
    public UnitType TargetUnitType;
}
