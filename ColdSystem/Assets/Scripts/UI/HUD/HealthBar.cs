using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(Slider))]
public class HealthBar : MonoBehaviour
{
    [SerializeField] private Health _health;

    private Slider _healthSlider;

    private void Awake()
    {
        _healthSlider = GetComponent<Slider>();
        if (_health != null)
        {
            _health.OnHealthChanged += OnDamageTaken;
        }
    }

    private void Start()
    {
        _healthSlider.minValue = 0;
        _healthSlider.maxValue = _health?.GetMaxHealth() ?? 100;
        _healthSlider.value = _health?.GetCurrentHealth() ?? 0;
    }

    public void SetHealth(Health health)
    {
        if (health == null)
        {
            Debug.LogError("Could not setup unit health HUD because unit is missing health.");
            return;
        }

        _health = health;
        _health.OnHealthChanged += OnDamageTaken;
        _healthSlider.maxValue = _health.GetMaxHealth();
        _healthSlider.value = _health.GetCurrentHealth();
    }

    public void SetHealth(float maxHealth, float currentHealth)
    {
        _healthSlider.maxValue = maxHealth;
        _healthSlider.value = currentHealth;
    }

    private void OnDamageTaken()
    {
        _healthSlider.value = _health.GetCurrentHealth();
    }
}