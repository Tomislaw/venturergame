using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Wind : MonoBehaviour
{
    public Vector2 size = new Vector2(12, 8);
    public float timeBetweenBlows = 5;
    public float maxWindForce = 20;
    public float windForce = 0;
    public float timeToNextBlow = 0;
    public bool blowing = false;

    // Start is called before the first frame update
    private void Start()
    {
    }

    // Update is called once per frame
    private void Update()
    {
        timeToNextBlow -= Time.deltaTime;
        if (timeToNextBlow < 0)
        {
            timeToNextBlow = Random.Range(0, timeBetweenBlows);
            blowing = true;
        }

        if (blowing)
        {
            windForce += maxWindForce * Time.deltaTime;
            if (windForce > maxWindForce)
            {
                windForce = maxWindForce;
                blowing = false;
            }
        }

        if (windForce <= 0)
            return;

        var colliders = Physics2D.OverlapAreaAll((Vector2)transform.position - size / 2, (Vector2)transform.position + size / 2);
        foreach (var collide in colliders)
        {
            var bodies = collide.GetComponentsInChildren<Rigidbody2D>();
            foreach (var body in bodies)
            {
                var factor = Mathf.PerlinNoise(body.transform.position.x / 10f, Time.time / 1000f);
                body.AddRelativeForce(new Vector2(factor * windForce * Time.deltaTime, 0));
            }
        }

        if (!blowing)
        {
            windForce -= maxWindForce * Time.deltaTime;
            if (windForce < 0)
                windForce = 0;
        }
    }
}