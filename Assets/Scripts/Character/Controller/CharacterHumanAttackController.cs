﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

public class CharacterHumanAttackController : MonoBehaviour
{
    public float timeToChargeLightAttack = 1;
    public float timeToChargeHeavyAttack = 2;
    public float timeToChargeMaxAttack = 3;

    public float timeBetweenAttack = 0.2f;
    public float timeOfAttack = 0.2f;

    private float _timeToChargeLightAttackLeft = 0;
    private float _timeToChargeHeavyAttackLeft = 0;
    private float _timeOfAttackLeft = 0;
    private float _timeToNextAttackLeft = 0;
    private bool _attackRequested = false;
    private bool _attackRequestedPreviousState = false;
    private bool _cancelled = false;

    public State AttackState { get; private set; } = State.None;

    public bool CanRequestAttack
    {
        get => (AttackState == State.None && _timeToNextAttackLeft <= 0)
|| AttackState == State.ChargingLight
|| AttackState == State.ChargingHeavy
|| AttackState == State.ChargingMax;
    }

    public void Attack(bool andMove = false)
    {
        if (!CanRequestAttack)
            return;

        _attackRequested = true;
        if (_attackRequested && !_attackRequestedPreviousState)
        {
            _timeToChargeLightAttackLeft = timeToChargeLightAttack;
            _timeToChargeHeavyAttackLeft = timeToChargeHeavyAttack;
            _timeOfAttackLeft = timeOfAttack;
            _timeToNextAttackLeft = timeBetweenAttack;
        }
    }

    public void Cancel()
    {
        if (AttackState == State.ChargingLight || AttackState == State.ChargingHeavy || AttackState == State.ChargingMax)
            _cancelled = true;
    }

    private void Clear()
    {
        _cancelled = false;
        _timeToChargeLightAttackLeft = 0;
        _timeToChargeHeavyAttackLeft = 0;
        _timeOfAttackLeft = 0;
        AttackState = State.None;
    }

    public void Update()
    {
        switch (AttackState)
        {
            case State.ChargingLight:
                if (_cancelled)
                {
                    Clear();
                    _cancelled = false;
                    break;
                }

                if (_attackRequested != _attackRequestedPreviousState)
                {
                    AttackState = State.Attacking;
                    break;
                }

                _timeToChargeLightAttackLeft -= Time.deltaTime;
                if (_timeToChargeLightAttackLeft < 0)
                    AttackState = State.ChargingHeavy;
                break;

            case State.ChargingHeavy:
                if (_cancelled)
                {
                    Clear();
                    _cancelled = false;
                    break;
                }

                if (_attackRequested != _attackRequestedPreviousState)
                {
                    AttackState = State.Attacking;
                    break;
                }

                _timeToChargeHeavyAttackLeft -= Time.deltaTime;
                if (_timeToChargeHeavyAttackLeft < 0)
                    AttackState = State.ChargingMax;
                break;

            case State.ChargingMax:
                if (_cancelled)
                {
                    Clear();
                    _cancelled = false;
                    break;
                }

                if (_attackRequested != _attackRequestedPreviousState)
                {
                    AttackState = State.Attacking;
                    break;
                }

                break;

            case State.Attacking:
                _timeOfAttackLeft -= Time.deltaTime;
                if (_timeOfAttackLeft < 0)
                    AttackState = State.FinishedAttack;
                break;

            case State.FinishedAttack:
                FindAndDamage();
                Clear();
                break;

            case State.None:
                if (_attackRequested)
                    AttackState = State.ChargingLight;
                else if (_timeToNextAttackLeft > 0)
                    _timeToNextAttackLeft -= Time.deltaTime;
                break;
        }

        _attackRequestedPreviousState = _attackRequested;
        _attackRequested = false;
    }

    private void FindAndDamage()
    {
    }

    public enum State
    {
        ChargingLight, ChargingHeavy, ChargingMax, Attacking, FinishedAttack, None
    }
}