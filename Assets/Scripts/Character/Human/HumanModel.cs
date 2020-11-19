using System;
using System.Linq;
using System.Collections;
using System.Collections.Generic;
using UnityEditor.Animations;
using UnityEngine;

[ExecuteAlways]
public class HumanModel : MonoBehaviour, IEquipInterceptor
{
    public bool male;
    public int headType;
    public int beardType;
    public int hairType;

    public int bodyType;
    public int legsType;

    public int bodyColor;
    public int hairColor;

    public HumanModelPart Head;
    public GameObject Hair;
    public GameObject Beard;
    public HumanModelPart Body;
    public HumanModelPart Legs;
    public HumanModelPart Arms;
    public GameObject Main;

    public HumanModelPrefabs malePrefabs;
    public HumanModelPrefabs femalePrefabs;

    private HumanModelPrefabs Prefabs { get => male ? malePrefabs : femalePrefabs; }

    private Dictionary<Equipment.Type, Equipment> equipment = new Dictionary<Equipment.Type, Equipment>();

    private UnityEngine.Animator mainAnimator;

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

    public bool IsShieldEquipped
    {
        get
        {
            return equipment.ContainsKey(Equipment.Type.OffHand);
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
            bool isArcher = equipment.ContainsKey(Equipment.Type.Bow);
            if (isArcher)
            {
                AimRotation = Mathf.Max(-60, Mathf.Min(attackController.attackAngle, 80));
                AimRotation -= AimRotation % 5;

                switch (attackController.AttackState)
                {
                    case CharacterHumanAttackController.State.ChargingLight:
                        mainAnimator.Play("Archer");
                        break;

                    case CharacterHumanAttackController.State.ChargingHeavy:
                    case CharacterHumanAttackController.State.ChargingMax:
                        mainAnimator.Play("ArcherIdle");
                        break;

                    case CharacterHumanAttackController.State.AttackingLight:
                    case CharacterHumanAttackController.State.AttackingHard:
                    case CharacterHumanAttackController.State.FinishedAttack:
                        mainAnimator.Play("Idle");
                        break;

                    default:
                        break;
                }
            }
            else
            {
                switch (attackController.AttackState)
                {
                    case CharacterHumanAttackController.State.ChargingLight:

                        if (controller.IsWalking || controller.IsRunning)
                            mainAnimator.Play("WalkAndPreAttack2");
                        else
                            mainAnimator.Play("PreAttack2");

                        break;

                    case CharacterHumanAttackController.State.ChargingHeavy:
                        if (controller.IsWalking || controller.IsRunning)
                            mainAnimator.Play("WalkAndPreAttack1");
                        else
                            mainAnimator.Play("PreAttack1");
                        break;

                    case CharacterHumanAttackController.State.ChargingMax:
                        if (controller.IsWalking || controller.IsRunning)
                            mainAnimator.Play("WalkAndPreAttack1");
                        else
                            mainAnimator.Play("PreAttack1");
                        break;

                    case CharacterHumanAttackController.State.AttackingLight:
                        mainAnimator.Play("Attack2");
                        controller.Stop();
                        break;

                    case CharacterHumanAttackController.State.AttackingHard:
                        mainAnimator.Play("Attack1");
                        controller.Stop();
                        break;

                    case CharacterHumanAttackController.State.FinishedAttack:
                        mainAnimator.Play("Idle");
                        break;

                    default:
                        break;
                }
            }
        }
        else if (blockController.IsPreparingToBlock)
        {
            char type = IsShieldEquipped ? (blockController.IsTargetingUpwards ? '3' : '1') : (IsWeaponEquipped ? '2' : '1');

            if (controller.IsWalking || controller.IsRunning)
                mainAnimator.Play("Walk");
            else
                mainAnimator.Play("Block" + type);
        }
        else if (blockController.IsBlocking)
        {
            char type = IsShieldEquipped ? (blockController.IsTargetingUpwards ? '3' : '1') : (IsWeaponEquipped ? '2' : '1');

            if (controller.IsWalking || controller.IsRunning)
                mainAnimator.Play("BlockAndWalk" + type);
            else
                mainAnimator.Play("BlockIdle" + type);
        }
        else if (controller.IsRunning)
        {
            if (IsWeaponEquipped)
                mainAnimator.Play("Run");
            else
                mainAnimator.Play("Runww");
        }
        else if (controller.IsWalking)
        {
            mainAnimator.Play("Walk");
        }
        else
        {
            mainAnimator.Play("Idle");
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
        mainAnimator = Main.GetComponent<UnityEngine.Animator>();

        Invalidate();
    }

    public void Invalidate()
    {
        mainAnimator.SetLayerWeight(1, male ? 0 : 1);

        InvalidateBodyPart(Head?.GetComponent<SwitchableTexture>(), Prefabs.Heads, headType);
        InvalidateBodyPart(Hair?.GetComponent<SwitchableTexture>(), Prefabs.Hairs, hairType);
        InvalidateBodyPart(Beard?.GetComponent<SwitchableTexture>(), Prefabs.Beards, beardType);
        InvalidateBodyPart(Body?.GetComponent<SwitchableTexture>(), Prefabs.Bodies, bodyType);
        InvalidateBodyPart(Arms?.GetComponent<SwitchableTexture>(), Prefabs.Bodies, bodyType);
        InvalidateBodyPart(Legs?.GetComponent<SwitchableTexture>(), Prefabs.Legs, legsType);

        InvalidateColors();

        foreach (Equipment.Type type in Enum.GetValues(typeof(Equipment.Type)))
        {
            InvalidateEquipment(type);
        }
    }

    public void InvalidateColors()
    {
        InvalidateColor(Hair, Prefabs.hairColors, hairColor);
        InvalidateColor(Beard, Prefabs.hairColors, hairColor);
        InvalidateColor(Legs.gameObject, Prefabs.bodyColors, bodyColor);
        InvalidateColor(Body.gameObject, Prefabs.bodyColors, bodyColor);
        InvalidateColor(Head.gameObject, Prefabs.bodyColors, bodyColor);
    }

    protected void InvalidateBodyPart(SwitchableTexture obj, List<SwitchableTextureData> textures, int id)
    {
        if (obj == null || textures == null || id < 0 || id > textures.Count)
            return;

        if (id == textures.Count)
        {
            obj.gameObject.SetActive(false);
            return;
        }
        else
            obj.gameObject.SetActive(true);

        var tex = textures[id];

        obj.texture = tex.main;
        obj.mask = tex.mask;
        obj.normalmap = tex.normal;
    }

    public void InvalidateEquipment(Equipment.Type type)
    {
        if (!Application.isPlaying)
            return;

        Head.RemoveAndDestroy(type.ToString());
        Body.RemoveAndDestroy(type.ToString());
        Legs.RemoveAndDestroy(type.ToString());
        Arms.RemoveAndDestroy(type.ToString());

        if (!equipment.ContainsKey(type))
            return;

        var item = equipment[type];

        var arms = male ? item.maleSpriteSheet_arms : item.femaleSpriteSheet_arms;
        if (arms)
        {
            var newItem = Instantiate(arms);
            Arms.Add(newItem);
            //  armsAnimators.Add(newItem.GetComponent<UnityEngine.Animator>());
        }

        var body = male ? item.maleSpriteSheet : item.femaleSpriteSheet;

        if (body)
        {
            var newItem = Instantiate(body);
            newItem.name = type.ToString();

            switch (type)
            {
                case Equipment.Type.MainHand:
                case Equipment.Type.OffHand:
                case Equipment.Type.TwoHanded:
                case Equipment.Type.Armor:
                case Equipment.Type.Necklace:
                case Equipment.Type.Ring:
                case Equipment.Type.Bow:
                    Body.Add(newItem);
                    break;

                case Equipment.Type.Helmet:
                    Head.Add(newItem);
                    break;

                case Equipment.Type.Boots:
                case Equipment.Type.Pants:
                    Legs.Add(newItem);
                    break;
            }
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
}