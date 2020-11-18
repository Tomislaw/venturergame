using UnityEngine;

public class BlockingJoystick : BasicJoystick
{
    public TouchUIPlayerControls Controls;

    private void Update()
    {
        base.Update();
        if (Controls != null && Joystick != null)
        {
            Controls.Blocking = Joystick.Pressed;
            Joystick.transform.localPosition =
                new Vector2(
                    Center.x,
                    Joystick.transform.localPosition.y < 0 ? 0 : Joystick.transform.localPosition.y);
        }
    }
}