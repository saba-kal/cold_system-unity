using UnityEngine;


public abstract class BaseWeapon : MonoBehaviour
{
    protected bool _enabled = false;

    public void SetEnabled(bool enabled)
    {
        _enabled = enabled;
    }
}