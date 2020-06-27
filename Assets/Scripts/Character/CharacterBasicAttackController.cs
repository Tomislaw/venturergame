using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public class CharacterBasicAttackController : MonoBehaviour
{
    public float timeBetweenAttack = 0.1f;
    public float attackTime = 0.3f;

    public bool canMoveWhileAttacking = true;
    public bool loseSpeedWhleAttacking = true;
    public float moveSpeedWhileAttacking = 0.1f;

    private float timeOfAttackLeft = 0;
    private float timeBetweenAttackLeft = 0;
    private float currentSpeedWhileAttacking = 0;
    private bool _attackRequested = false;
    private bool _attackRequestedWhileMoving = false;
    private bool? _ovverideDirection = null;

    public bool IsAttacking { get => timeOfAttackLeft > 0; }
    public bool CanAttack { get => timeOfAttackLeft <= 0 && timeBetweenAttackLeft <= 0; }
    public bool IsFinishedAttacking { get; private set; } = false;
    public bool IsStartedAttacking { get; private set; } = false;

    public void Attack(bool andMove = false)
    {
        var movementController = GetComponent<CharacterMovementController>();
        var blockComtroller = GetComponent<CharacterBlockComponent>();

        if (CanAttack)
        {
            _attackRequested = true;
            if (canMoveWhileAttacking)
            {
                _attackRequestedWhileMoving = andMove;
                currentSpeedWhileAttacking = movementController.FaceLeft ? -moveSpeedWhileAttacking : moveSpeedWhileAttacking;
                movementController.ForceMove(currentSpeedWhileAttacking);
            }
        }

        if (_ovverideDirection != null)
            movementController.FaceLeft = _ovverideDirection.Value;

        if (movementController && !canMoveWhileAttacking)
            movementController.Stop();
        if (blockComtroller)
            blockComtroller.StopBlocking();
    }

    public void CancelAttack()
    {
        timeOfAttackLeft = 0;
        timeBetweenAttackLeft = timeBetweenAttack;
    }

    private void FindAndDamage()
    {
        float weaponRange = 1;
        var colliders = Physics2D.OverlapCircleAll(transform.position, weaponRange);
        var movementController = GetComponent<CharacterMovementController>();
        foreach (var collider in colliders)
        {
            var damageable = collider.GetComponent<Damageable>();
            if (!damageable)
                continue;
            if (!movementController)
            {
                damageable.Damage(gameObject, 3);
            }
            else
            {
                if (movementController.FaceLeft && collider.transform.position.x > transform.position.x)
                    continue;
                if (!movementController.FaceLeft && collider.transform.position.x < transform.position.x)
                    continue;
                damageable.Damage(gameObject, 4);
            }
        }
    }

    private void Start()
    {
    }

    // Update is called once per frame
    private void Update()
    {
        IsFinishedAttacking = false;
        IsStartedAttacking = false;

        if (_attackRequested)
        {
            timeOfAttackLeft = attackTime;
            _attackRequested = false;
            IsStartedAttacking = true;
        }

        if (timeBetweenAttackLeft > 0)
        {
            timeBetweenAttackLeft -= Time.deltaTime;
        }

        if (IsAttacking)
        {
            // move while attacking
            if (canMoveWhileAttacking && _attackRequestedWhileMoving)
            {
                var movementController = GetComponent<CharacterMovementController>();
                if (movementController)
                {
                    if (loseSpeedWhleAttacking)
                        currentSpeedWhileAttacking *= Mathf.Max(0, (timeOfAttackLeft / attackTime));
                    movementController.ForceMove(currentSpeedWhileAttacking);
                }
            }

            // stop attacking and damage opponents
            timeOfAttackLeft -= Time.deltaTime;
            if (timeOfAttackLeft < 0)
            {
                // deal damage here
                timeOfAttackLeft = 0;
                IsFinishedAttacking = true;
                FindAndDamage();
                timeBetweenAttackLeft = timeBetweenAttack;
                currentSpeedWhileAttacking = 0;
            }
        }
    }
}