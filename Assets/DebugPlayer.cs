using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DebugPlayer : MonoBehaviour
{
    public Animator animator;
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
            transform.position -= new Vector3(3 * Time.deltaTime, 0, 0);
            transform.localScale = new Vector3(1, 1, 1);
            walking = true;
            PushGrass(-20);
        }
        else if (Input.GetKey(KeyCode.D))
        {
            transform.position += new Vector3(3 * Time.deltaTime, 0, 0);
            transform.localScale = new Vector3(-1, 1, 1);
            walking = true;
            PushGrass(20);
        }
        else
        {
            walking = false;
        }

        animator.SetBool("walk", walking);
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