using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface IZoneObject
{
    Zone Zone { get; set; }
}

[RequireComponent(typeof(Rigidbody2D))]
public class CharacterMovementController : MonoBehaviour, IZoneObject
{
    // Start is called before the first frame update
    public float walkSpeed = 1;

    public float runSpeed = 1;
    public float acceleration = 1;
    public float slowdown = 0.1f;
    public float speed { get; private set; } = 0;

    private int moveType = 0;
    private bool moveRequested = false;

    private Rigidbody2D rigidBody;

    public bool IsWalking
    {
        get => moveType == 1;
    }

    public bool IsRunning
    {
        get => moveType == 2;
    }

    public bool IsMoving
    {
        get => speed != 0;
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

    public Zone Zone { get; set; } = null;

    private void Awake()
    {
        rigidBody = GetComponent<Rigidbody2D>();

        var parent = transform.parent?.gameObject;
        while (Zone == null)
        {
            if (parent == null)
                break;
            Zone = parent.GetComponent<Zone>();
            parent = parent.transform.parent?.gameObject;
        }
    }

    // Update is called once per frame
    private void LateUpdate()
    {
        if (speed != 0)
        {
            rigidBody.velocity = new Vector2(speed, rigidBody.velocity.y);
        }
        else
        {
            var slowdownSpeed = slowdown;
            if (Mathf.Abs(rigidBody.velocity.x) < slowdown)
                slowdownSpeed = Mathf.Abs(rigidBody.velocity.x);

            if (rigidBody.velocity.x > 0)
                slowdownSpeed = -slowdownSpeed;

            rigidBody.velocity = new Vector2(rigidBody.velocity.x + slowdownSpeed, rigidBody.velocity.y);
        }

        //transform.position += new Vector3(speed * Time.deltaTime, 0, 0);
        if (!moveRequested && IsMoving)
            Stop();
        moveRequested = false;
    }

    public bool MoveToPosition(float position, bool isRunning = false, float margin = 0.5f)
    {
        if (transform.position.x.IsBetweenRange(position - margin, position + margin))
            return true;

        if (transform.position.x > position)
            MoveLeft(isRunning);
        else
            MoveRight(isRunning);

        return false;
    }

    public void MoveLeft(bool isRunning = false)
    {
        if (speed > 0)
            Stop();

        moveRequested = true;
        moveType = isRunning ? 2 : 1;
        FaceLeft = true;

        speed -= acceleration * Time.deltaTime;

        var maxSpeed = isRunning ? runSpeed : walkSpeed;

        if (-speed > maxSpeed)
            speed = -maxSpeed;
    }

    public void Move(bool isLeft, bool isRunning = false)
    {
        if (isLeft) MoveLeft(isRunning);
        else MoveRight(isRunning);
    }

    public void MoveRight(bool isRunning = false)
    {
        if (speed < 0)
            Stop();

        moveRequested = true;
        moveType = isRunning ? 2 : 1;
        FaceLeft = false;

        speed += acceleration * Time.deltaTime;

        var maxSpeed = isRunning ? runSpeed : walkSpeed;

        if (speed > maxSpeed)
            speed = maxSpeed;
    }

    public void ForceMove(float moveSpeed)
    {
        moveRequested = true;
        moveType = 0;

        if (speed != 0)
            FaceLeft = speed < 0;

        speed = moveSpeed;
    }

    public void Stop()
    {
        moveType = 0;
        speed = 0;
    }
}