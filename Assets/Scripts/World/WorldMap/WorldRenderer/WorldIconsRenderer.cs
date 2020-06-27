using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[ExecuteInEditMode]
public class WorldIconsRenderer : MonoBehaviour
{
    [Serializable]
    public class Icon
    {
        public Icon(WorldStructures.Biome b)
        {
            biome = b;
            icons = new List<Sprite>();
        }

        public WorldStructures.Structure structure;
        public WorldStructures.Biome biome;
        public List<Sprite> icons;
    }

    public WorldMap World;
    public List<Icon> Icons = new List<Icon>();
    public int pixelPerUnit = 32;

    private int worldHash = 0;

    private void Start()
    {
        AddIcons();
    }

#if UNITY_EDITOR

    private void OnValidate()
    {
        UnityEditor.EditorApplication.delayCall += () =>
        {
            Reload();
        };
    }

    private void Reload()
    {
        if (this == null)
            return;
        for (int i = this.transform.childCount; i > 0; --i)
            DestroyImmediate(this.transform.GetChild(0).gameObject);
        AddIcons();
    }

    private void Update()
    {
        if (World == null)
            return;

        if (World.Version == worldHash)
            return;

        worldHash = World.GetHashCode();

        UnityEditor.EditorApplication.delayCall += () =>
        {
            Reload();
        };
    }

#endif

    private void AddIcons()
    {
        if (World == null)
            return;

        var rnd = new System.Random();

        foreach (var iconType in Icons)
        {
            var regions = World.Regions.FindAll(it =>
            {
                if (iconType.structure != WorldStructures.Structure.None)
                    return it.structure == iconType.structure;
                else
                    return it.structure == iconType.structure && it.biome == iconType.biome;
            });

            foreach (var region in regions)
            {
                var sprite = iconType.icons[rnd.Next(0, iconType.icons.Count - 1)];
                var go = CreateIcon(sprite);

                go.transform.SetParent(transform);
                go.transform.localPosition = new Vector2(region.Center.x, region.Center.y) / (float)pixelPerUnit;
                go.name = region.biome.ToString() + "_" + sprite.name;
                go.layer = gameObject.layer;
            }
        }
    }

    private GameObject CreateIcon(Sprite sprite)
    {
        var go = new GameObject();
        var sr = go.AddComponent<SpriteRenderer>();
        sr.sprite = sprite;
        return go;
    }

    // Update is called once per frame
}