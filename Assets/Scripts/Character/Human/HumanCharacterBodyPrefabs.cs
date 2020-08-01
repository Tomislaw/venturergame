using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

[CreateAssetMenu(fileName = "HumanCharacterBodyPrefabs", menuName = "ScriptableObjects/Characters/HumanCharacterBodyPrefabs", order = 1)]
public class HumanCharacterBodyPrefabs : ScriptableObject
{
    public List<GameObject> femaleHeads = new List<GameObject>();
    public List<GameObject> maleHeads = new List<GameObject>();

    public List<GameObject> femaleLegs = new List<GameObject>();
    public List<GameObject> maleLegs = new List<GameObject>();

    public List<GameObject> femaleHairs = new List<GameObject>();
    public List<GameObject> maleHairs = new List<GameObject>();

    public List<GameObject> femaleBodies = new List<GameObject>();
    public List<GameObject> maleBodies = new List<GameObject>();

    public void Reload()
    {
        femaleHeads.Clear();
        maleHeads.Clear();
        femaleHairs.Clear();
        maleHairs.Clear();
        femaleBodies.Clear();
        maleBodies.Clear();

        var heads = Resources.LoadAll("Prefabs/Characters/Human/Head", typeof(GameObject));
        foreach (var head in heads)
        {
            if (head.name.StartsWith("f_"))
                femaleHeads.Add(head as GameObject);
            else if (head.name.StartsWith("m_"))
                maleHeads.Add(head as GameObject);
        }

        var bodies = Resources.LoadAll("Prefabs/Characters/Human/Body", typeof(GameObject));
        foreach (var body in bodies)
        {
            if (body.name.StartsWith("f_"))
                femaleBodies.Add(body as GameObject);
            else if (body.name.StartsWith("m_"))
                maleBodies.Add(body as GameObject);
        }

        var hairs = Resources.LoadAll("Prefabs/Characters/Human/Hair", typeof(GameObject));
        foreach (var hair in hairs)
        {
            if (hair.name.StartsWith("f_"))
                femaleHairs.Add(hair as GameObject);
            else if (hair.name.StartsWith("m_"))
                maleHairs.Add(hair as GameObject);
        }

        var legs = Resources.LoadAll("Prefabs/Characters/Human/Legs", typeof(GameObject));
        foreach (var leg in legs)
        {
            if (leg.name.StartsWith("f_"))
                femaleHairs.Add(leg as GameObject);
            else if (leg.name.StartsWith("m_"))
                maleHairs.Add(leg as GameObject);
        }
    }

    public void OnValidate()
    {
        Reload();
    }

    public void Awake()
    {
        Reload();
    }
}