using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InventoryItem : MonoBehaviour
{
    public string itemName;
    public bool stackable = false;
    public int count = 1;

    public Sprite uiSprite;
    public Sprite worldSprite;
}