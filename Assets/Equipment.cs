using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Equipment : MonoBehaviour
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
}