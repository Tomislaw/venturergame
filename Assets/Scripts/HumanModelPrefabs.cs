using System;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "HumanModelPrefabs", menuName = "ScriptableObjects/Characters/HumanModelPrefabs", order = 1)]
public class HumanModelPrefabs : ScriptableObject
{
    public List<SwitchableTextureData> Heads = new List<SwitchableTextureData>();
    public List<SwitchableTextureData> Beards = new List<SwitchableTextureData>();
    public List<SwitchableTextureData> Hairs = new List<SwitchableTextureData>();
    public List<SwitchableTextureData> Bodies = new List<SwitchableTextureData>();
    public List<SwitchableTextureData> Legs = new List<SwitchableTextureData>();

    public Color[] hairColors = new Color[0];
    public Color[] bodyColors = new Color[0];
}

[Serializable]
public struct SwitchableTextureData
{
    public Texture2D main;
    public Texture2D mask;
    public Texture2D normal;
}