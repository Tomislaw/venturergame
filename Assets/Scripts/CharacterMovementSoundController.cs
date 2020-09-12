using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CharacterMovementSoundController : MonoBehaviour
{
    public List<AudioClip> defaultSteps;
    public List<AudioClip> inWaterSteps;
    public List<AudioClip> inGrassSteps;
    public float timeBetweenSteps_running = 0.3f;
    public float timeBetweenSteps_walking = 0.5f;
    public AudioSource source;

    private Dictionary<WalkingOn, int> walkingOn = new Dictionary<WalkingOn, int>();
    private CharacterMovementController movement;
    private float timeToNextStep = 0;

    public int stepId = -1;

    private void Awake()
    {
        movement = GetComponent<CharacterMovementController>();
    }

    // Update is called once per frame
    private void Update()
    {
        if (!movement.IsMoving)
            return;

        timeToNextStep -= Time.deltaTime;
        if (timeToNextStep <= 0)
        {
            timeToNextStep = movement.IsRunning ? timeBetweenSteps_running : timeBetweenSteps_walking;
            PlayStepSound();
        }
    }

    public void StartedWalkingOn(WalkingOn type)
    {
        if (walkingOn.ContainsKey(type)) { walkingOn[type]++; }
        else walkingOn[type] = 1;
    }

    public void StoppedWalkingOn(WalkingOn type)
    {
        if (walkingOn.ContainsKey(type))
        {
            walkingOn[type]--;
            if (walkingOn[type] <= 0)
                walkingOn.Remove(type);
        }
    }

    private void PlayStepSound()
    {
        if (walkingOn.Count == 0)
        {
            if (defaultSteps.Count == 0)
                return;

            stepId = RandomIntExcept(0, defaultSteps.Count, stepId);
            source.PlayOneShot(defaultSteps[stepId]);
        }
        else
        {
            foreach (var type in walkingOn)
            {
                switch (type.Key)
                {
                    case WalkingOn.Grass:
                        if (inGrassSteps.Count == 0)
                            break;

                        stepId = RandomIntExcept(0, inGrassSteps.Count, stepId);
                        source.PlayOneShot(inGrassSteps[stepId]);
                        break;

                    case WalkingOn.Water:
                        if (inWaterSteps.Count == 0)
                            break;

                        stepId = RandomIntExcept(0, inWaterSteps.Count, stepId);
                        source.PlayOneShot(inWaterSteps[stepId]);
                        break;
                }
            }
        }
    }

    private int RandomIntExcept(int min, int max, int except)
    {
        int result = Random.Range(min, max);
        if (result == except)
        {
            result = result + 1;
            if (result >= max)
                return min;
        }
        return result;
    }

    public enum WalkingOn
    {
        Grass, Water
    }
}