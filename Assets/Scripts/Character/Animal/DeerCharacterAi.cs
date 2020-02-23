using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DeerCharacterAi : MonoBehaviour
{
    public enum State
    {
        Idle, Sleeping, Alerted, LookingForFood, Wander
    }

    public CharacterController controller;
    public SpriteAnimator animator;

    public State state = State.Idle;

    // Global state variables
    public float hungry = 0;

    // Idle state variables
    public float timeToChangeIdle = 7;

    // Looking for food variables
    public Vector2 grassPos = new Vector2(float.MaxValue, float.MaxValue);

    // Wander variables
    public float timeToWander = 20;

    public Vector2 positionToWander = new Vector2(float.MaxValue, float.MaxValue);

    // Sleeping variables
    public float sleppines = 0;

    // Start is called before the first frame update
    private void Start()
    {
    }

    // Update is called once per frame
    private void Update()
    {
        hungry += Time.deltaTime;
        sleppines += Time.deltaTime;

        timeToWander -= Time.deltaTime;

        if (State.Idle == state && sleppines > 100)
            state = State.Sleeping;

        if (State.Idle == state && hungry > 30)
            state = State.LookingForFood;

        if (State.Idle == state && timeToWander < 0)
            state = State.Wander;

        if (state == State.Idle)
        {
            idle();
        }
        else if (state == State.LookingForFood)
        {
            lookForfood();
        }
        else if (state == State.Wander)
        {
            wander();
        }
        else if (state == State.Alerted)
        {
        }
        else if (state == State.Sleeping)
        {
            sleep();
        }
    }

    private void alerted()
    {
    }

    private void wander()
    {
        if (positionToWander == new Vector2(float.MaxValue, float.MaxValue))
            positionToWander = transform.position + new Vector3(Random.Range(-5, 6), 0, 0);

        if (Mathf.Abs(positionToWander.x - transform.position.x) < 0.3)
        {
            state = State.Idle;
            animator.SetAnimation("idle");
            timeToWander = Random.Range(20, 30);
            positionToWander = new Vector2(float.MaxValue, float.MaxValue);
        }
        else
        {
            goToPosition(positionToWander);
        }
    }

    private void idle()
    {
        if (timeToChangeIdle <= 0)
        {
            if (animator.GetAnimation().name != "idle1" && animator.GetAnimation().name != "idle2")
                animator.SetAnimation("idle" + Random.Range(1, 3));

            if (!animator.IsAnimationFinished)
                return;

            timeToChangeIdle = Random.Range(3, 7);
        }
        timeToChangeIdle -= Time.deltaTime;
        animator.SetAnimation("idle");
    }

    private void lookForfood()
    {
        if (grassPos == new Vector2(float.MaxValue, float.MaxValue))
        {
            findFood();
        }
        else
        {
            if (Mathf.Abs(grassPos.x + 16 - transform.position.x) < 0.3)
            {
                eatFood();
                if (hungry < 0)
                    state = State.Idle;
            }
            else
            {
                goToPosition(grassPos + new Vector2(16, 0));
            }
        }
    }

    private void findFood()
    {
        var colliders = Physics2D.OverlapCircleAll(transform.position, 5f);
        foreach (var collide in colliders)
        {
            if (collide.GetComponent<ExpensiveGrass>())
            {
                grassPos = collide.transform.position;
            }
        }
    }

    private void eatFood()
    {
        if (animator.GetAnimation().name == "eat" && animator.IsAnimationFinished)
            hungry -= 30;
        animator.SetAnimation("eat");
    }

    private void goToPosition(Vector2 pos)
    {
        if (pos.x > transform.position.x)
        {
            controller.moveRight(false);
            animator.SetAnimation("run");
        }
        else
        {
            controller.moveLeft(false);
            animator.SetAnimation("run");
        }
    }

    private void sleep()
    {
        if (animator.GetAnimation().name == "stand")
        {
            if (animator.IsAnimationFinished)
                state = State.Idle;
            return;
        }

        if (sleppines < 0)
        {
            sleppines = 0;
            animator.SetAnimation("stand");
            return;
        }

        if (animator.GetAnimation().name != "sit")
            animator.SetAnimation("sit");

        sleppines -= Time.deltaTime * 5;
    }
}