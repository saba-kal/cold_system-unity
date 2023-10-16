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
    private float _originalSpeed;

    private void Start()
    {
        _agent = GetComponent<NavMeshAgent>();
        _autoAttack = GetComponent<UnitAutoAttack>();
        _animator = GetComponentInChildren<Animator>();
        _originalSpeed = _agent.speed;
    }

    private void Update()
    {
        _animator?.SetFloat("Speed", _agent.velocity.sqrMagnitude);
        var speedMultiplier = 1.0f;
        if (_autoAttack?.IsAttacking() ?? false)
        {
            speedMultiplier = _attackModeSpeedMultiplier;
        }
        _agent.speed = _originalSpeed * speedMultiplier;
        _animator?.SetFloat("RunAnimationSpeed", _runAnimationSpeed * speedMultiplier);
    }

    public void SetDestination(Vector3 destination)
    {
        _agent.SetDestination(destination);
    }
}
