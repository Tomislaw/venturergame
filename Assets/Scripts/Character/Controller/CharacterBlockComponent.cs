using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CharacterBlockComponent : MonoBehaviour
{
    public float blockTime = 0.1f;

    private float timeToStartBlocking = 0;
    private bool _blockRequested = false;

    public bool IsPreparingToBlock { get; private set; } = false;
    public bool IsBlocking { get; private set; } = false;

    public void StartBlocking()
    {
        var movementController = GetComponent<CharacterMovementController>();
        var attackController = GetComponent<CharacterBasicAttackController>();
        if (movementController != null)
        {
            movementController.Stop();
        }
        if (attackController != null)
        {
            movementController.Stop();
        }
        if (!IsPreparingToBlock && !IsBlocking)
            _blockRequested = true;
    }

    public void StopBlocking()
    {
        IsBlocking = false;
        IsPreparingToBlock = false;
        timeToStartBlocking = 0;
    }

    private void Start()
    {
    }

    // Update is called once per frame
    private void Update()
    {
        if (IsBlocking)
        {
            _blockRequested = false;
            IsPreparingToBlock = false;
        }

        if (_blockRequested && !IsPreparingToBlock && !IsBlocking)
        {
            timeToStartBlocking = blockTime;
            IsPreparingToBlock = true;
            _blockRequested = false;
        }

        if (IsPreparingToBlock)
        {
            timeToStartBlocking -= Time.deltaTime;
            if (timeToStartBlocking < 0)
            {
                // block ations here
                IsBlocking = true;
                IsPreparingToBlock = false;
                _blockRequested = false;
            }
        }
    }
}