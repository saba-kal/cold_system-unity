using UnityEngine;

public abstract class UnitAbility : MonoBehaviour
{
    public float CoolDown => _cooldown;
    public float TimeSinceLastActivation => _timeSinceLastActivation;


    [SerializeField] private float _cooldown = 10f;

    private float _timeSinceLastActivation = 0f;

    protected virtual void Start()
    {
        _timeSinceLastActivation = _cooldown;
    }

    protected virtual void Update()
    {
        _timeSinceLastActivation += Time.deltaTime;
    }

    public void Activate()
    {
        if (_timeSinceLastActivation < _cooldown)
        {
            return;
        }

        ExecuteAbility();
        _timeSinceLastActivation = 0f;
    }

    protected abstract void ExecuteAbility();
}
