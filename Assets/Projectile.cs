using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Rigidbody2D))]
[RequireComponent(typeof(Damageable))]
[RequireComponent(typeof(Collider2D))]
public class Projectile : MonoBehaviour, Damageable.OnDamage
{
    public float timeAliveAfterHit = 1;
    public float maxTimeAlive = 60;

    private bool hitTarget = false;
    private float timeLeft;

    private Damageable damageable;
    private new Rigidbody2D rigidbody2D;
    private new Collider2D collider;

    private void Awake()
    {
        timeLeft = maxTimeAlive;
        damageable = GetComponent<Damageable>();
        rigidbody2D = GetComponent<Rigidbody2D>();
        collider = GetComponent<Collider2D>();
    }

    private void Update()
    {
        timeLeft -= Time.deltaTime;
        if (timeLeft < 0)
            Destroy(gameObject);

        if (!hitTarget)
        {
            Vector2 v = rigidbody2D.velocity;
            var angle = Mathf.Atan2(v.y, v.x) * Mathf.Rad2Deg;
            transform.rotation = Quaternion.AngleAxis(angle, Vector3.forward);
        }
    }

    public void OnDamage(Damageable.DamageData damage)
    {
        if (damage.isDead && !hitTarget)
            Destroy(gameObject);
    }

    private void OnCollisionEnter2D(Collision2D col)
    {
        hitTarget = true;
        timeLeft = timeAliveAfterHit;
        rigidbody2D.isKinematic = true;
        rigidbody2D.velocity = new Vector2();
        rigidbody2D.angularVelocity = 0;
        collider.enabled = false;

        var target = col.gameObject.GetComponent<Damageable>();

        gameObject.transform.SetParent(col.gameObject.transform, true);

        if (target)
        {
            target.Damage(gameObject, damageable.health);
            damageable.Kill(col.gameObject);
        }
    }

    public void Shoot(GameObject who, Vector2 velocity)
    {
        foreach (var collider in who.GetComponentsInChildren<Collider2D>())
        {
            Physics2D.IgnoreCollision(collider, this.collider);
        }

        rigidbody2D.velocity = velocity;
    }

    public void Shoot(GameObject who, float angle, float velocity)
    {
        Shoot(who, Quaternion.AngleAxis(angle, Vector3.forward) * new Vector2(velocity, 0));
    }
}