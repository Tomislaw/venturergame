using System;
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

    private int _attackAngle = 0;

    public int attackAngle
    {
        get => _attackAngle;
        set
        {
            _attackAngle = Mathf.Min(Mathf.Max(minAngle, value), maxAngle);
        }
    }

    public int maxAngle = 80;
    public int minAngle = -65;

    private float _timeToChargeLightAttackLeft = 0;
    private float _timeToChargeHeavyAttackLeft = 0;
    private float _timeOfAttackLeft = 0;
    private float _timeToNextAttackLeft = 0;
    private bool _attackRequested = false;
    private bool _attackRequestedPreviousState = false;
    private bool _cancelled = false;

    private CharacterMovementController movementController = null;
    private CharacterStatisticsController statistics = null;
    private CharacterInventoryController inventory = null;
    private HumanCharacter character = null;
    private Equipment _equippedWeapon = null;

    public State AttackState { get; private set; } = State.None;

    public bool CanRequestAttack
    {
        get => (AttackState == State.None && _timeToNextAttackLeft <= 0)
|| AttackState == State.ChargingLight
|| AttackState == State.ChargingHeavy
|| AttackState == State.ChargingMax;
    }

    private void OnEnable()
    {
        statistics = GetComponent<CharacterStatisticsController>();
        inventory = GetComponent<CharacterInventoryController>();
        character = GetComponent<HumanCharacter>();
        movementController = GetComponent<CharacterMovementController>();
    }

    public void Attack(bool andMove = false)
    {
        if (!CanRequestAttack)
            return;

        _attackRequested = true;
        if (_attackRequested && !_attackRequestedPreviousState)
        {
            _equippedWeapon = inventory ? inventory.GetEquippedWeapon() : null;

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
                    AttackState = State.AttackingLight;
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
                    AttackState = State.AttackingHard;
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
                    AttackState = State.AttackingHard;
                    break;
                }

                break;

            case State.AttackingLight:
            case State.AttackingHard:
                _timeOfAttackLeft -= Time.deltaTime;

                if (_equippedWeapon?.type == Equipment.Type.Bow)
                {
                    Shoot();
                    _timeOfAttackLeft = 0;
                }

                if (_timeOfAttackLeft <= 0)
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

    private void Shoot()
    {
        if (_equippedWeapon?.projectile == null)
            return;

        var pos = character.Arms.transform.position;
        var projectile = Instantiate(_equippedWeapon.projectile);
        projectile.transform.position = pos;
        projectile.name = gameObject.name + "_projectile";

        var angle = attackAngle;
        if (movementController.FaceLeft)
            angle = 180 - angle;
        projectile.Shoot(gameObject, angle, 10f);
    }

    private void FindAndDamage()
    {
        float weaponRange = 1;
        var colliders = Physics2D.OverlapCircleAll(transform.position, weaponRange);

        foreach (var collider in colliders)
        {
            var damageable = collider.GetComponent<Damageable>();
            if (!damageable)
                continue;
            if (!movementController)
            {
                damageable.Damage(gameObject, 3);
            }
            else
            {
                if (movementController.FaceLeft && collider.transform.position.x > transform.position.x)
                    continue;
                if (!movementController.FaceLeft && collider.transform.position.x < transform.position.x)
                    continue;
                damageable.Damage(gameObject, 4);
            }
        }
    }

    public enum State
    {
        ChargingLight, ChargingHeavy, ChargingMax, AttackingLight, AttackingHard, FinishedAttack, None
    }
}