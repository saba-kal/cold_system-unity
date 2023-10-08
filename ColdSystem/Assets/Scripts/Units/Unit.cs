using UnityEngine;


[RequireComponent(typeof(UnitMovement))]
public class Unit : MonoBehaviour
{
    public bool Selected { get; set; }
    public GameObject SelectedIndicator { get; set; }

    private UnitMovement _unitMovement;
    private UnitAutoAttack _unitAutoAttack;

    private void Awake()
    {
        _unitMovement = GetComponent<UnitMovement>();
        _unitAutoAttack = GetComponent<UnitAutoAttack>();
    }

    public void SetDestination(Vector3 destination)
    {
        _unitMovement.SetDestination(destination);
    }

    public void SetOpposingUnits(Unit[] units)
    {
        _unitAutoAttack?.SetOpposingUnits(units);
    }
}
