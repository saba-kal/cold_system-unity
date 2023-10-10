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
        _health.OnDamageTaken += OnDamageTaken;
    }

    private void Start()
    {
        _healthSlider.minValue = 0;
        _healthSlider.maxValue = _health.GetMaxHealth();
        _healthSlider.value = _health.GetCurrentHealth();
    }

    private void OnDamageTaken()
    {
        _healthSlider.value = _health.GetCurrentHealth();
    }
}