using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(CharacterMovementController))]
[RequireComponent(typeof(BasicCharacter))]
public class DeerCharacterAi : MonoBehaviour, Damageable.OnDamage
{
    public enum State
    {
        Idle, Sleeping, Alerted, LookingForFood, Eat, Wander, Dead
    }

    private CharacterMovementController controller;
    private BasicCharacter character;

    public State state = State.Idle;

    // Global state variables
    public float timeToGetHungry = 30;

    private float hungry = 0;

    // Idle state variables
    public float timeToChangeIdle = 0;

    private float timeToChangeIdleLeft = 0;

    // Looking for food variables
    private Vector2 grassPos = new Vector2(float.MaxValue, float.MaxValue);

    // Wander variables
    public float timeToWander = 5;

    private float timeToWanderLeft = 0;
    private Vector2 positionToWander = new Vector2(float.MaxValue, float.MaxValue);

    // Sleeping variables
    private float sleppines = 0;

    // Alerted variables;
    public float alertedTime = 15;

    private float alertedTimeLeft = 0;

    private GameObject whoAlerted;

    // Update is called once per frame
    private void Update()
    {
        updateStates();

        if (state == State.Idle)
            idle();
        else if (state == State.LookingForFood)
            lookForfood();
        else if (state == State.Eat)
            eat();
        else if (state == State.Wander)
            wander();
        else if (state == State.Alerted)
            alerted();
        // else if (state == State.Sleeping)
        //    sleep();
    }

    private void updateStates()
    {
        var damageable = GetComponent<Damageable>();

        if (damageable.IsDead)
        {
            state = State.Dead;
            return;
        }

        if (alertedTimeLeft > 0)
        {
            alertedTimeLeft -= Time.deltaTime;
            state = State.Alerted;
        }

        hungry += Time.deltaTime;
        sleppines += Time.deltaTime;

        timeToWanderLeft -= Time.deltaTime;

        //if (State.Idle == state && sleppines > 100)
        //    state = State.Sleeping;

        if (State.Idle == state && hungry > timeToGetHungry)
            state = State.LookingForFood;

        if (State.Idle == state && timeToWanderLeft < 0)
            state = State.Wander;
    }

    private void alerted()
    {
        if (alertedTimeLeft < 0)
            state = State.Idle;
        controller.MoveLeft(true);

        return;
        if (whoAlerted == null)
        {
            state = State.Idle;
            return;
        }
        if (whoAlerted.transform.position.x > transform.position.x)
            controller.MoveLeft(true);
        else
            controller.MoveRight(true);
    }

    private void wander()
    {
        if (positionToWander == new Vector2(float.MaxValue, float.MaxValue))
        {
            positionToWander = transform.position + new Vector3(Random.Range(-2f, 2f), 0, 0);
        }

        if (controller.MoveToPosition(positionToWander.x))
        {
            state = State.Idle;
            timeToWanderLeft = timeToWander;
            positionToWander = new Vector2(float.MaxValue, float.MaxValue);
        }
    }

    private void idle()
    {
        if (timeToChangeIdleLeft <= 0)
        {
            character.Animate("idle" + Random.Range(1, 3));
            timeToChangeIdleLeft = timeToChangeIdle;
        }
        timeToChangeIdleLeft -= Time.deltaTime;
    }

    private void lookForfood()
    {
        if (grassPos == new Vector2(float.MaxValue, float.MaxValue))
        {
            findFood();
        }
        else
        {
            if (controller.MoveToPosition(grassPos.x + 0.5f))
                state = State.Eat;
        }
    }

    private void eat()
    {
        if (character.Animate("eat", false))
        {
            hungry = 0;
            state = State.Idle;
            character.Animate("idle");
        }
    }

    private void findFood()
    {
        var colliders = Physics2D.OverlapCircleAll(transform.position, 5f);

        GameObject closestGrass = null;
        foreach (var collide in colliders)
        {
            if (collide.GetComponent<ExpensiveGrass>())
            {
                if (!closestGrass)
                    closestGrass = collide.gameObject;
                else if (
                    Vector2.Distance(closestGrass.transform.position, transform.position)
                    > Vector2.Distance(collide.transform.position, transform.position))
                    closestGrass = collide.gameObject;
            }
        }

        if (!closestGrass)
        {
            hungry = 0;
            state = State.Idle;
            return;
        }
        else
        {
            grassPos = closestGrass.transform.position;
        }
    }

    public void OnDamage(Damageable.DamageData damage)
    {
        alertedTimeLeft = alertedTime;
    }

    //private void sleep()
    //{
    //    var animator = GetComponent<SpriteAnimator>();
    //    if (animator.GetAnimation().name != "stand" || animator.GetAnimation().name != "")
    //    {
    //        if (animator.IsAnimationFinished)
    //            state = State.Idle;
    //        return;
    //    }

    //    if (sleppines < 0)
    //    {
    //        sleppines = 0;
    //        character.SetAnimation("stand",f);
    //        return;
    //    }

    //    if (animator.GetAnimation().name != "sit")
    //        animator.SetAnimation("sit");

    //    sleppines -= Time.deltaTime * 5;
    //}

    private void OnEnable()
    {
        controller = GetComponent<CharacterMovementController>();
        character = GetComponent<BasicCharacter>();
    }
}