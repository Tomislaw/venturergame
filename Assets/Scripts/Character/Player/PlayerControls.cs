using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

public abstract class PlayerControls : ScriptableObject
{
    public abstract bool IsMoving { get; protected set; }
    public abstract bool IsMovingLeft { get; protected set; }
    public abstract bool IsSprinting { get; protected set; }

    public abstract bool IsAttacking { get; protected set; }
    public abstract float AttackAngle { get; protected set; }
    public abstract bool IsBlocking { get; protected set; }

    public abstract void UpdateControls();
}