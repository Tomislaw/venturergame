using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BasicCharacter : MonoBehaviour
{
    // Start is called before the first frame update
    private void Start()
    {
    }

    // Update is called once per frame

    private bool previousDeathState = false;
    private bool doingCustomAnimation;
    private bool idleOnFinish;

    private void Update()
    {
        var animator = GetComponent<SpriteAnimator>();
        if (animator == null)
            return;

        var damageable = GetComponent<Damageable>();
        if (damageable)
        {
            if (damageable.IsDead)
            {
                if (previousDeathState == false)
                {
                    previousDeathState = true;
                    animator.SetAnimation("death");
                }
                return;
            }
        }

        var movement = GetComponent<CharacterMovementController>();
        if (animator == null)
            return;

        if (movement.IsWalking)
        {
            animator.SetAnimation("walk");
            doingCustomAnimation = false;
        }
        else if (movement.IsRunning)
        {
            animator.SetAnimation("run");
            doingCustomAnimation = false;
        }
        else if (doingCustomAnimation)
        {
            if (idleOnFinish && animator.IsAnimationFinished)
            {
                animator.SetAnimation("idle");
                doingCustomAnimation = false;
            }
        }
        else
        {
            animator.SetAnimation("idle");
        }
    }

    public bool Animate(string animation, bool idleOnFinish = true)
    {
        var animator = GetComponent<SpriteAnimator>();
        if (animator == null)
            return true;

        doingCustomAnimation = true;
        this.idleOnFinish = idleOnFinish;

        if (animator.GetAnimation().name == animation)
            return animator.IsAnimationFinished;
        animator.SetAnimation(animation);

        return false;
    }
}