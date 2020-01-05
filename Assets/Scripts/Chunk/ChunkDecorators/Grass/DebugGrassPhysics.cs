using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DebugGrassPhysics : MonoBehaviour
{
    // Start is called before the first frame update

    public float Force = 10f;
    public float Radius = 2f;

    private void Start()
    {
    }

    // Update is called once per frame
    private void Update()
    {
        if (Input.GetMouseButtonDown(0))
        {
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
    }
}