using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CharacterAppearanceSetter : MonoBehaviour
{
    public GameObject character;
    public HumanCharacterBodyPrefabs prefabs;

    private HumanCharacter human;

    public bool IsMale
    {
        get { return human.male; }
        set
        {
            human.male = value;
            human.Invalidate();
        }
    }

    public int LegsCount { get => IsMale ? prefabs.Prefabs["maleLegs"].Count : prefabs.Prefabs["femaleLegs"].Count; }

    public int Legs
    {
        get { return human.legs; }
        set
        {
            if (value < 0 || value >= LegsCount)
            {
                human.legs = 0;
                return;
            }

            human.legs = value;
            human.Invalidate();
        }
    }

    public int HairCount { get => IsMale ? prefabs.Prefabs["maleHair"].Count : prefabs.Prefabs["femaleHair"].Count; }

    public int Hair
    {
        get { return human.hair; }
        set
        {
            if (value < 0 || value >= HairCount)
            {
                human.hair = 0;
                return;
            }

            human.hair = value;
            human.Invalidate();
        }
    }

    public int BodyCount { get => IsMale ? prefabs.Prefabs["maleBody"].Count : prefabs.Prefabs["femaleBody"].Count; }

    public int Body
    {
        get { return human.body; }
        set
        {
            if (value < 0 || value >= BodyCount)
            {
                human.body = 0;
                return;
            }

            human.body = value;
            human.Invalidate();
        }
    }

    public int HeadCount { get => IsMale ? prefabs.Prefabs["maleHead"].Count : prefabs.Prefabs["femaleHead"].Count; }

    public int Head
    {
        get { return human.head; }
        set
        {
            if (value < 0 || value >= HeadCount)
            {
                human.head = 0;
                return;
            }

            human.head = value;
            human.Invalidate();
        }
    }

    private void Awake()
    {
        human = character.GetComponent<HumanCharacter>();
    }
}