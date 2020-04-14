using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Inventory : MonoBehaviour
{
    public Vector2Int size;

    public Dictionary<Vector2Int, Inventory> items;
}