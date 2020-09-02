using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

[CreateAssetMenu(fileName = "HumanCharacterBodyPrefabs", menuName = "ScriptableObjects/Characters/HumanCharacterBodyPrefabs", order = 1)]
public class HumanCharacterBodyPrefabs : ScriptableObject
{
    public Dictionary<string, List<GameObject>> Prefabs = new Dictionary<string, List<GameObject>>();

    public Color[] hairColors = new Color[0];
    public Color[] bodyColors = new Color[0];

    public string beardPrefabs = "Prefabs/Characters/Human/Beard";
    public string headPrefabs = "Prefabs/Characters/Human/Head";
    public string hairPrefabs = "Prefabs/Characters/Human/Hair";
    public string legsPrefabs = "Prefabs/Characters/Human/Legs";
    public string bodyPrefabs = "Prefabs/Characters/Human/Body";

    public void Reload()
    {
        Prefabs.Clear();

        InsertPrefabs("Beard", beardPrefabs);
        InsertPrefabs("Head", headPrefabs);
        InsertPrefabs("Body", bodyPrefabs);
        InsertPrefabs("Hair", hairPrefabs);
        InsertPrefabs("Legs", legsPrefabs);
    }

    private void InsertPrefabs(string name, string directory)
    {
        var female = new List<GameObject>();
        var male = new List<GameObject>();
        var prefabs = Resources.LoadAll(directory, typeof(GameObject));
        foreach (var prefab in prefabs)
        {
            if (prefab.name.StartsWith("f_"))
                female.Add(prefab as GameObject);
            else if (prefab.name.StartsWith("m_"))
                male.Add(prefab as GameObject);
        }
        Prefabs["female" + name] = female;
        Prefabs["male" + name] = male;
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