using UnityEngine;

public class AttackJoystick : BasicJoystick
{
    public TouchUIPlayerControls Controls;

    private new void Update()
    {
        base.Update();
        if (Controls != null && Joystick != null)
        {
            Controls.AttackingAngle = Vector2.SignedAngle(new Vector2(1, 0), Value);
            if (Controls.AttackingAngle > 90)
                Controls.AttackingAngle = -Controls.AttackingAngle + 180;
            else if (Controls.AttackingAngle < -90)
                Controls.AttackingAngle = -Controls.AttackingAngle - 180;
            Controls.Attacking = Joystick.Pressed;
            if (Joystick.Pressed)
                Controls.FaceLeft = Value.x < 0;
            else
                Controls.FaceLeft = null;
        }
    }
}