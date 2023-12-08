using UnityEngine;

public class EnemyUnitHud : MonoBehaviour
{
    public void Initialize(Unit unit)
    {
        var healthBar = GetComponentInChildren<HealthBar>();
        if (healthBar != null)
        {
            var health = unit.GetComponent<Health>();
            healthBar.SetHealth(health);
        }

        var focusTargetIndicator = GetComponentInChildren<FocusTargetIndicator>();
        focusTargetIndicator?.Initialize(unit);
    }
}
