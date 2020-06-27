using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(CharacterMovementController))]
[RequireComponent(typeof(HumanCharacter))]
public class DebugPlayer : MonoBehaviour
{
    // Start is called before the first frame update
    private void Start()
    {
    }

    // Update is called once per frame
    private void Update()
    {
        var character = GetComponent<CharacterMovementController>();

        if (Input.GetKey(KeyCode.Space))
        {
            var attackComponent = GetComponent<CharacterBasicAttackController>();

            bool moveLeft = Input.GetKey(KeyCode.A);
            bool moveRight = Input.GetKey(KeyCode.D);

            if (attackComponent.CanAttack)
            {
                if (moveLeft != moveRight)
                {
                    character.FaceLeft = moveLeft;
                }
            }
            attackComponent.Attack(moveLeft || moveRight);
        }
        else if (Input.GetKey(KeyCode.LeftControl))
        {
            GetComponent<CharacterBlockComponent>().StartBlocking();
        }
        else if (Input.GetKey(KeyCode.A))
        {
            GetComponent<CharacterMovementController>().MoveLeft(Input.GetKey(KeyCode.LeftShift));
            PushGrass(character.speed);
        }
        else if (Input.GetKey(KeyCode.D))
        {
            GetComponent<CharacterMovementController>().MoveRight(Input.GetKey(KeyCode.LeftShift));
            PushGrass(character.speed);
        }
        else
        {
            // GetComponent<CharacterMovementController>().Stop();
        }
    }

    private void PushGrass2()
    {
        var colliders = Physics2D.OverlapCircleAll(transform.position, 1);
        foreach (var collide in colliders)
        {
            var bodies = collide.GetComponentsInChildren<ExpensiveGrassStalk>();
            foreach (var body in bodies)
            {
                var rg = body.GetComponent<Rigidbody2D>();
                if (Vector2.Distance(body.transform.position, transform.position) < 1)
                    rg.AddExplosionForce(1f, transform.position, 1);
            }
        }
    }

    private void PushGrass(float force)
    {
        var collider = GetComponent<Collider2D>();
        var colliders = new List<Collider2D>();
        collider.OverlapCollider(new ContactFilter2D(), colliders);

        foreach (var collide in colliders)
        {
            var bodies = collide.GetComponentsInChildren<ExpensiveGrassStalk>();
            foreach (var body in bodies)
            {
                var rg = body.GetComponent<Rigidbody2D>();
                if (Mathf.Abs(body.attach.x % 0.02f) > 0.01)
                    if (collider.OverlapPoint(body.transform.position))
                        rg.velocity = new Vector2(force, rg.velocity.y);
            }
        }
    }
}