using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

[CreateAssetMenu(fileName = "Keyboard controls", menuName = "Venturer/PlayerControls/Keyboard", order = 1)]
public class KeyboardPlayerControls : PlayerControls
{
    public override bool IsMoving { get; protected set; }
    public override bool IsMovingLeft { get; protected set; }
    public override bool IsSprinting { get; protected set; }
    public override bool IsAttacking { get; protected set; }
    public override float AttackAngle { get; protected set; }
    public override bool IsBlocking { get; protected set; }

    public KeyCode moveLeft = KeyCode.A;
    public KeyCode moveRight = KeyCode.D;
    public KeyCode attack = KeyCode.Space;
    public KeyCode toggleSprint = KeyCode.None;
    public KeyCode sprint = KeyCode.LeftShift;
    public KeyCode block = KeyCode.LeftControl;

    private bool _isSprinting = false;

    public override void UpdateControls()
    {
        bool left = Input.GetKey(moveLeft);
        bool right = Input.GetKey(moveRight);

        IsMoving = left != right;
        if (IsMoving)
            IsMovingLeft = left;

        IsSprinting = _isSprinting;

        if (Input.GetKeyDown(toggleSprint))
        {
            _isSprinting = !_isSprinting;
            IsSprinting = _isSprinting;
        }

        if (Input.GetKey(sprint))
        {
            IsSprinting = !_isSprinting;
        }

        IsBlocking = Input.GetKey(block);
        IsAttacking = Input.GetKey(attack);
    }
}