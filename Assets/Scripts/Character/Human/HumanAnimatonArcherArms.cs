using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HumanAnimatonArcherArms : MonoBehaviour
{
    public GameObject objectToMove;

    public Vector2 maxRotationOffset = new Vector2(0, 0.1f);
    public Vector2 minRotationOffset = new Vector2(0, -0.05f);
    public Vector2 rotationOffset = new Vector2();

    public float maxRotationZ = 80;
    public float minRotationZ = -80;

    private float previousRotation;

    public bool TakeRotationFromParent = false;

    private void Update()
    {
        float rotation = TakeRotationFromParent ? transform.localEulerAngles.z : transform.localEulerAngles.z;
        if (rotation > 180)
            rotation -= 360;
        if (rotation == previousRotation)
            return;

        if (rotation >= 0)
        {
            if (rotation > maxRotationZ)
            {
                objectToMove.transform.localPosition = maxRotationOffset;
                previousRotation = maxRotationZ;
                return;
            }
            else
            {
                var factor = rotation / maxRotationZ;
                objectToMove.transform.localPosition = Vector2.Lerp(rotationOffset, maxRotationOffset, factor);
            }
        }
        else
        {
            if (rotation < minRotationZ)
            {
                objectToMove.transform.localPosition = minRotationOffset;
                previousRotation = minRotationZ;
                return;
            }
            else
            {
                var factor = rotation / minRotationZ;
                objectToMove.transform.localPosition = Vector2.Lerp(rotationOffset, minRotationOffset, factor);
            }
        }

        previousRotation = rotation;
    }
}