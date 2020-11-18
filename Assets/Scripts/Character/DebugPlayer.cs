using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(CharacterMovementController))]
public class DebugPlayer : MonoBehaviour
{
    // Start is called before the first frame update
    private void Start()
    {
    }

    public PlayerControls controls;

    // Update is called once per frame
    private void Update()
    {
        var character = GetComponent<CharacterMovementController>();
        controls.UpdateControls();

        if (controls.IsAttacking)
        {
            var attackComponent = GetComponent<CharacterHumanAttackController>();
            attackComponent.attackAngle = (int)controls.AttackAngle;
            attackComponent.Attack(controls.IsMoving);

            if (controls.IsMoving)
                character.Move(controls.IsMovingLeft, false);
            else
                character.FaceLeft = controls.IsMovingLeft;
        }
        else if (controls.IsBlocking)
        {
            var blockComponent = GetComponent<CharacterBlockComponent>();
            blockComponent.Block();

            if (controls.IsMoving)
                character.Move(controls.IsMovingLeft, false);
            else
                character.FaceLeft = controls.IsMovingLeft;
        }
        else if (controls.IsMoving)
        {
            character.Move(controls.IsMovingLeft, controls.IsSprinting);
        }
    }
}