using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using UnityEngine;
using WorldGenerator;
using WorldStructures.Serializable;
using System.Linq;

public class WorldMap : MonoBehaviour, IWorldMap
{
    public List<WorldStructures.Serializable.Region> Regions;
    public List<WorldStructures.Serializable.River> Rivers;
    public List<WorldStructures.Serializable.Road> Roads;

    public float MaxHeight { get; private set; } = 0;
    public float MinHeight { get; private set; } = 0;
    public Vector2Int Size;

    public int Version = 0;

    public void SetRegions(List<Region> Regions)
    {
        this.Regions = Regions;
        MaxHeight = Regions.Max(it => it.Height);
        MinHeight = Regions.Min(it => it.Height);
        Rivers = new List<River>();
        Roads = new List<Road>();

        foreach (var region in Regions)
        {
            foreach (var river in region.Rivers)
                if (!Rivers.Contains(river))
                    Rivers.Add(river);

            foreach (var road in region.Roads)
                if (!Roads.Contains(road))
                    Roads.Add(road);
        }

#if UNITY_EDITOR
        UnityEditor.EditorApplication.delayCall += () =>
        {
            Version++;
        };
    }

#endif
}