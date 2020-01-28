using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DebugGrassPhysics : MonoBehaviour
{
    // Start is called before the first frame update

    public float Force = 10f;
    public float Radius = 2f;

    private float switchable = 1;

    private void Start()
    {
    }

    // Update is called once per frame
    private void Update()
    {
        if (Input.GetMouseButtonDown(0))
        {
            switchable *= -1;
            var pos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            Debug.Log("Input down " + pos);
            var colliders = Physics2D.OverlapCircleAll(pos, Radius);
            foreach (var collide in colliders)
            {
                var bodies = collide.GetComponentsInChildren<Rigidbody2D>();
                foreach (var body in bodies)
                {
                    body.AddExplosionForce(Force, pos, Radius);
                }
            }
        }
        if (Input.GetMouseButton(1))
        {
            var pos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            Debug.Log("Input down " + pos);
            var colliders = Physics2D.OverlapCircleAll(pos, Radius);
            foreach (var collide in colliders)
            {
                var bodies = collide.GetComponentsInChildren<Rigidbody2D>();
                foreach (var body in bodies)
                {
                    if (Vector2.Distance(pos, body.position) < Radius)
                        body.AddRelativeForce(new Vector2(Force * switchable * Time.deltaTime, 0));
                }
            }
        }
    }
}