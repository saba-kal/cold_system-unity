using System.Collections.Generic;
using System.Linq;
using UnityEngine;


public class PlayerUnitManager : MonoBehaviour
{
    private List<UnitMovement> _playerUnits = new List<UnitMovement>();

    private void Start()
    {
        _playerUnits = GetComponentsInChildren<UnitMovement>().ToList();
    }

    public void SetDestination(Vector3 destination)
    {
        foreach (var unitMovement in _playerUnits)
        {
            unitMovement.SetDestination(destination);
        }
    }
}