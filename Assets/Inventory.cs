using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEngine;

[System.Serializable]
public class InventoryItem
{
    public int count;
    public Item item;

    public bool IsStackable { get => item.MaxCountPerStack > 1; }
}

[System.Serializable]
public struct InventoryPair
{
    public InventoryItem item;
    public Vector2Int pos;
}

public class Inventory : MonoBehaviour
{
    public Vector2Int size;

    [SerializeField]
    public List<InventoryPair> items = new List<InventoryPair>();

    private void Start()
    {
    }

    public bool PutItem(InventoryItem item, Vector2Int slot)
    {
        if (!CanPlaceItemHere(item, slot))
            return false;

        items.Add(new InventoryPair { pos = slot, item = item });

        return true;
    }

    public bool PutItem(InventoryItem item)
    {
        if (item == null)
            return false;
        for (int x = 0; x < size.x; x++)
            for (int y = 0; y < size.y; y++)
                if (CanPlaceItemHere(item, new Vector2Int(x, y)))
                {
                    items.Add(new InventoryPair { pos = new Vector2Int(x, y), item = item });
                    return true;
                }
        return false;
    }

    public InventoryItem TakeItem(Vector2Int slot, int count = 1)
    {
        var item = items.Find(it => it.pos == slot);

        if (item.item == null)
            return null;

        if (!item.item.IsStackable || item.item.count <= count)
        {
            items.Remove(item);
            return item.item;
        }
        else
        {
            item.item.count -= count;
            var newObj = item.item;
            newObj.count = count;
            return newObj;
        }
    }

    public InventoryItem TakeItem(InventoryItem item, int count = 1)
    {
        if (item == null)
            return null;

        var invitem = items.Find(it => it.item.item == item.item);
        if (invitem.item == null)
            return null;

        return TakeItem(invitem.pos, count);
    }

    public bool IsSlotTaken(Vector2Int slot)
    {
        if (slot.x < 0 || slot.x >= size.x || slot.y < 0 || slot.y >= size.y)
            return true;
        return GetItem(slot) != null;
    }

    public bool CanPlaceItemHere(InventoryItem item, Vector2Int slot)
    {
        if (slot.x < 0 || slot.x >= size.x || slot.y < 0 || slot.y >= size.y)
            return false;
        for (int x = slot.x; x < item.item.Size.x + slot.x; x++)
        {
            for (int y = slot.y; y < item.item.Size.y + slot.y; y++)
            {
                if (IsSlotTaken(new Vector2Int(x, y)))
                    return false;
            }
        }
        return true;
    }

    public InventoryItem GetItem(Vector2Int slot)
    {
        foreach (var item in items)
        {
            var rect = new RectInt(item.pos, item.item.item.Size);
            if (rect.Contains(slot))
                return item.item;
        }
        return null;
    }

    public List<InventoryItem> GetItems()
    {
        return items.Select(it => it.item).ToList();
    }
}

#if UNITY_EDITOR

[CustomEditor(typeof(Inventory)), CanEditMultipleObjects]
internal class InventoryEditor : Editor
{
    private Inventory gameObject;
    private Item addItem;
    private int addItemCount = 1;
    private bool folded = false;

    public void OnEnable()
    {
        gameObject = (serializedObject.targetObject as Inventory);
    }

    public override void OnInspectorGUI()
    {
        DrawDefaultInspector();

        EditorGUILayout.Space();

        folded = EditorGUILayout.BeginFoldoutHeaderGroup(folded, "Custom editor");
        if (folded)
        {
            EditorGUI.indentLevel = 2;
            var array = new int[gameObject.size.x, gameObject.size.y];

            for (int y = 0; y < gameObject.size.y; y++)
                for (int x = 0; x < gameObject.size.x; x++)
                    if (gameObject.GetItem(new Vector2Int(x, y)) == null)
                        array[x, y] = -1;

            var items = gameObject.items;
            for (int i = 0; i < items.Count; i++)
                array[items[i].pos.x, items[i].pos.y] = i + 1;

            var label = "";
            for (int y = 0; y < gameObject.size.y; y++)
            {
                label = "";
                for (int x = 0; x < gameObject.size.x; x++)
                {
                    label += array[x, y].ToString() + "  ";
                }
                EditorGUILayout.LabelField(label);
            }

            for (int i = 0; i < items.Count; i++)
                EditorGUILayout.LabelField((i + 1).ToString() + " - " + items[i].item.item.name + " (x" + items[i].item.count + ")");

            EditorGUILayout.Space();
            EditorGUILayout.BeginHorizontal();
            addItem = EditorGUILayout.ObjectField("Item", addItem, typeof(Item), true) as Item;
            if (addItem != null && addItem.MaxCountPerStack > 1)
            {
                addItemCount = EditorGUILayout.IntField(addItemCount);
            }

            if (GUILayout.Button("Add"))
            {
                var item = new InventoryItem();
                item.item = addItem;
                item.count = Mathf.Min(item.item.MaxCountPerStack, addItemCount);
                if (item.count == 0)
                    item.count = 1;

                gameObject.PutItem(item);
            }

            EditorGUILayout.EndHorizontal();

            if (GUILayout.Button("Clear"))
            {
                gameObject.items.Clear();
            }
            EditorGUI.indentLevel = 0;
            EditorGUILayout.EndFoldoutHeaderGroup();
        }
    }
}

#endif