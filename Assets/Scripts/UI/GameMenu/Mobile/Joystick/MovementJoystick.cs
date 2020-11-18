using UnityEngine;

public class MovementJoystick : BasicJoystick
{
    public TouchUIPlayerControls Controls;

    private void Update()
    {
        base.Update();
        if (Controls != null && Joystick != null)
        {
            Controls.MoveX = Value.x;
        }
    }
}