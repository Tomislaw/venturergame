using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using System.Linq;

[RequireComponent(typeof(CharacterMovementController))]
public class HumanCharacter : MonoBehaviour
{
    public bool male = false;
    public int legs;
    public int body;
    public int head;
    public int beard;
    public int hair;
    public int arms;

    public int hairColor;
    public int bodyColor;

    internal Color _hairColor { get => hairColor > 0 || hairColor < prefabs.hairColors.Length ? prefabs.hairColors[hairColor] : Color.white; }
    internal Color _bodyColor { get => bodyColor > 0 || hairColor < prefabs.bodyColors.Length ? prefabs.bodyColors[bodyColor] : Color.white; }

    public GameObject Head { get; private set; } = null;
    public GameObject Body { get; private set; } = null;
    public GameObject Legs { get; private set; } = null;
    public GameObject Arms { get; private set; } = null;

    public HumanCharacterBodyPrefabs prefabs;

    private Dictionary<Equipment.Type, Equipment> equipment = new Dictionary<Equipment.Type, Equipment>();
    private Dictionary<string, GameObject> parts = new Dictionary<string, GameObject>();

    private List<Animator> bodyAnimators = new List<Animator>();
    private List<Animator> legsAnimators = new List<Animator>();

    private CharacterHumanAttackController.State previousAttackState = CharacterHumanAttackController.State.None;
    private State state;

    [Range(-80, 80)]
    public int AimRotation = 0;

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

    private enum AttackType { Bow, TwoHanded, OneHanded }

    private BlockType BlockingType
    {
        get
        {
            bool haveMainHand = false;
            foreach (var item in equipment)
            {
                if (item.Key == Equipment.Type.TwoHanded)
                    return BlockType.TwoHanded;
                else if (item.Key == Equipment.Type.OffHand)
                    return BlockType.WithShield;
                else if (item.Key == Equipment.Type.MainHand)
                    haveMainHand = true;
            }
            return haveMainHand ? BlockType.OneHanded : BlockType.WithShield;
        }
    }

    private enum BlockType { TwoHanded, WithShield, OneHanded }

    public void InvalidateBody()
    {
        InvalidateBodyPart("Body");
        InvalidateBodyPart("Head");
        InvalidateBodyPart("Legs");
        InvalidateBodyPart("Arms");
        InvalidateBodyPart("Hair");
        InvalidateBodyPart("Beard");
    }

    public void InvalidateBodyPart(string name)
    {
        // return if no prefabs data set
        if (prefabs == null)
        {
            Debug.LogWarning("Warning, " + name + " don't have HumanCharacterBodyPrefabs set");
            return;
        }

        // which body type is selected
        GameObject parent = this.gameObject;
        int type = -1;
        switch (name)
        {
            case "Head":
                type = this.head;
                break;

            case "Body":
                type = this.body;
                break;

            case "Arms":
                type = this.arms;
                break;

            case "Legs":
                type = this.legs;
                break;

            case "Hair":
                parent = Head;
                type = this.hair;
                break;

            case "Beard":
                parent = Head;
                type = this.beard;
                break;

            default:
                return;
        }

        // remove from list before adding new one
        RemoveAndDestroy(name);

        //get diffirent prefab based on gender
        string gender = male ? "male" : "female";

        var bodyObjects = prefabs.Prefabs[gender + name];

        if (bodyObjects.Count <= type || type < 0)
        {
            // dont add if nothing found
            //Debug.LogWarning("Warning, " + name + "  using invalid prefab id");
        }
        else
        {
            var item = GameObject.Instantiate(bodyObjects[type]);
            item.name = name;
            item.transform.SetParent(parent.transform, false);

            AddToAnimators(item);

            switch (name)
            {
                case "Head":
                    this.Head = item;
                    break;

                case "Body":
                    this.Body = item;
                    break;

                case "Legs":
                    this.Legs = item;
                    break;

                case "Arms":
                    this.Arms = item;
                    this.Arms.SetActive(false);
                    break;
            }
        }
    }

    private void RemoveAndDestroy(string go)
    {
        if (parts.ContainsKey(go))
        {
            var part = parts[go];
            parts.Remove(go);
            RemoveFromAnimators(part);
            Destroy(part);
        }
    }

    private void RemoveFromAnimators(GameObject go)
    {
        var type = go.GetComponent<HumanAnimationType>();

        if (type == null)
            return;

        var anims = go.GetComponents<Animator>();

        if (type.AnimationType == HumanAnimationType.Type.Legs)
            legsAnimators.RemoveAll(it => anims.Contains(it));
        else
            bodyAnimators.RemoveAll(it => anims.Contains(it));
    }

    private void AddToAnimators(GameObject go)
    {
        var type = go.GetComponent<HumanAnimationType>();

        if (type == null)
            return;

        var anims = go.GetComponentsInChildren<Animator>(true);

        if (type.AnimationType == HumanAnimationType.Type.Legs)
            legsAnimators.AddRange(anims);
        else
            bodyAnimators.AddRange(anims);
    }

    public void InvalidateEquipment()
    {
        foreach (var equip in equipment.Values)
            Equip(equip);
    }

    public void InvalidateColors()
    {
        var sprites = GetComponentsInChildren<SpriteAnimator>();
        foreach (var item in sprites)
        {
            var name = item.gameObject.name;
            if (name == "Beard" || name == "Hair")
                item.GetComponent<SpriteRenderer>().color = _hairColor;
            else if (name == "Head" || name == "Body" || name == "Legs" || name == "Arms")
                item.GetComponent<SpriteRenderer>().color = _bodyColor;
        }
    }

    public void Invalidate()
    {
        InvalidateBody();
        InvalidateEquipment();
        InvalidateColors();
        SyncAnimation();
    }

    public void Equip(Equipment equipment)
    {
        Unequip(equipment.type);
        this.equipment[equipment.type] = equipment;

#if UNITY_EDITOR
        if (!EditorApplication.isPlaying)
            return;
#endif

        GameObject parent = this.gameObject;

        switch (equipment.type)
        {
            case Equipment.Type.MainHand:
            case Equipment.Type.OffHand:
            case Equipment.Type.TwoHanded:
            case Equipment.Type.Armor:
            case Equipment.Type.Necklace:
            case Equipment.Type.Ring:
            case Equipment.Type.Bow:
                parent = Body;
                break;

            case Equipment.Type.Helmet:
                parent = Head;

                break;

            case Equipment.Type.Boots:
            case Equipment.Type.Pants:
                parent = Legs;
                break;
        }

        GameObject animator = male ? equipment.maleSpriteSheet : equipment.femaleSpriteSheet;
        if (animator != null)
        {
            var item = Instantiate(animator);
            item.name = equipment.type.ToString().ToLower();
            item.transform.SetParent(parent.transform, false);
            parts[item.name] = item;
            AddToAnimators(item);
        }

        GameObject animatorArms = male ? equipment.maleSpriteSheet_arms : equipment.femaleSpriteSheet_arms;
        if (animatorArms != null)
        {
            var item = Instantiate(animatorArms);
            item.name = equipment.type.ToString().ToLower() + "_arms";
            item.transform.SetParent(Arms.transform, false);
            parts[item.name] = item;
            AddToAnimators(item);
        }

        SyncAnimation();
    }

    public void Unequip(Equipment.Type type)
    {
        this.equipment.Remove(type);

#if UNITY_EDITOR
        if (!EditorApplication.isPlaying)
            return;
#endif

        var name = type.ToString().ToLower();
        RemoveAndDestroy(name);

        var nameArms = name + "_arms";
        RemoveAndDestroy(nameArms);
    }

    public void SyncAnimation()
    {
        Animator firstBody = null;
        foreach (var item in bodyAnimators)
        {
            if (firstBody == null)
            {
                firstBody = item;
                continue;
            }
            item.Sync(firstBody);
        }

        Animator firstLegs = null;
        foreach (var item in legsAnimators)
        {
            if (firstLegs == null)
            {
                firstLegs = item;
                continue;
            }
            item.Sync(firstLegs);
        }
    }

    // Start is called before the first frame update
    private void Awake()
    {
        Invalidate();
    }

    // Update is called once per frame
    private void Update()
    {
        var controller = GetComponent<CharacterMovementController>();
        var attackController = GetComponent<CharacterHumanAttackController>();
        var inventoryController = GetComponent<CharacterInventoryController>();
        var blockController = GetComponent<CharacterBlockComponent>();

        AimRotation = 0;

        if (attackController.AttackState != CharacterHumanAttackController.State.None)
        {
            AimRotation = Mathf.Max(-60, Mathf.Min(attackController.attackAngle, 80));
            AimRotation -= AimRotation % 5;
            switch (attackController.AttackState)
            {
                case CharacterHumanAttackController.State.ChargingLight:
                    SetState(State.PreWeakAttack);
                    previousAttackState = attackController.AttackState;
                    break;

                case CharacterHumanAttackController.State.ChargingHeavy:
                    SetState(State.PreStrongAttack);
                    previousAttackState = attackController.AttackState;
                    break;

                case CharacterHumanAttackController.State.ChargingMax:
                    if (previousAttackState != CharacterHumanAttackController.State.ChargingMax)
                    {
                        // play some nice effect here
                    }
                    SetState(State.PreStrongAttack);
                    previousAttackState = attackController.AttackState;
                    break;

                case CharacterHumanAttackController.State.AttackingHard:
                case CharacterHumanAttackController.State.AttackingLight:
                    if (previousAttackState == CharacterHumanAttackController.State.ChargingHeavy
                        || previousAttackState == CharacterHumanAttackController.State.ChargingMax)
                        SetState(State.StrongAttack);
                    else
                        SetState(State.WeakAttack);
                    break;

                case CharacterHumanAttackController.State.FinishedAttack:
                    SetState(State.Idle);
                    previousAttackState = attackController.AttackState;
                    break;

                default:
                    break;
            }
        }
        else if (controller.IsRunning)
        {
            SetState(State.Run);
            attackController.Cancel();
            blockController.StopBlocking();
        }
        else if (controller.IsWalking)
        {
            SetState(State.Walk);
            attackController.Cancel();
            blockController.StopBlocking();
        }
        else if (blockController.IsPreparingToBlock)
        {
            SetState(State.Block);
        }
        else if (blockController.IsBlocking)
        {
        }
        else
        {
            SetState(State.Idle);
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

    private void SetState(State state)
    {
        this.state = state;

        var legsAnnimation = "idle";
        var mainAnimation = "idle";
        var restartIfSame = true;

        var showArms = false;

        // legs
        switch (state)
        {
            case State.PreWeakAttack:
            case State.PreStrongAttack:
                showArms = true;
                legsAnnimation = "preattack";
                break;

            case State.StrongAttack:
            case State.WeakAttack:
                showArms = true;
                if (WeaponType == AttackType.Bow)
                    legsAnnimation = "preattack";
                else
                    legsAnnimation = "attack";
                restartIfSame = false;
                break;

            case State.Idle:
                legsAnnimation = "idle";
                break;

            case State.Walk:
            case State.BlockAndWalk:
            case State.BlockUpAndWalk:
                legsAnnimation = "walk";
                break;

            case State.Run:
                legsAnnimation = "run";
                break;

            case State.PreBlock:
                legsAnnimation = "block";
                break;

            case State.Block:
            case State.BlockUp:
                legsAnnimation = "blockIdle";
                break;

            case State.Death:
                legsAnnimation = "death";
                break;
        }

        // main
        switch (state)
        {
            case State.PreWeakAttack:
                if (WeaponType == AttackType.Bow)
                    mainAnimation = "archer";
                else
                    mainAnimation = "preattack2";

                break;

            case State.WeakAttack:
                if (WeaponType == AttackType.Bow)
                    mainAnimation = "archer";
                else
                    mainAnimation = "attack2";

                break;

            case State.PreStrongAttack:
                if (WeaponType == AttackType.Bow)
                {
                    mainAnimation = "archer";
                    showArms = true;
                }
                else
                {
                    mainAnimation = "preattack1";
                }

                break;

            case State.StrongAttack:
                if (WeaponType == AttackType.Bow)
                {
                    mainAnimation = "archer";
                    showArms = true;
                }
                else
                    mainAnimation = "attack1";
                break;

            case State.Idle:
                mainAnimation = "idle";
                break;

            case State.Walk:
                mainAnimation = "walk";
                break;

            case State.Run:
                if (IsWeaponEquipped)
                    mainAnimation = "run";
                else
                    mainAnimation = "runww";
                break;

            case State.PreBlock:
                var typePreBlock = BlockingType;
                if (typePreBlock == BlockType.WithShield)
                    mainAnimation = "block1";
                else
                    mainAnimation = "block2";
                break;

            case State.Block:
            case State.BlockAndWalk:
                var typeBlock = BlockingType;
                if (typeBlock == BlockType.WithShield)
                    mainAnimation = "block1Idle";
                else
                    mainAnimation = "block2Idle";
                break;

            case State.BlockUp:
                mainAnimation = "block3Idle";
                break;

            case State.BlockUpAndWalk:
                mainAnimation = "block3Idle";
                break;

            case State.Death:
                mainAnimation = "death";
                break;
        }

        if (Arms.activeSelf != showArms)
            Arms.SetActive(showArms);

        foreach (var animator in legsAnimators)
            animator.SetAnimation(legsAnnimation, restartIfSame);

        foreach (var animator in bodyAnimators)
            animator.SetAnimation(mainAnimation, restartIfSame);
    }

    public enum State
    {
        StrongAttack, PreStrongAttack,
        WeakAttack, PreWeakAttack,
        Idle,
        Walk,
        Run,
        PreBlock, Block, BlockUp, BlockAndWalk, BlockUpAndWalk,
        Death,
    }
}

#if UNITY_EDITOR

[CustomEditor(typeof(HumanCharacter)), CanEditMultipleObjects]
internal class HumanCharacterEditor : Editor
{
    public HumanCharacter gameObject;
    public int m_head;
    public int m_body;
    public int m_hair;
    public int m_legs;
    public int m_beard;
    public int f_head;
    public int f_body;
    public int f_hair;
    public int f_legs;
    public int f_beard;
    public int f_arms;
    public int m_arms;
    public int hairColor = 0;
    public int bodyColor = 0;
    public bool gender;

    public bool updated = false;

    private bool showCharData = true;

    public void OnEnable()
    {
        gameObject = (serializedObject.targetObject as HumanCharacter);
        gender = gameObject.male;
        if (gameObject.male)
        {
            m_head = gameObject.head;
            m_hair = gameObject.hair;
            m_body = gameObject.body;
            m_legs = gameObject.legs;
            m_beard = gameObject.beard;
            m_arms = gameObject.arms;
        }
        else
        {
            f_head = gameObject.head;
            f_hair = gameObject.hair;
            f_body = gameObject.body;
            f_legs = gameObject.legs;
            f_beard = gameObject.beard;
            f_arms = gameObject.arms;
        }
        hairColor = gameObject.hairColor;
        bodyColor = gameObject.bodyColor;
    }

    public override void OnInspectorGUI()
    {
        if (gameObject == null)
            return;

        EditorGUILayout.PropertyField(serializedObject.FindProperty("prefabs"));
        EditorGUILayout.PropertyField(serializedObject.FindProperty("AimRotation"));
        if (gameObject.prefabs == null)
        {
            serializedObject.ApplyModifiedProperties();
            return;
        }

        showCharData = EditorGUILayout.Foldout(showCharData, "Character data");
        if (showCharData)
        {
            EditorGUI.indentLevel++;

            var g = EditorGUILayout.Toggle("Is male", gender);
            updated |= g != gender; gender = g;

            if (!gender)
            {
                var head = EditorGUILayout.Popup("female head", f_head, gameObject.prefabs.Prefabs["femaleHead"].Select(it => it.name).ToArray());
                var body = EditorGUILayout.Popup("female body", f_body, gameObject.prefabs.Prefabs["femaleBody"].Select(it => it.name).ToArray());
                var hair = EditorGUILayout.Popup("female hair", f_hair, gameObject.prefabs.Prefabs["femaleHair"].Select(it => it.name).ToArray());
                var legs = EditorGUILayout.Popup("female legs", f_legs, gameObject.prefabs.Prefabs["femaleLegs"].Select(it => it.name).ToArray());
                var arms = EditorGUILayout.Popup("female arms", f_legs, gameObject.prefabs.Prefabs["femaleArms"].Select(it => it.name).ToArray());
                var beard = EditorGUILayout.Popup("female beards", f_beard, gameObject.prefabs.Prefabs["femaleBeard"].Select(it => it.name).ToArray());
                updated |= f_head != head || f_body != body || f_hair != hair || f_hair != legs || f_beard != beard || f_arms != arms;
                f_head = head; f_body = body; f_hair = hair; f_legs = legs; f_beard = beard; f_arms = arms;
            }
            else
            {
                var head = EditorGUILayout.Popup("male head", m_head, gameObject.prefabs.Prefabs["maleHead"].Select(it => it.name).ToArray());
                var body = EditorGUILayout.Popup("male body", m_body, gameObject.prefabs.Prefabs["maleBody"].Select(it => it.name).ToArray());
                var hair = EditorGUILayout.Popup("male hair", m_hair, gameObject.prefabs.Prefabs["maleHair"].Select(it => it.name).ToArray());
                var legs = EditorGUILayout.Popup("male legs", m_legs, gameObject.prefabs.Prefabs["maleLegs"].Select(it => it.name).ToArray());
                var arms = EditorGUILayout.Popup("male arms", f_legs, gameObject.prefabs.Prefabs["maleArms"].Select(it => it.name).ToArray());
                var beard = EditorGUILayout.Popup("male beards", m_beard, gameObject.prefabs.Prefabs["maleBeard"].Select(it => it.name).ToArray());
                updated |= m_head != head || m_body != body || m_hair != hair || m_hair != legs || m_beard != beard || m_arms != arms;
                m_head = head; m_body = body; m_hair = hair; m_legs = legs; m_beard = beard; m_arms = arms;
            }

            hairColor = EditorGUILayout.IntSlider("hair color", hairColor, 0, gameObject.prefabs.hairColors.Length - 1);
            bodyColor = EditorGUILayout.IntSlider("body color", bodyColor, 0, gameObject.prefabs.bodyColors.Length - 1);

            bool colorUpdated = hairColor != gameObject.hairColor || bodyColor != gameObject.bodyColor;

            serializedObject.FindProperty("male").boolValue = gender;
            if (!gender)
            {
                serializedObject.FindProperty("head").intValue = f_head;
                serializedObject.FindProperty("body").intValue = f_body;
                serializedObject.FindProperty("hair").intValue = f_hair;
                serializedObject.FindProperty("legs").intValue = f_legs;
                serializedObject.FindProperty("beard").intValue = f_beard;
                serializedObject.FindProperty("arms").intValue = f_arms;
            }
            else
            {
                serializedObject.FindProperty("head").intValue = m_head;
                serializedObject.FindProperty("body").intValue = m_body;
                serializedObject.FindProperty("hair").intValue = m_hair;
                serializedObject.FindProperty("legs").intValue = m_legs;
                serializedObject.FindProperty("beard").intValue = m_beard;
                serializedObject.FindProperty("arms").intValue = m_arms;
            }
            serializedObject.FindProperty("hairColor").intValue = hairColor;
            serializedObject.FindProperty("bodyColor").intValue = bodyColor;
            serializedObject.ApplyModifiedProperties();

            if (updated && Application.isPlaying)
                gameObject.Invalidate();
            updated = false;

            if (colorUpdated && Application.isPlaying)
            {
                var sprites = gameObject.GetComponentsInChildren<SpriteRenderer>();
                foreach (var sprite in sprites)
                {
                    var name = sprite.gameObject.name;
                    if (name == "beard" || name == "hair")
                        sprite.color = gameObject._hairColor;
                    else if (name == "head" || name == "body" || name == "legs" || name == "arms")
                        sprite.color = gameObject._bodyColor;
                }
            }
            EditorGUI.indentLevel--;
        }
    }
}

#endif