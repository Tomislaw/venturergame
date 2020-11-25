using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using UnityEngine;

[CreateAssetMenu(fileName = "Main controls", menuName = "Venturer/PlayerControls/Main", order = 1)]
public class MainPlayerControls : PlayerControls
{
    public PlayerControls Controls;

    public bool IsTouchBased
    {
        get => Controls != null && typeof(TouchUIPlayerControls).IsInstanceOfType(Controls) || typeof(TouchPlayerControls).IsInstanceOfType(Controls);
    }

    public override bool IsMoving { get => Controls.IsMoving; protected set { } }
    public override bool IsMovingLeft { get => Controls.IsMovingLeft; protected set { } }
    public override bool IsSprinting { get => Controls.IsSprinting; protected set { } }
    public override bool IsAttacking { get => Controls.IsAttacking; protected set { } }
    public override float AttackAngle { get => Controls.AttackAngle; protected set { } }
    public override bool IsBlocking { get => Controls.IsBlocking; protected set { } }
    public override bool IsUsing1 { get => Controls.IsUsing1; protected set { } }
    public override bool IsUsing2 { get => Controls.IsUsing2; protected set { } }

    public override void UpdateControls()
    {
        Controls.UpdateControls();
    }
}