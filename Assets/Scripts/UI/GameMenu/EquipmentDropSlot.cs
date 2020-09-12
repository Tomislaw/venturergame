using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public interface IItemDropSlot
{
    bool OnHoverItem(Vector2 position, InventoryItem item);

    void OnFinishedHovering();

    void OnDropItem(InventoryItem item, GameObject gameObject);
}

public class EquipmentDropSlot : MonoBehaviour, IItemDropSlot
{
    public Equipment current;
    public Equipment.Type acceptType;
    public Color hoverValidColor;
    private Color initialColor;
    public Image image;
    public Vector2 itemOffset;

    public EquipmentUI equipmentUI;

    private void OnEnable()
    {
        if (image)
            initialColor = image.color;
    }

    public bool OnHoverItem(Vector2 position, InventoryItem item)
    {
        var equipment = item.item as Equipment;

        bool valid = (equipment != null && equipment.type == acceptType);

        if (image && valid)
            image.color = hoverValidColor;

        return valid;
    }

    public void OnDropItem(InventoryItem item, GameObject gameObject)
    {
        gameObject.transform.SetParent(transform, false);
        gameObject.transform.localPosition = new Vector3();
        if (image)
            image.color = initialColor;

        equipmentUI.EquipItem(item, gameObject);
    }

    public void OnFinishedHovering()
    {
        if (image)
            image.color = initialColor;
    }

    public void Start()
    {
    }

    // Update is called once per frame
    private void Update()
    {
    }
}