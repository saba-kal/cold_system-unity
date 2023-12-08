using System;
using UnityEngine;


public abstract class BaseWeapon : MonoBehaviour
{
    protected bool _enabled = false;
    protected UnitType _parentUnitType;
    protected Action _onWeaponFired;

    public void Initialize(UnitType parentUnitType)
    {
        _parentUnitType = parentUnitType;
    }

    public void SetEnabled(bool enabled)
    {
        _enabled = enabled;
    }

    public void SetOnWeaponFired(Action onWeaponFired)
    {
        _onWeaponFired = onWeaponFired;
    }
}