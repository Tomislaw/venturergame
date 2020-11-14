using System;
using System.Linq;
using System.Collections;
using System.Collections.Generic;
using UnityEditor.Animations;
using UnityEngine;

[ExecuteAlways]
public class HumanModel : MonoBehaviour
{
    public bool male;
    public int headType;
    public int beardType;
    public int hairType;

    public int bodyType;
    public int legsType;

    public int bodyColor;
    public int hairColor;

    public GameObject Head;
    public GameObject Hair;
    public GameObject Beard;
    public GameObject Body;
    public GameObject Legs;
    public GameObject Arms;
    public GameObject Main;

    public HumanModelPrefabs malePrefabs;
    public HumanModelPrefabs femalePrefabs;

    private HumanModelPrefabs Prefabs { get => male ? malePrefabs : femalePrefabs; }

    private Dictionary<Equipment.Type, Equipment> equipment = new Dictionary<Equipment.Type, Equipment>();

    private UnityEngine.Animator mainAnimator;
    private HashSet<UnityEngine.Animator> headAnimators = new HashSet<UnityEngine.Animator>();
    private HashSet<UnityEngine.Animator> bodyAnimators = new HashSet<UnityEngine.Animator>();
    private HashSet<UnityEngine.Animator> legsAnimators = new HashSet<UnityEngine.Animator>();
    private HashSet<UnityEngine.Animator> armsAnimators = new HashSet<UnityEngine.Animator>();

    public string debugAnim = "idle";
    private string currentAnimation = "idle";

    public bool IsWeaponEquipped
    {
        get
        {
            if (equipment == null || equipment.Count == 0)
                return false;

            foreach (var item in equipment)
                if (item.Key.IsWeapon())
                    return true;

            return false;
        }
    }

    private enum AttackType { Bow, TwoHanded, OneHanded }

    private AttackType WeaponType
    {
        get
        {
            foreach (var item in equipment)
            {
                if (item.Key == Equipment.Type.Bow)
                    return AttackType.Bow;
                if (item.Key == Equipment.Type.TwoHanded)
                    return AttackType.TwoHanded;
                else if (item.Key == Equipment.Type.MainHand)
                    return AttackType.OneHanded;
            }
            return AttackType.OneHanded;
        }
    }

    private void Update()
    {
        var controller = GetComponent<CharacterMovementController>();
        var attackController = GetComponent<CharacterHumanAttackController>();
        var inventoryController = GetComponent<CharacterInventoryController>();
        var blockController = GetComponent<CharacterBlockComponent>();

        var AimRotation = 0;

        if (attackController.AttackState != CharacterHumanAttackController.State.None)
        {
            AimRotation = Mathf.Max(-60, Mathf.Min(attackController.attackAngle, 80));
            AimRotation -= AimRotation % 5;
            switch (attackController.AttackState)
            {
                case CharacterHumanAttackController.State.ChargingLight:
                    SetAnimation("preattack2");
                    break;

                case CharacterHumanAttackController.State.ChargingHeavy:
                    SetAnimation("preattack1");
                    break;

                case CharacterHumanAttackController.State.ChargingMax:
                    SetAnimation("preattack1");
                    break;

                case CharacterHumanAttackController.State.AttackingLight:
                    SetAnimation("attack2");
                    break;

                case CharacterHumanAttackController.State.AttackingHard:
                    SetAnimation("attack1");
                    break;

                case CharacterHumanAttackController.State.FinishedAttack:
                    SetAnimation("idle");
                    break;

                default:
                    break;
            }
        }
        else if (controller.IsRunning)
        {
            if (IsWeaponEquipped)
                SetAnimation("run");
            else
                SetAnimation("runww");

            attackController.Cancel();
            blockController.StopBlocking();
        }
        else if (controller.IsWalking)
        {
            SetAnimation("walk");
            attackController.Cancel();
            blockController.StopBlocking();
        }
        else if (blockController.IsPreparingToBlock)
        {
            SetAnimation("block");
        }
        else if (blockController.IsBlocking)
        {
            SetAnimation("block");
        }
        else
        {
            SetAnimation("idle");
        }

        Head.transform.localEulerAngles =
            new Vector3(
                Head.transform.eulerAngles.x,
                Head.transform.eulerAngles.y,
                AimRotation
        );

        Arms.transform.localEulerAngles =
            new Vector3(
                Arms.transform.eulerAngles.x,
                Arms.transform.eulerAngles.y,
                AimRotation
        );

        transform.localEulerAngles =
            new Vector3(
                transform.eulerAngles.x,
                transform.eulerAngles.y,
                AimRotation < 0 ? AimRotation * Mathf.Sign(transform.localScale.x) / 10 : 0
        );
    }

    private void Start()
    {
        headAnimators.Add(Head.GetComponent<UnityEngine.Animator>());
        headAnimators.Add(Hair.GetComponent<UnityEngine.Animator>());
        headAnimators.Add(Beard.GetComponent<UnityEngine.Animator>());
        bodyAnimators.Add(Body.GetComponent<UnityEngine.Animator>());
        legsAnimators.Add(Legs.GetComponent<UnityEngine.Animator>());
        armsAnimators.Add(Arms.GetComponent<UnityEngine.Animator>());
        mainAnimator = Main.GetComponent<UnityEngine.Animator>();

        Invalidate();
    }

    public void Invalidate()
    {
        InvalidateBodyPart(Head?.GetComponent<SwitchableTexture>(), Prefabs.Heads, headType);
        InvalidateBodyPart(Hair?.GetComponent<SwitchableTexture>(), Prefabs.Hairs, hairType);
        InvalidateBodyPart(Beard?.GetComponent<SwitchableTexture>(), Prefabs.Beards, beardType);
        InvalidateBodyPart(Body?.GetComponent<SwitchableTexture>(), Prefabs.Bodies, bodyType);
        InvalidateBodyPart(Arms?.GetComponent<SwitchableTexture>(), Prefabs.Bodies, bodyType);
        InvalidateBodyPart(Legs?.GetComponent<SwitchableTexture>(), Prefabs.Legs, legsType);

        InvalidateColor(Hair, Prefabs.hairColors, hairColor);
        InvalidateColor(Beard, Prefabs.hairColors, hairColor);
        InvalidateColor(Legs, Prefabs.bodyColors, bodyColor);
        InvalidateColor(Body, Prefabs.bodyColors, bodyColor);
        InvalidateColor(Head, Prefabs.bodyColors, bodyColor);

        foreach (Equipment.Type type in Enum.GetValues(typeof(Equipment.Type)))
        {
            InvalidateEquipment(type);
        }
    }

    protected void InvalidateBodyPart(SwitchableTexture obj, List<SwitchableTextureData> textures, int id)
    {
        if (obj == null || textures == null || id < 0 || id > textures.Count)
            return;

        var tex = textures[id];

        obj.texture = tex.main;
        obj.mask = tex.mask;
        obj.normalmap = tex.normal;
    }

    public void InvalidateEquipment(Equipment.Type type)
    {
        if (Application.isEditor)
            return;

        var obj = Arms.transform.Find(type.ToString())?.gameObject;
        if (obj != null)
            Destroy(obj);

        obj = Head.transform.Find(type.ToString())?.gameObject;
        if (obj != null)
            Destroy(obj);

        obj = Body.transform.Find(type.ToString())?.gameObject;
        if (obj != null)
            Destroy(obj);

        obj = Legs.transform.Find(type.ToString())?.gameObject;
        if (obj != null)
            Destroy(obj);

        if (!equipment.ContainsKey(type))
            return;

        var item = equipment[type];

        var arms = male ? item.maleSpriteSheet_arms : item.femaleSpriteSheet_arms;
        if (arms)
        {
            var newItem = Instantiate(arms);
            newItem.transform.SetParent(Arms.transform, false);
            newItem.name = type.ToString();
            armsAnimators.Add(newItem.GetComponent<UnityEngine.Animator>());
        }

        var body = male ? item.maleSpriteSheet : item.femaleSpriteSheet;

        if (body)
        {
            var newItem = Instantiate(arms);
            newItem.name = type.ToString();
            GameObject target = gameObject;

            switch (type)
            {
                case Equipment.Type.MainHand:
                case Equipment.Type.OffHand:
                case Equipment.Type.TwoHanded:
                case Equipment.Type.Armor:
                case Equipment.Type.Necklace:
                case Equipment.Type.Ring:
                case Equipment.Type.Bow:
                    target = Body;
                    bodyAnimators.Add(newItem.GetComponent<UnityEngine.Animator>());
                    break;

                case Equipment.Type.Helmet:
                    target = Head;
                    headAnimators.Add(newItem.GetComponent<UnityEngine.Animator>());
                    break;

                case Equipment.Type.Boots:
                case Equipment.Type.Pants:
                    target = Legs;
                    legsAnimators.Add(newItem.GetComponent<UnityEngine.Animator>());
                    break;
            }

            newItem.transform.SetParent(target.transform, false);
        }
    }

    public void Equip(Equipment item)
    {
        equipment[item.type] = item;
        InvalidateEquipment(item.type);
    }

    public void Unequip(Equipment item)
    {
        equipment[item.type] = null;
        InvalidateEquipment(item.type);
    }

    public void Unequip(Equipment.Type type)
    {
        equipment[type] = null;
        InvalidateEquipment(type);
    }

    protected void InvalidateColor(GameObject obj, Color[] colors, int id)
    {
        if (obj == null || colors == null || id < 0 || id > colors.Length)
            return;

        var sr = obj.GetComponent<SpriteRenderer>();
        if (sr == null)
            return;

        sr.color = colors[id];
    }

    public void SetAnimation(string animation)
    {
        debugAnim = animation;

        switch (animation)
        {
            case "idle":

                if (currentAnimation == animation)
                    return;

                mainAnimator.Play("Idle", male ? 0 : 1);
                foreach (var anim in legsAnimators)
                    anim.Play("Idle");
                foreach (var anim in bodyAnimators)
                    anim.Play("Idle");
                foreach (var anim in headAnimators)
                    anim.Play("Idle");
                break;

            case "walk":

                if (currentAnimation == animation)
                    return;

                mainAnimator.Play("Walk", male ? 0 : 1);
                foreach (var anim in legsAnimators)
                    anim.Play("Walk");
                foreach (var anim in bodyAnimators)
                    anim.Play("Walk");
                foreach (var anim in headAnimators)
                    anim.Play("Idle");
                break;

            case "run":

                if (currentAnimation == animation)
                    return;

                mainAnimator.Play("Run", male ? 0 : 1);
                foreach (var anim in legsAnimators)
                    anim.Play("Run");
                foreach (var anim in bodyAnimators)
                    anim.Play("Run");
                foreach (var anim in headAnimators)
                    anim.Play("Idle");
                break;

            case "runww":

                if (currentAnimation == animation)
                    return;

                mainAnimator.Play("Runww", male ? 0 : 1);
                foreach (var anim in legsAnimators)
                    anim.Play("Run");
                foreach (var anim in bodyAnimators)
                    anim.Play("Runww");
                foreach (var anim in headAnimators)
                    anim.Play("Idle");
                break;

            case "attack1":

                if (currentAnimation == animation)
                    return;

                mainAnimator.Play("Attack1", male ? 0 : 1);
                foreach (var anim in legsAnimators)
                    anim.Play("Attack");
                foreach (var anim in bodyAnimators)
                    anim.Play("Attack1");
                foreach (var anim in headAnimators)
                    anim.Play("Idle");
                break;

            case "preattack1":

                if (currentAnimation == animation)
                    return;

                mainAnimator.Play("PreAttack1", male ? 0 : 1);
                foreach (var anim in legsAnimators)
                    anim.Play("BlockIdle");
                foreach (var anim in bodyAnimators)
                    anim.Play("PreAttack1");
                foreach (var anim in headAnimators)
                    anim.Play("Idle");
                break;

            case "attack2":

                if (currentAnimation == animation)
                    return;

                mainAnimator.Play("Attack2", male ? 0 : 1);
                foreach (var anim in legsAnimators)
                    anim.Play("Attack");
                foreach (var anim in bodyAnimators)
                    anim.Play("Attack2");
                foreach (var anim in headAnimators)
                    anim.Play("Idle");
                break;

            case "preattack2":

                if (currentAnimation == animation)
                    return;

                mainAnimator.Play("PreAttack2", male ? 0 : 1);
                foreach (var anim in legsAnimators)
                    anim.Play("BlockIdle");
                foreach (var anim in bodyAnimators)
                    anim.Play("PreAttack2");
                foreach (var anim in headAnimators)
                    anim.Play("Idle");
                break;

            case "block1":
                mainAnimator.Play("Block", male ? 0 : 1);
                foreach (var anim in legsAnimators)
                    anim.Play("Block");
                foreach (var anim in bodyAnimators)
                    anim.Play("Block1");
                foreach (var anim in headAnimators)
                    anim.Play("Idle");
                break;

            case "block2":
                mainAnimator.Play("Block", male ? 0 : 1);
                foreach (var anim in legsAnimators)
                    anim.Play("Block");
                foreach (var anim in bodyAnimators)
                    anim.Play("Block2");
                foreach (var anim in headAnimators)
                    anim.Play("Idle");
                break;

            case "blockidle1":

                if (currentAnimation == animation)
                    return;

                mainAnimator.Play("BlockIdle", male ? 0 : 1);
                foreach (var anim in legsAnimators)
                    anim.Play("BlockIdle");
                foreach (var anim in bodyAnimators)
                    anim.Play("BlockIdle1");
                foreach (var anim in headAnimators)
                    anim.Play("Idle");
                break;

            case "blockidle2":

                if (currentAnimation == animation)
                    return;

                mainAnimator.Play("BlockIdle", male ? 0 : 1);
                foreach (var anim in legsAnimators)
                    anim.Play("Block");
                foreach (var anim in bodyAnimators)
                    anim.Play("BlockIdle2");
                foreach (var anim in headAnimators)
                    anim.Play("Idle");
                break;

            case "blockidle3":

                if (currentAnimation == animation)
                    return;

                mainAnimator.Play("BlockIdle", male ? 0 : 1);
                foreach (var anim in legsAnimators)
                    anim.Play("Block");
                foreach (var anim in bodyAnimators)
                    anim.Play("BlockIdle3");
                foreach (var anim in headAnimators)
                    anim.Play("Idle");
                break;

            case "death":
                mainAnimator.Play("Death", male ? 0 : 1);
                foreach (var anim in legsAnimators)
                    anim.Play("Death");
                foreach (var anim in bodyAnimators)
                    anim.Play("Death");
                foreach (var anim in headAnimators)
                    anim.Play("Death");
                break;

            case "archer":

                if (currentAnimation == animation)
                    return;

                mainAnimator.Play("Archer", male ? 0 : 1);
                foreach (var anim in legsAnimators)
                    anim.Play("BlockIdle");
                foreach (var anim in bodyAnimators)
                    anim.Play("Archer");
                foreach (var anim in headAnimators)
                    anim.Play("Idle");
                foreach (var anim in armsAnimators)
                    anim.Play("Idle");
                break;

            case "archerfire":
                mainAnimator.Play("Archer", male ? 0 : 1);
                foreach (var anim in legsAnimators)
                    anim.Play("Block");
                foreach (var anim in bodyAnimators)
                    anim.Play("Archer");
                foreach (var anim in headAnimators)
                    anim.Play("Idle");
                foreach (var anim in armsAnimators)
                    anim.Play("Fire");
                break;
        }

        currentAnimation = animation;
    }
}