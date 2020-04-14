using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

[RequireComponent(typeof(CharacterController))]
public class HumanCharacter : MonoBehaviour
{
    public bool male = false;

    private Dictionary<string, SpriteAnimator> sprites = new Dictionary<string, SpriteAnimator>();

    public void Equip(Equipment equipment)
    {
        Unequip(equipment.type);

        GameObject animator = male ? equipment.maleSpriteSheet : equipment.femaleSpriteSheet;
        if (animator != null && animator.GetComponent<SpriteAnimator>() != null)
        {
            var item = Instantiate(animator);
            item.name = equipment.type.ToString().ToLower();
            item.transform.parent = transform;
            item.transform.localPosition = new Vector2();
            sprites.Add(item.name, item.GetComponent<SpriteAnimator>());
        }
    }

    public void Unequip(Equipment.Type type)
    {
        var name = type.ToString();

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

    public void SetAnimation(string animation)
    {
        foreach (var item in sprites)
        {
            item.Value.SetAnimation(animation);
        }
    }

    // Start is called before the first frame update
    private void Start()
    {
        var components = GetComponentsInChildren<SpriteAnimator>();
        foreach (var c in components)
            sprites.Add(c.name, c);
    }

    // Update is called once per frame
    private void Update()
    {
        var controller = GetComponent<CharacterController>();

        if (controller.IsRunning)
            SetAnimation("runww");
        else if (controller.IsWalking)
            SetAnimation("walk");
        // else
        //    SetAnimation("idle");

        if (Input.GetKey(KeyCode.Keypad0))
            SetAnimation("death");
        if (Input.GetKey(KeyCode.Keypad1))
            SetAnimation("attack1");
        if (Input.GetKey(KeyCode.Keypad2))
            SetAnimation("attack2");
        if (Input.GetKey(KeyCode.Keypad3))
            SetAnimation("block1");
        if (Input.GetKey(KeyCode.Keypad4))
            SetAnimation("block2");
        if (Input.GetKey(KeyCode.Keypad5))
            SetAnimation("run");
    }
}

[CustomEditor(typeof(HumanCharacter)), CanEditMultipleObjects]
internal class HumanCharacterEditor : Editor
{
    private Equipment.Type type;
    private HumanCharacter gameObject;

    private Equipment equipment;

    public void OnEnable()
    {
        gameObject = (serializedObject.targetObject as HumanCharacter);
    }

    public override void OnInspectorGUI()
    {
        DrawDefaultInspector();

        EditorGUILayout.Space();
        EditorGUILayout.LabelField("Custom editor");

        EditorGUILayout.BeginHorizontal();
        type = (Equipment.Type)EditorGUILayout.EnumPopup("InventorySlot", type);
        if (GUILayout.Button("Unequip"))
            gameObject.Unequip(type);
        EditorGUILayout.EndHorizontal();

        EditorGUILayout.Space();
        EditorGUILayout.BeginHorizontal();
        equipment = EditorGUILayout.ObjectField("Equipment", equipment, typeof(Equipment), true) as Equipment;
        if (GUILayout.Button("Equip"))
            gameObject.Equip(equipment);
        EditorGUILayout.EndHorizontal();
    }
}