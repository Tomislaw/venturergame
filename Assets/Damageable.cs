using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class Damageable : MonoBehaviour
{
    public int damageGroup = 0;
    public int health = 10;
    public int maxHealth = 10;

    public SpriteHealthBar healthbar;

    public bool IsDead { get => health <= 0; }

    public bool IsHealthChanged { get => previousHealth != health || HealthChange != 0; }

    public int HealthChange { get; private set; } = 0;

    private int previousHealth = 10;

    private void Start()
    {
        if (healthbar != null)
            healthbar.healthRange = Mathf.Min(1, Mathf.Max(0, (float)maxHealth / health));
    }

    private void Update()
    {
        if (previousHealth != health)
        {
            HealthChange = health - previousHealth;
            previousHealth = health;
            if (healthbar != null)
                healthbar.healthRange = Mathf.Min(1f, Mathf.Max(0f, (float)health / maxHealth));
        }
        else
        {
            HealthChange = 0;
        }
    }

    public void Damage(GameObject who, int damage)
    {
        if (IsDead)
            return;
        health -= damage;
        HealthChange = health - damage;
    }
}