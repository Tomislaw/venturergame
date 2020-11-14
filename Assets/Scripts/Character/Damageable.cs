using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class Damageable : MonoBehaviour
{
    public struct DamageData
    {
        public GameObject who;
        public int damage;
        public int currentHealth;
        public int maxHealth;
        public bool isDead;
    }

    public interface OnDamage
    {
        void OnDamage(DamageData damage);
    }

    public int damageGroup = 0;
    public int health = 10;
    public int maxHealth = 10;

    public SpriteHealthBar healthbar;

    public bool IsDead { get => health <= 0; }

    private void Start()
    {
        if (healthbar != null)
            healthbar.healthRange = Mathf.Min(1, Mathf.Max(0, (float)maxHealth / health));
    }

    public void Damage(GameObject who, int damage, bool friendlyFire = false)
    {
        if (IsDead)
            return;

        var filter = who.GetComponent<RelationshipGroupFilter>();
        if (filter && !friendlyFire)
        {
            if (filter.GetBehavior(gameObject) == RelationshipGroupFilter.Behavior.Friendly)
                return;
        }

        health -= damage;

        if (healthbar != null)
            healthbar.healthRange = Mathf.Min(1f, Mathf.Max(0f, (float)health / maxHealth));

        var data = new DamageData
        {
            who = who,
            damage = damage,
            currentHealth = health,
            maxHealth = maxHealth,
            isDead = IsDead,
        };

        foreach (var item in GetComponentsInChildren<OnDamage>())
        {
            item.OnDamage(data);
        }
    }

    public void Kill(GameObject who = null)
    {
        Damage(who, this.health, true);
    }

    public bool CanHit(Damageable other)
    {
        return other.damageGroup != damageGroup;
    }
}