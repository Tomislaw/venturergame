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
                    animator.Animation = "death";
                }
                return;
            }
        }

        var movement = GetComponent<CharacterMovementController>();
        if (animator == null)
            return;

        if (movement.IsWalking)
        {
            animator.Animation = "walk";
            doingCustomAnimation = false;
        }
        else if (movement.IsRunning)
        {
            animator.Animation = "run";
            doingCustomAnimation = false;
        }
        else if (doingCustomAnimation)
        {
            if (idleOnFinish && animator.IsAnimationFinished)
            {
                animator.Animation = "idle";
                doingCustomAnimation = false;
            }
        }
        else
        {
            animator.Animation = "idle";
        }
    }

    public bool Animate(string animation, bool idleOnFinish = true)
    {
        var animator = GetComponent<SpriteAnimator>();
        if (animator == null)
            return true;

        doingCustomAnimation = true;
        this.idleOnFinish = idleOnFinish;

        if (animator.Animation == null)
        {
            animator.Animation = animation;
        }

        if (animator.Animation == animation)
            return animator.IsAnimationFinished;
        animator.Animation = animation;

        return false;
    }
}