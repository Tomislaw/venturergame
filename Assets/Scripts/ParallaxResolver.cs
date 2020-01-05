using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ParallaxResolver : MonoBehaviour
{
    public float movementFactor = 0;

    private Vector3 startingPosition;

    public bool moveX = true;
    public bool moveY = false;

    private void Start()
    {
        startingPosition = transform.position;
    }

    // Update is called once per frame
    private void Update()
    {
        Vector3 newPos = startingPosition;
        if (moveX)
            newPos.x += (startingPosition.x - Camera.main.transform.position.x) * movementFactor;
        if (moveY)
            newPos.y -= (startingPosition.y - Camera.main.transform.position.y) * movementFactor;
        transform.position = newPos;
    }

    private void OnDrawGizmos()
    {
        var sr = GetComponent<SpriteRenderer>();

        if (sr == null)
            return;

        Gizmos.color = Color.green;
        var size = sr.size;
        if (movementFactor > 0)
        {
            if (moveX)
                size.x /= 1 + movementFactor;
            if (moveY)
                size.y /= 1 + movementFactor;
        }
        else if (movementFactor < 0)
        {
            float f = 1 / (1 - (Mathf.Abs(movementFactor)));
            if (moveX)
                size.x *= f;
            if (moveY)
                size.y *= f;
        }

        Gizmos.DrawWireCube(sr.bounds.center, size);
        Gizmos.DrawSphere((Vector2)transform.position + sr.sprite.textureRectOffset, 0.1f);
        Gizmos.color = Color.white;
    }
}