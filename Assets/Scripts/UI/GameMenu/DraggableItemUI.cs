using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

[RequireComponent(typeof(RectTransform))]
public class DraggableItemUI : MonoBehaviour, IDragHandler, IEndDragHandler, IBeginDragHandler
{
    public Color dragValidColor = Color.blue;
    public Color dragInvalidColor = Color.red;
    public Image background;
    public Image icon;
    public InventoryItem item;
    private Color originalColor;
    private Vector3 initialPosition;
    private Vector3 draggingOffset;

    public CharacterInventoryController equippedController = null;
    public Inventory inventoryController = null;

    private IItemDropSlot lastDropSlot;

    private void OnEnable()
    {
        originalColor = background.color;
        SetItem(item);
    }

    public void RecalculateSize()
    {
        var rect = GetComponent<RectTransform>();
        if (rect != null && item != null && item.item != null)
            rect.sizeDelta = new Vector2(32 * item.item.Size.x, 32 * item.item.Size.y);
    }

    public void OnEndDrag(PointerEventData eventData)
    {
        transform.position = Input.mousePosition + draggingOffset;
        background.color = originalColor;
        transform.position = initialPosition;

        if (lastDropSlot != null)
        {
            lastDropSlot.OnDropItem(item, gameObject);
        }
    }

    public void OnDrag(PointerEventData eventData)
    {
        transform.position = Input.mousePosition + draggingOffset;

        PointerEventData pointerData = new PointerEventData(EventSystem.current)
        {
            pointerId = -1,
        };
        pointerData.position = transform.position;
        List<RaycastResult> results = new List<RaycastResult>();
        EventSystem.current.RaycastAll(pointerData, results);

        foreach (var result in results)
        {
            var slot = result.gameObject.GetComponent<IItemDropSlot>();
            var canDrop = false;
            if (slot != null)
            {
                canDrop = slot.OnHoverItem(transform.position, item);
                background.color = canDrop ? dragValidColor : dragInvalidColor;
                if (canDrop)
                {
                    lastDropSlot = slot;
                    break;
                }
                if (lastDropSlot != null)
                    lastDropSlot.OnFinishedHovering();
                lastDropSlot = null;
            }
            else
            {
                background.color = canDrop ? dragValidColor : dragInvalidColor;
                if (lastDropSlot != null)
                    lastDropSlot.OnFinishedHovering();
                lastDropSlot = null;
            }
        }
    }

    public void OnBeginDrag(PointerEventData eventData)
    {
        draggingOffset = transform.position - Input.mousePosition;
        initialPosition = transform.position;
    }

    public void SetItem(InventoryItem item)
    {
        this.item = item;
        icon.sprite = item.item ? item.item.UiSprite : null;
        RecalculateSize();
    }
}