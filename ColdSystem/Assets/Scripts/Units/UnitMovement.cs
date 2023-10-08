using UnityEngine;
using UnityEngine.AI;

[RequireComponent(typeof(NavMeshAgent))]
public class UnitMovement : MonoBehaviour
{
    [SerializeField] private float _runAnimationSpeed = 1.0f;

    private NavMeshAgent _agent;
    private Animator _animator;

    private void Start()
    {
        _agent = GetComponent<NavMeshAgent>();
        _animator = GetComponentInChildren<Animator>();
    }

    private void Update()
    {
        _animator?.SetFloat("Speed", _agent.velocity.sqrMagnitude);
        _animator?.SetFloat("RunAnimationSpeed", _runAnimationSpeed);
    }

    public void SetDestination(Vector3 destination)
    {
        _agent.SetDestination(destination);
    }
}
