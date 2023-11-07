using UnityEngine;


public class Health : MonoBehaviour
{
    public delegate void DamageTaken();
    public delegate void HealthLost();

    public event DamageTaken OnDamageTaken;
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
        OnDamageTaken?.Invoke();
        if (_currentHealth <= 0 && !_allHealthLost)
        {
            OnHealthLost?.Invoke();
            _allHealthLost = true;
        }
    }

    public float GetCurrentHealth() => _currentHealth;
    public float GetMaxHealth() => _maxHealth;
}