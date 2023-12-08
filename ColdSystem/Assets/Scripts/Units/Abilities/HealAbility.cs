using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.VFX;

public class HealAbility : UnitAbility
{

    [SerializeField] private float _radius = 10f;
    [SerializeField] private float _healDuration = 3f;
    [SerializeField] private float _healAmount = 30f;
    [SerializeField] private UnitType _targetUnit = UnitType.Player;
    [SerializeField] private GameObject _effectPrefab;

    private GameObject _effect = null;
    private VisualEffect _visualEffect = null;
    private UnitAutoAttack _autoAttack = null;

    private void Awake()
    {
        _autoAttack = GetComponent<UnitAutoAttack>();
    }

    protected override void Start()
    {
        base.Start();
        if (_effectPrefab != null)
        {
            _effect = Instantiate(_effectPrefab);
            _effect.transform.position = transform.position;
            _effect.SetActive(false);

            _visualEffect = _effect.GetComponent<VisualEffect>();
            _visualEffect?.SetFloat("Radius", _radius);
        }
    }

    protected override void Update()
    {
        base.Update();
        if (_effect != null)
        {
            _effect.transform.position = transform.position;
        }
    }

    protected override void ExecuteAbility()
    {
        var unitHealths = new List<Health>();
        foreach (var unit in UnitFunctions.GetUnitsFriendlyTo(_targetUnit))
        {
            if (unit == null) continue;
            var health = unit.GetComponent<Health>();
            if (health != null)
            {
                unitHealths.Add(health);
            }
        }

        StartCoroutine(HealUnitsOverTime(unitHealths));
    }

    private IEnumerator HealUnitsOverTime(List<Health> unitHealths)
    {
        var totalDuration = 0f;
        var healAmountPerSecond = _healAmount / _healDuration;
        ActivateEffect();
        while (totalDuration < _healDuration)
        {
            _autoAttack?.SetEnabled(false);
            foreach (var health in unitHealths)
            {
                if ((health.transform.position - transform.position).sqrMagnitude <= (_radius * _radius))
                {
                    health.Heal(healAmountPerSecond * Time.deltaTime);
                }
            }
            totalDuration += Time.deltaTime;
            yield return null;
        }
        _autoAttack?.SetEnabled(true);
        DeactivateEffect();
    }

    private void ActivateEffect()
    {
        _effect?.SetActive(true);
    }

    private void DeactivateEffect()
    {
        if (_visualEffect == null)
        {
            _effect?.SetActive(false);
        }
        else
        {
            StartCoroutine(ReduceParticleSpawnOverTime());
        }
    }

    private IEnumerator ReduceParticleSpawnOverTime()
    {
        var originalSpawnRate = _visualEffect.GetInt("SpawnRate");
        var duration = 0f;
        var spawnRate = (float)originalSpawnRate;
        while (duration <= 1f)
        {
            spawnRate -= Time.deltaTime * originalSpawnRate;
            spawnRate = Mathf.Clamp(spawnRate, 0, originalSpawnRate);
            _visualEffect.SetInt("SpawnRate", Mathf.RoundToInt(spawnRate));
            duration += Time.deltaTime;
            yield return null;
        }
        yield return new WaitForSeconds(2f);
        _effect.SetActive(false);
        _visualEffect.SetInt("SpawnRate", originalSpawnRate);
    }
}