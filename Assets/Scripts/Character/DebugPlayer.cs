using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(CharacterController))]
[RequireComponent(typeof(HumanCharacter))]
public class DebugPlayer : MonoBehaviour
{
    public bool walking = false;

    // Start is called before the first frame update
    private void Start()
    {
    }

    // Update is called once per frame
    private void Update()
    {
        if (Input.GetKey(KeyCode.A))
        {
            GetComponent<CharacterController>().moveLeft(true);
            GetComponent<HumanCharacter>().SetAnimation("run");
            PushGrass(-20);
        }
        else if (Input.GetKey(KeyCode.D))
        {
            GetComponent<CharacterController>().moveRight(true);
            GetComponent<HumanCharacter>().SetAnimation("run");
            PushGrass(20);
        }
        else if (Input.GetKey(KeyCode.S))
        {
            GetComponent<SpriteAnimator>().SetAnimation("sit");
        }
        else if (Input.GetKey(KeyCode.W))
        {
            GetComponent<SpriteAnimator>().SetAnimation("stand");
        }
        else
        {
            GetComponent<SpriteAnimator>().SetAnimation("idle");
        }
    }

    private void PushGrass(float force)
    {
        var colliders = Physics2D.OverlapCircleAll(transform.position, 1f);
        foreach (var collide in colliders)
        {
            var bodies = collide.GetComponentsInChildren<Rigidbody2D>();
            foreach (var body in bodies)
            {
                if (Vector2.Distance(transform.position, body.position) < 0.7f)
                    body.AddRelativeForce(new Vector2(force * Time.deltaTime, 0));
            }
        }
    }
}