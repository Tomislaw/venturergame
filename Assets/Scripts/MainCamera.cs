using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MainCamera : MonoBehaviour
{
    public CharacterMovementController objectToFollow;

    private float walkOffset = 2;
    private float runOffset = 2.2f;

    private void Start()
    {
    }

    // Update is called once per frame
    private void Update()
    {
        if (!objectToFollow)
            return;

        var offset = objectToFollow.IsRunning ? runOffset : walkOffset;

        var cameraOffset = offset;
        if (objectToFollow.FaceLeft)
            cameraOffset = -offset;

        var newPosition = objectToFollow.gameObject.transform.position.x + cameraOffset;
        var x = Mathf.Lerp(transform.position.x, newPosition, 0.2f);

        transform.position = new Vector3(x, transform.position.y, transform.position.z);
    }
}