using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EquipmentUI : MonoBehaviour
{
    public CharacterInventoryController controller;

    public EquipmentDropSlot head;
    public EquipmentDropSlot body;
    public EquipmentDropSlot pants;
    public EquipmentDropSlot boots;
    public EquipmentDropSlot mainhand;
    public EquipmentDropSlot offhand;
    public EquipmentDropSlot necklace;
    public EquipmentDropSlot ring;

    public DraggableItemUI draggablePrefab;

    private void OnValidate()
    {
        if (head)
            head.equipmentUI = this;
        if (body)
            body.equipmentUI = this;
        if (pants)
            pants.equipmentUI = this;
        if (boots)
            boots.equipmentUI = this;
        if (mainhand)
            mainhand.equipmentUI = this;
        if (offhand)
            offhand.equipmentUI = this;
        if (necklace)
            necklace.equipmentUI = this;
        if (ring)
            ring.equipmentUI = this;
    }

    // Update is called once per frame
    private void Update()
    {
    }

    private void OnEnable()
    {
        Reload();
    }

    private void CreateAndPutInventoryIcon(Equipment equipped)
    {
        EquipmentDropSlot parent = null;
        if (equipped.type == Equipment.Type.Helmet)
            parent = head;
        else if (equipped.type == Equipment.Type.Armor)
            parent = body;
        else if (equipped.type == Equipment.Type.Pants)
            parent = pants;
        else if (equipped.type == Equipment.Type.Boots)
            parent = boots;
        else if (equipped.type == Equipment.Type.MainHand || equipped.type == Equipment.Type.TwoHanded || equipped.type == Equipment.Type.Bow)
            parent = mainhand;
        else if (equipped.type == Equipment.Type.OffHand)
            parent = offhand;
        else if (equipped.type == Equipment.Type.Ring)
            parent = ring;
        else if (equipped.type == Equipment.Type.Necklace)
            parent = necklace;

        var item = new InventoryItem();
        item.item = equipped;

        var go = Instantiate(draggablePrefab);
        go.SetItem(item);
        go.transform.SetParent(parent.transform, false);
        go.transform.localPosition = new Vector3();
        go.transform.localScale = new Vector3(1, 1, 1);
        go.equippedController = controller;
    }

    public bool EquipItem(InventoryItem equip, GameObject gameObject)
    {
        var draggable = gameObject.GetComponent<DraggableItemUI>();
        draggable.equippedController = controller;
        var newItem = draggable.inventoryController.TakeItem(equip);
        draggable.inventoryController = null;
        controller.Equip(newItem);
        return true;
    }

    public void Reload()
    {
        foreach (var component in GetComponentsInChildren<DraggableItemUI>())
        {
            Destroy(component.gameObject);
        }
        foreach (var equippment in controller.equipped)
        {
            CreateAndPutInventoryIcon(equippment);
        }
    }
}