using UnityEngine;
using UnityEngine.AI;

[RequireComponent(typeof(NavMeshAgent))]
public class UnitMovement : MonoBehaviour
{
    [SerializeField] private float _runAnimationSpeed = 1.0f;
    [SerializeField] private float _attackModeSpeedMultiplier = 0.5f;

    private NavMeshAgent _agent;
    private Animator _animator;
    private UnitAutoAttack _autoAttack;
    private UnitFieldOfView _fieldOfView;
    private Unit _targetUnit;
    private float _originalSpeed;

    private void Start()
    {
        _agent = GetComponent<NavMeshAgent>();
        _autoAttack = GetComponent<UnitAutoAttack>();
        _fieldOfView = GetComponent<UnitFieldOfView>();
        _animator = GetComponentInChildren<Animator>();
        _originalSpeed = _agent.speed;
    }

    private void Update()
    {
        _animator?.SetFloat("Speed", _agent.velocity.sqrMagnitude);
        var speedMultiplier = 1.0f;
        if (_autoAttack?.IsAttacking ?? false)
        {
            speedMultiplier = _attackModeSpeedMultiplier;
        }
        _agent.speed = _originalSpeed * speedMultiplier;
        _animator?.SetFloat("RunAnimationSpeed", _runAnimationSpeed * speedMultiplier);

        if (_targetUnit != null)
        {
            MoveTowardsTargetUnit();
        }
    }

    public void SetDestination(Vector3 destination)
    {
        _agent.SetDestination(destination);
    }

    public bool DestinationReached()
    {
        return (_agent.destination - transform.position).sqrMagnitude <=
            (_agent.stoppingDistance * _agent.stoppingDistance) + 0.1f;
    }

    public void SetTargetUnit(Unit unit)
    {
        _targetUnit = unit;
    }

    private void MoveTowardsTargetUnit()
    {
        if (_targetUnit == null)
        {
            return;
        }

        var distanceSqrToTarget = (_targetUnit.transform.position - transform.position).sqrMagnitude;
        var desiredDistance = _fieldOfView.GetFieldOfView().ViewDistance / 2;

        if (_autoAttack.UnitCanBeAttacked(_targetUnit) &&
            distanceSqrToTarget < (desiredDistance * desiredDistance))
        {
            SetDestination(transform.position);
        }
        else
        {
            SetDestination(_targetUnit.transform.position);
        }
    }
}
