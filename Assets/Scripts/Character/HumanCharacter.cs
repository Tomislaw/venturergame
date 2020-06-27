using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using System.Linq;

[RequireComponent(typeof(CharacterMovementController))]
public class HumanCharacter : MonoBehaviour
{
    public bool male = false;
    public int body;
    public int head;
    public int hair;

    private Dictionary<Equipment.Type, Equipment> equipment = new Dictionary<Equipment.Type, Equipment>();

    public HumanCharacterBodyPrefabs prefabs;

    private Dictionary<string, SimpleAnimator> sprites = new Dictionary<string, SimpleAnimator>();

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
#if UNITY_EDITOR
        if (body)
            DestroyImmediate(body);
        if (hair)
            DestroyImmediate(hair);
        if (head)
            DestroyImmediate(head);
#else
        if (body)
            Destroy(body);
        if (hair)
            Destroy(hair);
        if (head)
            Destroy(head);
#endif

        if (male)
        {
            AddBodyPart(prefabs.maleBodies, this.body, "body");
            AddBodyPart(prefabs.maleHeads, this.head, "head");
            AddBodyPart(prefabs.maleHairs, this.hair, "hair");
        }
        else
        {
            AddBodyPart(prefabs.femaleBodies, this.body, "body");
            AddBodyPart(prefabs.femaleHeads, this.head, "head");
            AddBodyPart(prefabs.femaleHairs, this.hair, "hair");
        }

        var components = GetComponentsInChildren<SimpleAnimator>();
        foreach (var c in components)
            sprites[c.name] = c;

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
                sprites[item.name] = item.GetComponent<SimpleAnimator>();
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

        GameObject animator = male ? equipment.maleSpriteSheet : equipment.femaleSpriteSheet;
        if (animator != null && animator.GetComponent<SimpleAnimator>() != null)
        {
            var item = Instantiate(animator);
            item.name = equipment.type.ToString().ToLower();
            item.transform.SetParent(transform, false);
            sprites[item.name] = item.GetComponent<SimpleAnimator>();
        }

        StartCoroutine(SyncAnimation());
    }

    public void Unequip(Equipment.Type type)
    {
        this.equipment.Remove(type);

        var name = type.ToString().ToLower();

        sprites.Remove(name);
        Transform item = transform.Find(name);
        if (item != null)
        {
#if UNITY_EDITOR
            DestroyImmediate(item.gameObject);
#else
            Destroy(item.gameObject);
#endif
        }
    }

    private IEnumerator SyncAnimation()
    {
        yield return null;
        SimpleAnimator first = null;
        foreach (var item in sprites)
        {
            if (first == null)
                first = item.Value;
            else
            {
                item.Value.Animation = first.Animation;
                item.Value.Sync(first);
            }
        }
        yield return null;
    }

    public void SetAnimation(string animation)
    {
        foreach (var item in sprites)
            item.Value.Animation = animation;
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
        var attackController = GetComponent<CharacterBasicAttackController>();
        var inventoryController = GetComponent<CharacterInventoryController>();
        var blockController = GetComponent<CharacterBlockComponent>();

        if (controller.IsRunning)
        {
            if (inventoryController && inventoryController.GetEquippedWeapon())
                SetAnimation("run");
            else
                SetAnimation("runww");
            attackController.CancelAttack();
            blockController.StopBlocking();
        }
        else if (controller.IsWalking)
        {
            SetAnimation("walk");
            attackController.CancelAttack();
            blockController.StopBlocking();
        }
        else if (attackController.IsStartedAttacking)
        {
            SetAnimation("attack" + Random.Range(1, 3).ToString());
        }
        else if (attackController.IsAttacking)
        {
        }
        else if (blockController.IsPreparingToBlock)
        {
            if (inventoryController)
            {
                if (inventoryController.GetEquippedOffhand())
                    SetAnimation("block1");
                else if (inventoryController.GetEquippedWeapon())
                    SetAnimation("block2");
                else
                    SetAnimation("block1");
            }
            else
            {
                SetAnimation("block1");
            }
        }
        else if (blockController.IsBlocking)
        {
        }
        else
        {
            SetAnimation("idle");
        }
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
    public int f_head;
    public int f_body;
    public int f_hair;
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
        }
        else
        {
            f_head = gameObject.head;
            f_hair = gameObject.hair;
            f_body = gameObject.body;
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
            updated |= f_head != head || f_body != body || f_hair != hair;
            f_head = head; f_body = body; f_hair = hair;
        }
        else
        {
            var head = EditorGUILayout.Popup("male head", m_head, gameObject.prefabs.maleHeads.Select(it => it.name).ToArray());
            var body = EditorGUILayout.Popup("male body", m_body, gameObject.prefabs.maleBodies.Select(it => it.name).ToArray());
            var hair = EditorGUILayout.Popup("male hair", m_hair, gameObject.prefabs.maleHairs.Select(it => it.name).ToArray());
            updated |= m_head != head || m_body != body || m_hair != hair;
            f_head = m_head; m_body = body; m_hair = hair;
        }

        serializedObject.FindProperty("male").boolValue = gender;
        if (!gender)
        {
            serializedObject.FindProperty("head").intValue = f_head;
            serializedObject.FindProperty("body").intValue = f_body;
            serializedObject.FindProperty("hair").intValue = f_hair;
        }
        else
        {
            serializedObject.FindProperty("head").intValue = m_head;
            serializedObject.FindProperty("body").intValue = m_body;
            serializedObject.FindProperty("hair").intValue = m_hair;
        }
        serializedObject.ApplyModifiedProperties();

        if (updated)
            gameObject.Invalidate();
        updated = false;
    }
}

#endif