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
            if (this == null)
                return;
            for (int i = this.transform.childCount; i > 0; --i)
                DestroyImmediate(this.transform.GetChild(0).gameObject);
            AddIcons();
        };
    }

    private void Update()
    {
        if (World == null)
            return;

        if (World.GetHashCode() == worldHash)
            return;

        worldHash = World.GetHashCode();

        if (this == null)
            return;
        for (int i = this.transform.childCount; i > 0; --i)
            DestroyImmediate(this.transform.GetChild(0).gameObject);
        AddIcons();
    }

#endif

    private void AddIcons()
    {
        if (World == null)
            return;

        var rnd = new System.Random();

        foreach (var region in World.Regions)
        {
            var icon = Icons.Find((i) => i.biome == region.biome);

            if (icon != null && icon.icons.Count > 0)
            {
                var sprite = icon.icons[rnd.Next(0, icon.icons.Count - 1)];
                var go = CreateIcon(sprite);

                go.transform.SetParent(transform);
                go.transform.localPosition = new Vector2(region.Center.x, region.Center.y) / (float)pixelPerUnit;
                go.name = region.biome.ToString() + "_" + sprite.name;
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