using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Box : MonoBehaviour, Damageable.OnDamage
{
    public InventoryItem items;

    private void Start()
    {
    }

    // Update is called once per frame
    private void Update()
    {
        var animator = GetComponent<SimpleAnimator>();
        if (animator == null)
            return;

        if (animator.Animation == "damage")
            if (animator.IsAnimationFinished)
                animator.Animation = "idle";
    }

    public void OnDamage(Damageable.DamageData damage)
    {
        var animator = GetComponent<SimpleAnimator>();
        if (animator == null)
            return;
        if (damage.isDead)
            animator.Animation = "destroy";
        else if (damage.damage > 0)
            animator.Animation = "damage";
    }
}