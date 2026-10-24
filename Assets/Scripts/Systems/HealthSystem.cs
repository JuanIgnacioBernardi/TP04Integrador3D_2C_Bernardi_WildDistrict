using System;
using UnityEngine;

public class HealthSystem : MonoBehaviour, IDamageable
{
    public event Action<float, float> OnHealthChanged; // current health, max health
    public event Action<float> OnDamaged;              // On being damaged  
    public event Action<float> OnHealed;               // on being healed
    public event Action OnDied;

    [SerializeField, Min(1f)] private float maxHealth = 100f;
    [SerializeField] private bool startInvulnerable;
    private float currentHealth;

    public float CurrentHealth => currentHealth;
    public float MaxHealth => maxHealth;
    public float Normalized => currentHealth / maxHealth;
    public bool IsDead => currentHealth <= 0f;
    public bool IsInvulnerable { get; set; }

    private void Awake()
    {
        currentHealth = maxHealth;
        IsInvulnerable = startInvulnerable;
    }
    private void Start() => NotifyHealthChanged();
    public void TakeDamage(float damage)
    {
        if (damage <= 0f || IsInvulnerable || IsDead) return;

        currentHealth = Mathf.Max(currentHealth - damage, 0f);

        OnDamaged?.Invoke(damage);
        NotifyHealthChanged();

        if (IsDead) OnDied?.Invoke();
    }
    public void Heal(float amount)
    {
        if (amount <= 0f || IsDead || currentHealth >= maxHealth) return;

        float previous = currentHealth;
        currentHealth = Mathf.Min(currentHealth + amount, maxHealth);

        OnHealed?.Invoke(currentHealth - previous);
        NotifyHealthChanged();
    }
    public void SetMaxHealth(float value, bool refill = false)
    {
        maxHealth = Mathf.Max(1f, value);
        currentHealth = refill ? maxHealth : Mathf.Min(currentHealth, maxHealth);
        NotifyHealthChanged();
    }
    public void ResetHealth()
    {
        currentHealth = maxHealth;
        NotifyHealthChanged();
    }

    private void NotifyHealthChanged() => OnHealthChanged?.Invoke(currentHealth, maxHealth);
}