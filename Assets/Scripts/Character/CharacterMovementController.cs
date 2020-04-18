using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CharacterMovementController : MonoBehaviour
{
    // Start is called before the first frame update
    public float walkSpeed = 1;

    public float runSpeed = 1;
    public float acceleration = 1;
    private float speed = 0;

    private int moveType = 0;

    public bool IsWalking
    {
        get => moveType == 1;
    }

    public bool IsRunning
    {
        get => moveType == 2;
    }

    public bool FaceLeft
    {
        get { return transform.localScale.x < 0; }
        set
        {
            if (FaceLeft == value)
                return;
            else if (value)
                transform.localScale = new Vector3(-1, 1, 1);
            else
                transform.localScale = new Vector3(1, 1, 1);
        }
    }

    private void Start()
    {
    }

    // Update is called once per frame
    private void LateUpdate()
    {
        transform.position += new Vector3(speed * Time.deltaTime, 0, 0);
    }

    public void MoveLeft(bool isRunning = false)
    {
        moveType = isRunning ? 2 : 1;
        FaceLeft = true;

        if (speed > 0)
            Stop();

        speed -= acceleration * Time.deltaTime;

        var maxSpeed = isRunning ? runSpeed : walkSpeed;

        if (-speed > maxSpeed)
            speed = -maxSpeed;
    }

    public void MoveRight(bool isRunning = false)
    {
        moveType = isRunning ? 2 : 1;
        FaceLeft = false;

        if (speed < 0)
            Stop();

        speed += acceleration * Time.deltaTime;

        var maxSpeed = isRunning ? runSpeed : walkSpeed;

        if (speed > maxSpeed)
            speed = maxSpeed;
    }

    public void Stop()
    {
        moveType = 0;
        speed = 0;
    }
}