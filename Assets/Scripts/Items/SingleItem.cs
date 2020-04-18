using UnityEngine;

[CreateAssetMenu(fileName = "SingeItem", menuName = "ScriptableObjects/Items/SingeItem", order = 1)]
[System.Serializable]
public class SingleItem : Item
{
    public string name;

    public Sprite uiSprite;
    public Sprite gameSprite;
    public Vector2Int size = new Vector2Int(1, 1);

    public override string ItemName => name;
    public override Sprite UiSprite => uiSprite;
    public override Sprite GameSprite => gameSprite;

    public override Vector2Int Size => size;

    public override int MaxCountPerStack => 1;
}