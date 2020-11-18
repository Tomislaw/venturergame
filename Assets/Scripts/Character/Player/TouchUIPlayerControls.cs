using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

[CreateAssetMenu(fileName = "Touch UI controls", menuName = "Venturer/PlayerControls/TouchUI", order = 1)]
public class TouchUIPlayerControls : PlayerControls
{
    public override bool IsMoving { get; protected set; }
    public override bool IsMovingLeft { get; protected set; }
    public override bool IsSprinting { get; protected set; }
    public override bool IsAttacking { get; protected set; }
    public override float AttackAngle { get; protected set; }
    public override bool IsBlocking { get; protected set; }

    [HideInInspector]
    public float MoveX = 0;

    [HideInInspector]
    public bool Attacking;

    [HideInInspector]
    public float AttackingAngle;

    [HideInInspector]
    public bool Blocking;

    public bool? FaceLeft;

    public override void UpdateControls()
    {
        IsAttacking = Attacking;
        IsMoving = Mathf.Abs(MoveX) > 0.1;
        IsSprinting = Mathf.Abs(MoveX) > 0.6;
        IsMovingLeft = FaceLeft == null ? MoveX < 0 : FaceLeft.Value;
        IsAttacking = Attacking;
        IsBlocking = Blocking;
        AttackAngle = AttackingAngle;
    }
}