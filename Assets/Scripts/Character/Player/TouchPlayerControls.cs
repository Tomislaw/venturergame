using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

[CreateAssetMenu(fileName = "Touch controls", menuName = "Venturer/PlayerControls/Touch", order = 1)]
public class TouchPlayerControls : PlayerControls
{
    public override bool IsMoving { get; protected set; }
    public override bool IsMovingLeft { get; protected set; }
    public override bool IsSprinting { get; protected set; }
    public override bool IsAttacking { get; protected set; }
    public override float AttackAngle { get; protected set; }
    public override bool IsBlocking { get; protected set; }

    private int moveTouchId = -1;
    private int attackTouchId = -1;
    private Vector2 moveStartPos;

    private float moveX = 0;

    public override void UpdateControls()
    {
        SetDataFromDevice();

        IsAttacking = moveTouchId != -1;
        IsMoving = moveX != 0;
        IsSprinting = Mathf.Abs(moveX) > 0.5;
        IsMovingLeft = moveX < 0;
        IsAttacking = attackTouchId != -1;
    }

    private void SetDataFromDevice()
    {
        foreach (var touch in Input.touches)
        {
            var phase = touch.phase;
            var id = touch.fingerId;
            var position = touch.position;

            switch (phase)
            {
                case TouchPhase.Began:
                    var center = Screen.width / 2;
                    if (position.x < center)
                    {
                        moveTouchId = id;
                        moveX = 0;
                        moveStartPos = touch.position;
                    }
                    else
                    {
                        attackTouchId = id;
                    }
                    break;

                case TouchPhase.Moved:
                    if (id == moveTouchId)
                    {
                        var maxMoveWidth = Screen.width / 6;
                        moveX = (position.x - moveStartPos.x) / maxMoveWidth;
                        if (moveX > 1)
                            moveX = 1;
                        else if (moveX < -1)
                            moveX = -1;
                    }
                    break;

                case TouchPhase.Ended:
                case TouchPhase.Canceled:
                    if (id == moveTouchId)
                    {
                        moveTouchId = -1;
                        moveX = 0;
                    }
                    else if (id == attackTouchId)
                    {
                        attackTouchId = -1;
                    }

                    break;

                case TouchPhase.Stationary:
                    break;
            }
        }
    }
}