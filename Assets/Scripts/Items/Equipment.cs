using UnityEngine;

[System.Serializable]
[CreateAssetMenu(fileName = "EquipmentItem", menuName = "Venturer/Items/EquipmentItem", order = 1)]
public class Equipment : SingleItem
{
    public int armorModifier;
    public int damageModifier;

    public float attackTime = 0.2f;

    public GameObject femaleSpriteSheet;
    public GameObject maleSpriteSheet;

    public GameObject femaleSpriteSheet_arms;
    public GameObject maleSpriteSheet_arms;

    public Projectile projectile;

    public Type type;

    public enum Type
    {
        MainHand, OffHand, TwoHanded, Helmet, Armor, Boots, Pants, Necklace, Ring, Bow
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

public static class EquipmentExtensions
{
    public static bool IsWeapon(this Equipment.Type w)
    {
        return w == Equipment.Type.MainHand || w == Equipment.Type.OffHand || w == Equipment.Type.TwoHanded || w == Equipment.Type.Bow;
    }
}