using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using System.Linq;

[RequireComponent(typeof(Inventory))]
public class CharacterInventoryController : MonoBehaviour
{
    public List<Equipment> equipped = new List<Equipment>();

    public void Start()
    {
        var human = GetComponent<HumanCharacter>();
        if (human)
            foreach (var item in equipped)
            {
                human.Equip(item);
            }
    }

    public Equipment GetEquippedWeapon()
    {
        return equipped.Find(it => it.type == Equipment.Type.MainHand || it.type == Equipment.Type.TwoHanded);
    }

    public Equipment GetEquippedOffhand()
    {
        return equipped.Find(it => it.type == Equipment.Type.OffHand);
    }

    private Equipment GetEquippedItem(Equipment.Type type)
    {
        foreach (var item in equipped)
        {
            if (item.type == type)
                return item;
        }
        return null;
    }

    private void SetEquippedItem(Equipment equip)
    {
        equipped.Add(equip);
    }

    private void RemoveEquippedItem(Equipment.Type type)
    {
        equipped.RemoveAll(it => it.type == type
      || (type == Equipment.Type.TwoHanded && (it.type == Equipment.Type.MainHand || it.type == Equipment.Type.OffHand)));
    }

    public bool EquipFromInventory(InventoryItem inventoryItem)
    {
        var equipment = inventoryItem.item as Equipment;
        if (equipment == false)
            return false;

        UnequipAndPutToInventory(equipment.type);
        SetEquippedItem(equipment);

        var human = GetComponent<HumanCharacter>();
        switch (equipment.type)
        {
            case Equipment.Type.MainHand:
            case Equipment.Type.OffHand:
            case Equipment.Type.Helmet:
            case Equipment.Type.Armor:
            case Equipment.Type.Boots:
            case Equipment.Type.Pants:
                if (human != null)
                    human.Equip(equipment);
                break;

            case Equipment.Type.TwoHanded:
                if (human != null)
                {
                    human.Unequip(Equipment.Type.MainHand);
                    human.Unequip(Equipment.Type.OffHand);
                }
                human.Equip(equipment);
                break;

            case Equipment.Type.Necklace:
            case Equipment.Type.Ring:
                break;
        }

        var inventory = GetComponent<Inventory>();
        if (inventory != null && inventory.ContainsItem(inventoryItem))
        {
            inventory.TakeItem(inventoryItem);
        }
        return true;
    }

    public Equipment Equip(InventoryItem inventoryItem)
    {
        var equipment = inventoryItem.item as Equipment;
        if (equipment == false)
            return null;

        var current = Unequip(equipment.type);
        SetEquippedItem(equipment);

        var human = GetComponent<HumanCharacter>();
        switch (equipment.type)
        {
            case Equipment.Type.MainHand:
            case Equipment.Type.OffHand:
            case Equipment.Type.Helmet:
            case Equipment.Type.Armor:
            case Equipment.Type.Boots:
            case Equipment.Type.Pants:
                if (human != null)
                    human.Equip(equipment);
                break;

            case Equipment.Type.TwoHanded:
                if (human != null)
                {
                    human.Unequip(Equipment.Type.MainHand);
                    human.Unequip(Equipment.Type.OffHand);
                }
                human.Equip(equipment);
                break;

            case Equipment.Type.Necklace:
            case Equipment.Type.Ring:
                break;
        }

        return current;
    }

    public Equipment Unequip(Equipment.Type type)
    {
        var human = GetComponent<HumanCharacter>();
        var item = GetEquippedItem(type);

        if (item == null)
            return null;

        RemoveEquippedItem(type);

        switch (type)
        {
            case Equipment.Type.MainHand:
            case Equipment.Type.OffHand:
            case Equipment.Type.Helmet:
            case Equipment.Type.Armor:
            case Equipment.Type.Boots:
            case Equipment.Type.Pants:
                if (human != null)
                    human.Unequip(type);
                break;

            case Equipment.Type.TwoHanded:
                if (human != null)
                {
                    human.Unequip(Equipment.Type.MainHand);
                    human.Unequip(Equipment.Type.OffHand);
                }
                human.Unequip(type);
                break;

            case Equipment.Type.Necklace:
            case Equipment.Type.Ring:
                break;
        }

        return item;
    }

    public void UnequipAndPutToInventory(Equipment.Type type)
    {
        var human = GetComponent<HumanCharacter>();
        var item = GetEquippedItem(type);

        if (item == null)
            return;

        RemoveEquippedItem(type);

        switch (type)
        {
            case Equipment.Type.MainHand:
            case Equipment.Type.OffHand:
            case Equipment.Type.Helmet:
            case Equipment.Type.Armor:
            case Equipment.Type.Boots:
            case Equipment.Type.Pants:
                if (human != null)
                    human.Unequip(type);
                break;

            case Equipment.Type.TwoHanded:
                if (human != null)
                {
                    human.Unequip(Equipment.Type.MainHand);
                    human.Unequip(Equipment.Type.OffHand);
                }
                human.Unequip(type);
                break;

            case Equipment.Type.Necklace:
            case Equipment.Type.Ring:
                break;
        }

        var inventory = GetComponent<Inventory>();
        if (inventory != null)
        {
            inventory.PutItem(new InventoryItem { item = item, count = 1 });
        }
    }
}

#if UNITY_EDITOR

[CustomEditor(typeof(CharacterInventoryController)), CanEditMultipleObjects]
internal class CharacterInventoryControllerEditor : Editor
{
    private Equipment.Type type;
    private CharacterInventoryController gameObject;

    private List<Equipment> equipment;

    private Vector2 scroll;

    public void OnEnable()
    {
        gameObject = (serializedObject.targetObject as CharacterInventoryController);
    }

    public override void OnInspectorGUI()
    {
        DrawDefaultInspector();

        if (gameObject == null)
            return;

        var inventory = gameObject.GetComponent<Inventory>();
        if (inventory == null)
            return;

        EditorGUILayout.Space();
        EditorGUILayout.LabelField("Custom editor");

        EditorGUILayout.Space();
        EditorGUILayout.LabelField("Current inventory");
        foreach (var item in gameObject.equipped)
            EditorGUILayout.LabelField(item.ToString());
        EditorGUILayout.BeginHorizontal();
        type = (Equipment.Type)EditorGUILayout.EnumPopup("InventorySlot", type);
        if (GUILayout.Button("Unequip"))
            gameObject.UnequipAndPutToInventory(type);
        EditorGUILayout.EndHorizontal();

        EditorGUILayout.Space();
        EditorGUILayout.BeginHorizontal();

        scroll = EditorGUILayout.BeginScrollView(scroll);

        foreach (var item in inventory.GetItems())
        {
            var eqiupment = item.item as Equipment;
            if (eqiupment != null)
            {
                if (GUILayout.Button(eqiupment.ToString()))
                    gameObject.EquipFromInventory(item);
            }
        }
        EditorGUILayout.EndScrollView();

        EditorGUILayout.EndHorizontal();
    }
}

#endif