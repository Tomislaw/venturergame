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

    public int hairColor;
    public int bodyColor;

    internal Color _hairColor { get => hairColor > 0 || hairColor < prefabs.hairColors.Length ? prefabs.hairColors[hairColor] : Color.white; }
    internal Color _bodyColor { get => bodyColor > 0 || hairColor < prefabs.bodyColors.Length ? prefabs.bodyColors[bodyColor] : Color.white; }

    public GameObject Head;
    public GameObject Body;
    public GameObject Legs;

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

    private int BlockingType
    {
        get
        {
            bool haveMainHand = false;
            foreach (var item in equipment)
            {
                if (item.Key == Equipment.Type.TwoHanded)
                    return 2;
                else if (item.Key == Equipment.Type.OffHand)
                    return 0;
                else if (item.Key == Equipment.Type.MainHand)
                    haveMainHand = true;
            }
            return haveMainHand ? 1 : 0;
        }
    }

    public HumanCharacterBodyPrefabs prefabs;

    private Dictionary<Equipment.Type, Equipment> equipment = new Dictionary<Equipment.Type, Equipment>();
    private Dictionary<string, (Animator, HumanAnimationType.Type)> sprites = new Dictionary<string, (Animator, HumanAnimationType.Type)>();

    private CharacterHumanAttackController.State previousAttackState = CharacterHumanAttackController.State.None;
    private State state;

    public void InvalidateBody()
    {
        InvalidateBodyPart("Body");
        InvalidateBodyPart("Head");
        InvalidateBodyPart("Hair");
        InvalidateBodyPart("Legs");
        InvalidateBodyPart("Beard");
    }

    public void InvalidateBodyPart(string name)
    {
        if (prefabs == null)
        {
            Debug.LogWarning("Warning, " + name + " don't have HumanCharacterBodyPrefabs set");
            return;
        }

        GameObject parent = this.gameObject;

        int type = -1;
        switch (name)
        {
            case "Head":
                type = this.head;
                parent = Head;
                break;

            case "Body":
                parent = Body;
                type = this.body;
                break;

            case "Beard":
                parent = Head;
                type = this.beard;
                break;

            case "Legs":
                parent = Legs;
                type = this.legs;
                break;

            case "Hair":
                parent = Head;
                type = this.hair;
                break;

            default:
                return;
        }

        if (sprites.ContainsKey(name))
        {
            var part = sprites[name].Item1?.gameObject;
            if (part)
                Destroy(part);

            sprites.Remove(name);
        }

        string gender = male ? "male" : "female";
        AddBodyPart(prefabs.Prefabs[gender + name], type, name, parent.transform);
    }

    public void InvalidateEquipment()
    {
        foreach (var item in equipment.Keys)
        {
            var itemName = item.ToString().ToLower();
            var go = sprites[itemName].Item1?.gameObject;
            if (go)
            {
#if UNITY_EDITOR
                DestroyImmediate(go);
#else
                Destroy(go);
#endif
            }
        }

        foreach (var equipment in equipment.Values)
        {
            GameObject animator = male ? equipment.maleSpriteSheet : equipment.femaleSpriteSheet;
            if (animator != null && animator.GetComponent<SpriteAnimator>() != null)
            {
                var item = Instantiate(animator);
                item.name = equipment.type.ToString().ToLower();
                item.transform.SetParent(transform, false);
                var type = item.GetComponent<HumanAnimationType>()?.AnimationType ?? HumanAnimationType.Type.Main;
                sprites[item.name] = (item.GetComponent<SpriteAnimator>(), type);
            }
        }
    }

    public void InvalidateColors()
    {
        var sprites = GetComponentsInChildren<SpriteAnimator>();
        foreach (var item in sprites)
        {
            var name = item.gameObject.name;
            if (name == "Beard" || name == "Hair")
                item.GetComponent<SpriteRenderer>().color = _hairColor;
            else if (name == "Head" || name == "Body")
                item.GetComponent<SpriteRenderer>().color = _bodyColor;
        }
    }

    public void Invalidate()
    {
        foreach (var item in GetComponentsInChildren<HumanAnimationType>())
        {
            var animator = item.GetComponent<Animator>();
            if (animator != null)
                sprites[animator.gameObject.name] = (animator, item.AnimationType);
        }
        InvalidateBody();
        InvalidateEquipment();
        InvalidateColors();
        SyncAnimation();
    }

    private void AddBodyPart(List<GameObject> objects, int id, string name, Transform parent)
    {
        if (objects.Count <= id || id < 0)
        {
            //Debug.LogWarning("Warning, " + name + "  using invalid prefab id");
        }
        else
        {
            var item = GameObject.Instantiate(objects[id]);
            item.name = name;
            item.transform.SetParent(parent, false);
            sprites[item.name] = (item.GetComponent<SpriteAnimator>(), item.GetComponent<HumanAnimationType>().AnimationType);
        }
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
        if (animator != null && animator.GetComponent<SpriteAnimator>() != null)
        {
            var item = Instantiate(animator);
            item.name = equipment.type.ToString().ToLower();
            item.transform.SetParent(parent.transform, false);
            var type = item.GetComponent<HumanAnimationType>()?.AnimationType ?? HumanAnimationType.Type.Main;
            sprites[item.name] = (item.GetComponent<SpriteAnimator>(), type);
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
        if (sprites.ContainsKey(name))
        {
            var item = sprites[name].Item1?.gameObject;
            if (item)
                Destroy(item);
            sprites.Remove(name);
        }
    }

    public void SyncAnimation()
    {
        Animator firstBody = null;
        Animator firstLegs = null;
        foreach (var item in sprites)
        {
            if (firstBody == null && item.Value.Item2 == HumanAnimationType.Type.Main)
            {
                firstBody = item.Value.Item1;
                continue;
            }

            if (firstLegs == null && item.Value.Item2 == HumanAnimationType.Type.Legs)
            {
                firstLegs = item.Value.Item1;
                continue;
            }

            item.Value.Item1.Sync(item.Value.Item2 == HumanAnimationType.Type.Main ? firstBody : firstLegs);
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

        if (attackController.AttackState != CharacterHumanAttackController.State.None)
        {
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

                case CharacterHumanAttackController.State.Attacking:
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
    }

    private void SetState(State state)
    {
        this.state = state;

        var legsAnnimation = "idle";
        var mainAnimation = "idle";
        var restartIfSame = true;
        // legs
        switch (state)
        {
            case State.PreWeakAttack:
            case State.PreStrongAttack:
                legsAnnimation = "preattack";
                break;

            case State.StrongAttack:
            case State.WeakAttack:
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
                mainAnimation = "preattack2";
                break;

            case State.WeakAttack:
                mainAnimation = "attack2";
                break;

            case State.PreStrongAttack:
                mainAnimation = "preattack1";
                break;

            case State.StrongAttack:
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
                int typePreBlock = BlockingType;
                if (typePreBlock == 0)
                    mainAnimation = "block1";
                else
                    mainAnimation = "block2";
                break;

            case State.Block:
                int typeBlock = BlockingType;
                if (typeBlock == 0)
                    mainAnimation = "block1Idle";
                else
                    mainAnimation = "block2Idle";
                break;

            case State.BlockUp:
                mainAnimation = "block3Idle";
                break;

            case State.BlockAndWalk:
                int typeBlockAndWalk = BlockingType;
                if (typeBlockAndWalk == 0)
                    mainAnimation = "block1Idle";
                else
                    mainAnimation = "block2Idle";
                break;

            case State.BlockUpAndWalk:
                mainAnimation = "block3Idle";
                break;

            case State.Death:
                mainAnimation = "death";
                break;
        }

        foreach (var item in sprites)
        {
            var sprite = item.Value.Item1;
            if (item.Value.Item2 == HumanAnimationType.Type.Legs)
                item.Value.Item1.SetAnimation(legsAnnimation, restartIfSame);
            else
                item.Value.Item1.SetAnimation(mainAnimation, restartIfSame);
        }
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
        }
        else
        {
            f_head = gameObject.head;
            f_hair = gameObject.hair;
            f_body = gameObject.body;
            f_legs = gameObject.legs;
            f_beard = gameObject.beard;
        }
        hairColor = gameObject.hairColor;
        bodyColor = gameObject.bodyColor;
    }

    public override void OnInspectorGUI()
    {
        if (gameObject == null)
            return;

        EditorGUILayout.PropertyField(serializedObject.FindProperty("prefabs"));
        EditorGUILayout.PropertyField(serializedObject.FindProperty("Head"));
        EditorGUILayout.PropertyField(serializedObject.FindProperty("Body"));
        EditorGUILayout.PropertyField(serializedObject.FindProperty("Legs"));

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
                var beard = EditorGUILayout.Popup("female beards", f_beard, gameObject.prefabs.Prefabs["femaleBeard"].Select(it => it.name).ToArray());
                updated |= f_head != head || f_body != body || f_hair != hair || f_hair != legs || f_beard != beard; ;
                f_head = head; f_body = body; f_hair = hair; f_legs = legs; f_beard = beard;
            }
            else
            {
                var head = EditorGUILayout.Popup("male head", m_head, gameObject.prefabs.Prefabs["maleHead"].Select(it => it.name).ToArray());
                var body = EditorGUILayout.Popup("male body", m_body, gameObject.prefabs.Prefabs["maleBody"].Select(it => it.name).ToArray());
                var hair = EditorGUILayout.Popup("male hair", m_hair, gameObject.prefabs.Prefabs["maleHair"].Select(it => it.name).ToArray());
                var legs = EditorGUILayout.Popup("male legs", m_legs, gameObject.prefabs.Prefabs["maleLegs"].Select(it => it.name).ToArray());
                var beard = EditorGUILayout.Popup("male beards", m_beard, gameObject.prefabs.Prefabs["maleBeard"].Select(it => it.name).ToArray());
                updated |= m_head != head || m_body != body || m_hair != hair || m_hair != legs || m_beard != beard;
                m_head = head; m_body = body; m_hair = hair; m_legs = legs; m_beard = beard;
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
            }
            else
            {
                serializedObject.FindProperty("head").intValue = m_head;
                serializedObject.FindProperty("body").intValue = m_body;
                serializedObject.FindProperty("hair").intValue = m_hair;
                serializedObject.FindProperty("legs").intValue = m_legs;
                serializedObject.FindProperty("beard").intValue = m_beard;
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
                    else if (name == "head" || name == "body" || name == "body")
                        sprite.color = gameObject._bodyColor;
                }
            }
            EditorGUI.indentLevel--;
        }
    }
}

#endif