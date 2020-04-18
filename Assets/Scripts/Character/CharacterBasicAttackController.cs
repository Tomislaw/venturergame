using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public class CharacterBasicAttackController : MonoBehaviour
{
    public float attackTime = 0.8f;

    private float timeOfAttackLeft = 0;
    private bool _attackRequested = false;

    public bool IsAttacking { get => timeOfAttackLeft > 0; }
    public bool CanAttack { get => timeOfAttackLeft <= 0; }
    public bool IsFinishedAttacking { get; private set; } = false;
    public bool IsStartedAttacking { get; private set; } = false;

    public void Attack()
    {
        var movementController = GetComponent<CharacterMovementController>();
        var blockComtroller = GetComponent<CharacterBlockComponent>();
        if (movementController)
            movementController.Stop();
        if (blockComtroller)
            blockComtroller.StopBlocking();
        if (CanAttack)
            _attackRequested = true;
    }

    public void CancelAttack()
    {
        timeOfAttackLeft = 0;
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
                damageable.Damage(gameObject, 1);
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

        if (IsAttacking)
        {
            timeOfAttackLeft -= Time.deltaTime;
            if (timeOfAttackLeft < 0)
            {
                // deal damage here
                timeOfAttackLeft = 0;
                IsFinishedAttacking = true;
                FindAndDamage();
            }
        }
    }
}