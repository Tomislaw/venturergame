using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StepSoundTrigger : MonoBehaviour
{
    public CharacterMovementSoundController.WalkingOn type;

    private void OnTriggerExit2D(Collider2D other)
    {
        var go = other.gameObject;

        var soundSet = go.GetComponent<CharacterMovementSoundController>();
        if (soundSet)
            soundSet.StoppedWalkingOn(type);
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        var go = other.gameObject;

        var soundSet = go.GetComponent<CharacterMovementSoundController>();
        if (soundSet)
            soundSet.StartedWalkingOn(type);
    }
}