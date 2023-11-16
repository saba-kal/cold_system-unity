using UnityEngine;


public class Health : MonoBehaviour
{
    public delegate void HealthChanged();
    public delegate void HealthLost();

    public event HealthChanged OnHealthChanged;
    public event HealthLost OnHealthLost;

    [SerializeField] private float _maxHealth;

    private float _currentHealth;
    private bool _allHealthLost = false;

    private void Awake()
    {
        _currentHealth = _maxHealth;
    }

    public void TakeDamage(float damage)
    {
        _currentHealth -= damage;
        _currentHealth = Mathf.Clamp(_currentHealth, 0, _maxHealth);
        OnHealthChanged?.Invoke();
        if (_currentHealth <= 0 && !_allHealthLost)
        {
            OnHealthLost?.Invoke();
            _allHealthLost = true;
        }
    }

    public void Heal(float healAmount)
    {
        _currentHealth += healAmount;
        _currentHealth = Mathf.Clamp(_currentHealth, 0, _maxHealth);
        OnHealthChanged?.Invoke();
    }

    public float GetCurrentHealth() => _currentHealth;
    public float GetMaxHealth() => _maxHealth;
}