using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MainCamera : MonoBehaviour
{
    public CharacterMovementController objectToFollow;

    public float walkOffset = 2;
    public float runOffset = 3f;

    public float moveTime = 2;

    private bool previousFaceLeft = false;
    private bool previousIsRunning = false;
    private float previousOffset = 0;
    private bool isSmoothlyMoveCamera = false;
    private float moveStatus = 1f;
    private float offset = 0;

    private void Start()
    {
        if (!objectToFollow)
        {
            previousFaceLeft = objectToFollow.FaceLeft;
            previousIsRunning = objectToFollow.IsRunning;
        }
    }

    // Update is called once per frame
    private void Update()
    {
        if (!objectToFollow)
            return;

        // if changed direction
        bool retriggerSmoothMovement = previousFaceLeft != objectToFollow.FaceLeft || previousIsRunning != objectToFollow.IsRunning;
        if (retriggerSmoothMovement)
        {
            isSmoothlyMoveCamera = true;
            moveStatus = 0;
            previousOffset = offset;
        }

        // get camera offset
        var desiredOffset = objectToFollow.IsRunning ? runOffset : walkOffset;
        if (objectToFollow.FaceLeft)
            desiredOffset = -desiredOffset;

        // smoothly move cameraa
        if (isSmoothlyMoveCamera)
        {
            moveStatus += Time.deltaTime / moveTime;
            if (moveStatus > 1)
            {
                moveStatus = 1;
                isSmoothlyMoveCamera = false;
            }
            offset = LeanTween.easeInOutQuad(previousOffset, desiredOffset, moveStatus);
        }
        else // snap camera to target position
        {
            offset = desiredOffset;
        }
        transform.position = new Vector3(objectToFollow.transform.position.x + offset, transform.position.y, transform.position.z);
        previousFaceLeft = objectToFollow.FaceLeft;
        previousIsRunning = objectToFollow.IsRunning;
    }
}