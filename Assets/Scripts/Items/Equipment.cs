using UnityEngine;

[System.Serializable]
[CreateAssetMenu(fileName = "EquipmentItem", menuName = "ScriptableObjects/Items/EquipmentItem", order = 1)]
public class Equipment : SingleItem
{
    public int armorModifier;
    public int damageModifier;

    public GameObject femaleSpriteSheet;
    public GameObject maleSpriteSheet;

    public Type type;

    public enum Type
    {
        MainHand, OffHand, TwoHanded, Helmet, Armor, Boots, Pants, Necklace, Ring
    }

    public override string ToString()
    {
        var str = "{";
        str += "name:" + name;
        str += ",type:" + type;
        str += ",atk:" + armorModifier;
        str += ",def:" + damageModifier;
        str += "}";
        return str;
    }
}