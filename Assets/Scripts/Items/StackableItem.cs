using UnityEngine;

[System.Serializable]
[CreateAssetMenu(fileName = "StackableItem", menuName = "ScriptableObjects/Items/StackableItem", order = 1)]
public class StackableItem : Item
{
    public string name;

    public Sprite uiSprite;
    public Sprite gameSprite;

    public Vector2Int size = new Vector2Int(1, 1);
    public int maxCountPerStack = 1;

    public override string ItemName => name;
    public override Sprite UiSprite => uiSprite;
    public override Sprite GameSprite => gameSprite;
    public override Vector2Int Size => size;
    public override int MaxCountPerStack => maxCountPerStack;
}