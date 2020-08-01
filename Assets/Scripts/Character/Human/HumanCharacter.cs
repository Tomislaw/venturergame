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
    public int hair;

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

    private Dictionary<Equipment.Type, Equipment> equipment = new Dictionary<Equipment.Type, Equipment>();

    public HumanCharacterBodyPrefabs prefabs;

    private Dictionary<string, (SimpleAnimator, HumanAnimationType.Type)> sprites = new Dictionary<string, (SimpleAnimator, HumanAnimationType.Type)>();

    private CharacterHumanAttackController.State previousAttackState = CharacterHumanAttackController.State.None;
    private State state;

    public void InvalidateBody()
    {
        if (prefabs == null)
        {
            Debug.LogWarning("Warning, " + name + " don't have HumanCharacterBodyPrefabs set");
            return;
        }
        var body = transform.Find("body")?.gameObject;
        var hair = transform.Find("hair")?.gameObject;
        var head = transform.Find("head")?.gameObject;
        var legs = transform.Find("legs")?.gameObject;

#if UNITY_EDITOR
        if (body)
            DestroyImmediate(body);
        if (hair)
            DestroyImmediate(hair);
        if (head)
            DestroyImmediate(head);
        if (legs)
            DestroyImmediate(legs);
#else
        if (body)
            Destroy(body);
        if (hair)
            Destroy(hair);
        if (head)
            Destroy(head);
        if (legs)
            Destroy(legs);
#endif

        if (male)
        {
            AddBodyPart(prefabs.maleBodies, this.body, "body");
            AddBodyPart(prefabs.maleHeads, this.head, "head");
            AddBodyPart(prefabs.maleHairs, this.hair, "hair");
            AddBodyPart(prefabs.maleLegs, this.legs, "legs");
        }
        else
        {
            AddBodyPart(prefabs.femaleBodies, this.body, "body");
            AddBodyPart(prefabs.femaleHeads, this.head, "head");
            AddBodyPart(prefabs.femaleHairs, this.hair, "hair");
            AddBodyPart(prefabs.femaleLegs, this.legs, "legs");
        }

        var components = GetComponentsInChildren<SimpleAnimator>();
        foreach (var c in components)
        {
            var type = c.GetComponent<HumanAnimationType>()?.AnimationType ?? HumanAnimationType.Type.Main;
            sprites[c.name] = (c, type);
        }

        StartCoroutine(SyncAnimation());
    }

    public void InvalidateEquipment()
    {
        foreach (var item in equipment.Keys)
        {
            var itemName = item.ToString().ToLower();
            var go = transform.Find(itemName)?.gameObject;

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
            if (animator != null && animator.GetComponent<SimpleAnimator>() != null)
            {
                var item = Instantiate(animator);
                item.name = equipment.type.ToString().ToLower();
                item.transform.SetParent(transform, false);
                var type = item.GetComponent<HumanAnimationType>()?.AnimationType ?? HumanAnimationType.Type.Main;
                sprites[item.name] = (item.GetComponent<SimpleAnimator>(), type);
            }
        }
    }

    public void Invalidate()
    {
        InvalidateBody();
        InvalidateEquipment();
    }

    private void AddBodyPart(List<GameObject> objects, int id, string name)
    {
        if (objects.Count <= id)
        {
            //Debug.LogWarning("Warning, " + name + "  using invalid prefab id");
        }
        else
        {
            var item = GameObject.Instantiate(objects[id]);
            item.name = name;
            item.transform.parent = transform;
            item.transform.localPosition = new Vector3();
            item.transform.localScale = new Vector3(1, 1, 1);
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

        GameObject animator = male ? equipment.maleSpriteSheet : equipment.femaleSpriteSheet;
        if (animator != null && animator.GetComponent<SimpleAnimator>() != null)
        {
            var item = Instantiate(animator);
            item.name = equipment.type.ToString().ToLower();
            item.transform.SetParent(transform, false);
            var type = item.GetComponent<HumanAnimationType>()?.AnimationType ?? HumanAnimationType.Type.Main;
            sprites[item.name] = (item.GetComponent<SimpleAnimator>(), type);
        }

        StartCoroutine(SyncAnimation());
    }

    public void Unequip(Equipment.Type type)
    {
        this.equipment.Remove(type);

#if UNITY_EDITOR
        if (!EditorApplication.isPlaying)
            return;
#endif

        var name = type.ToString().ToLower();
        sprites.Remove(name);
        var item = transform.Find(name);
        if (item != null)
            Destroy(item.gameObject);
    }

    private IEnumerator SyncAnimation()
    {
        yield return null;
        SimpleAnimator first = null;
        foreach (var item in sprites)
        {
            if (first == null)
                first = item.Value.Item1;
            else
            {
                item.Value.Item1.Animation = first.Animation;
                item.Value.Item1.Sync(first);
            }
        }
        yield return null;
    }

    // Start is called before the first frame update
    private void Start()
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
        foreach (var item in sprites)
        {
            var sprite = item.Value.Item1;
            if (item.Value.Item2 == HumanAnimationType.Type.Legs)
            {
                switch (state)
                {
                    case State.PreWeakAttack:
                    case State.PreStrongAttack:
                        if (sprite.Animation != "preattack")
                            sprite.Animation = "preattack";
                        break;

                    case State.StrongAttack:
                    case State.WeakAttack:
                        if (sprite.Animation != "attack")
                            sprite.Animation = "attack";
                        break;

                    case State.Idle:
                        sprite.Animation = "idle";
                        break;

                    case State.Walk:
                    case State.BlockAndWalk:
                    case State.BlockUpAndWalk:
                        sprite.Animation = "walk";
                        break;

                    case State.Run:
                        sprite.Animation = "run";
                        break;

                    case State.PreBlock:
                        sprite.Animation = "block";
                        break;

                    case State.Block:
                    case State.BlockUp:
                        sprite.Animation = "blockIdle";
                        break;

                    case State.Death:
                        sprite.Animation = "death";
                        break;
                }
            }
            else
            {
                switch (state)
                {
                    case State.PreWeakAttack:
                        if (sprite.Animation != "preattack2")
                            sprite.Animation = "preattack2";
                        break;

                    case State.WeakAttack:
                        if (sprite.Animation != "attack2")
                            sprite.Animation = "attack2";
                        break;

                    case State.PreStrongAttack:
                        if (sprite.Animation != "preattack1")
                            sprite.Animation = "preattack1";
                        break;

                    case State.StrongAttack:
                        if (sprite.Animation != "attack1")
                            sprite.Animation = "attack1";
                        break;

                    case State.Idle:
                        sprite.Animation = "idle";
                        break;

                    case State.Walk:
                        sprite.Animation = "walk";
                        break;

                    case State.Run:
                        if (IsWeaponEquipped)
                            sprite.Animation = "run";
                        else
                            sprite.Animation = "runww";
                        break;

                    case State.PreBlock:
                        int typePreBlock = BlockingType;
                        if (typePreBlock == 0)
                            sprite.Animation = "block1";
                        else
                            sprite.Animation = "block2";
                        break;

                    case State.Block:
                        int typeBlock = BlockingType;
                        if (typeBlock == 0)
                            sprite.Animation = "block1Idle";
                        else
                            sprite.Animation = "block2Idle";
                        break;

                    case State.BlockUp:
                        sprite.Animation = "block3Idle";
                        break;

                    case State.BlockAndWalk:
                        int typeBlockAndWalk = BlockingType;
                        if (typeBlockAndWalk == 0)
                            sprite.Animation = "block1Idle";
                        else
                            sprite.Animation = "block2Idle";
                        break;

                    case State.BlockUpAndWalk:
                        sprite.Animation = "block3Idle";
                        break;

                    case State.Death:
                        sprite.Animation = "death";
                        break;
                }
            }
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
    public int f_head;
    public int f_body;
    public int f_hair;
    public int f_legs;
    public bool gender;

    public bool updated = false;

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
        }
        else
        {
            f_head = gameObject.head;
            f_hair = gameObject.hair;
            f_body = gameObject.body;
            f_legs = gameObject.legs;
        }
    }

    public override void OnInspectorGUI()
    {
        if (gameObject == null)
            return;

        EditorGUILayout.PropertyField(serializedObject.FindProperty("prefabs"));

        if (gameObject.prefabs == null)
        {
            serializedObject.ApplyModifiedProperties();
            return;
        }

        var g = EditorGUILayout.Toggle("Is male", gender);
        updated |= g != gender; gender = g;

        if (!gender)
        {
            var head = EditorGUILayout.Popup("female head", f_head, gameObject.prefabs.femaleHeads.Select(it => it.name).ToArray());
            var body = EditorGUILayout.Popup("female body", f_body, gameObject.prefabs.femaleBodies.Select(it => it.name).ToArray());
            var hair = EditorGUILayout.Popup("female hair", f_hair, gameObject.prefabs.femaleHairs.Select(it => it.name).ToArray());
            var legs = EditorGUILayout.Popup("female legs", f_legs, gameObject.prefabs.femaleLegs.Select(it => it.name).ToArray());
            updated |= f_head != head || f_body != body || f_hair != hair || f_hair != legs;
            f_head = head; f_body = body; f_hair = hair; f_legs = legs;
        }
        else
        {
            var head = EditorGUILayout.Popup("male head", m_head, gameObject.prefabs.maleHeads.Select(it => it.name).ToArray());
            var body = EditorGUILayout.Popup("male body", m_body, gameObject.prefabs.maleBodies.Select(it => it.name).ToArray());
            var hair = EditorGUILayout.Popup("male hair", m_hair, gameObject.prefabs.maleHairs.Select(it => it.name).ToArray());
            var legs = EditorGUILayout.Popup("male legs", m_legs, gameObject.prefabs.maleLegs.Select(it => it.name).ToArray());
            updated |= m_head != head || m_body != body || m_hair != hair || m_hair != legs;
            m_head = head; m_body = body; m_hair = hair; m_legs = legs;
        }

        serializedObject.FindProperty("male").boolValue = gender;
        if (!gender)
        {
            serializedObject.FindProperty("head").intValue = f_head;
            serializedObject.FindProperty("body").intValue = f_body;
            serializedObject.FindProperty("hair").intValue = f_hair;
            serializedObject.FindProperty("legs").intValue = f_legs;
        }
        else
        {
            serializedObject.FindProperty("head").intValue = m_head;
            serializedObject.FindProperty("body").intValue = m_body;
            serializedObject.FindProperty("hair").intValue = m_hair;
            serializedObject.FindProperty("legs").intValue = m_legs;
        }
        serializedObject.ApplyModifiedProperties();

        if (updated)
            gameObject.Invalidate();
        updated = false;
    }
}

#endif