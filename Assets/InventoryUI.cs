using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(RectTransform))]
[RequireComponent(typeof(GridLayoutGroup))]
public class InventoryUI : MonoBehaviour, IItemDropSlot
{
    public GameObject inventorySlot;
    private Vector2Int size;
    public GameObject[,] slots;
    public Inventory inventory;

    public Color basicColor;
    public Color hoveredColor;

    public DraggableItemUI draggablePrefab;
    private Vector2Int lastHoveredPosition;

    public void OnDropItem(InventoryItem item, GameObject gameObject)
    {
        foreach (var slot in slots)
        {
            var _image = slot.GetComponent<Image>();
            _image.color = basicColor;
        }

        var itemSlot = slots[lastHoveredPosition.x, lastHoveredPosition.y];
        gameObject.transform.parent = itemSlot.transform;
        gameObject.transform.localPosition = new Vector3((item.item.Size.x) * 16, -(item.item.Size.y - 2) * 16);

        var draggable = gameObject.GetComponent<DraggableItemUI>();
        draggable.inventoryController = inventory;
        if (draggable.equippedController)
            draggable.equippedController.Unequip((item.item as Equipment).type);
        draggable.equippedController = null;

        if (inventory.ContainsItem(item))
            inventory.MoveItem(item, lastHoveredPosition);
        else
            inventory.PutItem(item, lastHoveredPosition);
    }

    public void OnFinishedHovering()
    {
        foreach (var slot in slots)
        {
            var _image = slot.GetComponent<Image>();
            _image.color = basicColor;
        }
        //throw new System.NotImplementedException();
    }

    public bool OnHoverItem(Vector2 position, InventoryItem item)
    {
        foreach (var slot in slots)
        {
            var _image = slot.GetComponent<Image>();
            _image.color = basicColor;
        }

        var slotId = GetSlotId(position, item.item);
        if (slotId.x >= size.x || slotId.y >= size.y || slotId.x < 0 | slotId.y < 0)
            return false;

        lastHoveredPosition = slotId;

        for (int x = slotId.x; x < (slotId.x + item.item.Size.x); x++)
        {
            for (int y = slotId.y; y < (slotId.y + item.item.Size.y); y++)
            {
                if (x < 0 || x >= size.x || y < 0 || y >= size.y)
                    continue;
                var itemSlot = slots[x, y];
                var image = itemSlot.GetComponent<Image>();
                image.color = hoveredColor;
            }
        }
        if (inventory.ContainsItem(item))
            return inventory.CanMoveItemHere(item, lastHoveredPosition);
        else
            return inventory.CanPlaceItemHere(item, lastHoveredPosition);
    }

    private Vector2Int GetSlotId(Vector2 worldPosition, Item item)
    {
        var pos = transform.InverseTransformPoint(worldPosition);
        pos += new Vector3(-(item.Size.x - 1) * 16, (item.Size.y - 1) * 16);
        pos /= 32;

        return new Vector2Int((int)pos.x, -(int)pos.y);
    }

    private void OnEnable()
    {
        if (!inventory)
            return;

        size = inventory.size;

        var layout = GetComponent<GridLayoutGroup>();
        layout.constraintCount = size.x;

        slots = new GameObject[size.x, size.y];

        if (inventorySlot != null)
            for (int y = 0; y < size.y; y++)
                for (int x = 0; x < size.x; x++)
                {
                    var obj = Instantiate(inventorySlot);
                    obj.name = "x" + x + "_y" + y;
                    obj.transform.SetParent(transform, false);
                    obj.GetComponent<Image>().color = basicColor;
                    slots[x, y] = obj;
                }

        foreach (var item in inventory.items)
        {
            CreateAndPutInventoryIcon(item);
        }
    }

    private void OnDisable()
    {
        foreach (Transform child in transform)
        {
            Destroy(child.gameObject);
        }
    }

    private void CreateAndPutInventoryIcon(InventoryPair pair)
    {
        var go = Instantiate(draggablePrefab);
        var slot = slots[pair.pos.x, pair.pos.y];
        go.transform.parent = slot.transform;
        go.SetItem(pair.item);
        go.transform.localPosition = new Vector3((go.item.item.Size.x) * 16, -(go.item.item.Size.y - 2) * 16);
        go.transform.localScale = new Vector3(1, 1, 1);
        go.inventoryController = inventory;
    }

    // Update is called once per frame
    private void Update()
    {
    }
}