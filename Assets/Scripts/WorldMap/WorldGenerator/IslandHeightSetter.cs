using System;
using System.Collections.Generic;
using UnityEngine;
using WorldStructures;

namespace WorldGenerator
{
    [System.Serializable]
    public struct IslandHeightSetterSettings
    {
        public RectInt IslandRectangle;
        public int MaxHeight;
        public float Sharpness;
    }

    public class IslandHeightSetter
    {
        public int Seed = 0;
        public IslandHeightSetterSettings Settings;

        public void GenerateIsland(ref List<Region> regions)
        {
            foreach (var region in regions)
            {
                //var intersection = UnityEngineExtensions.IntersectionWithRayFromCenter(Settings.IslandRectangle, region.Center);
                //var distanceMax = Vector2.Distance(intersection, Settings.IslandRectangle.center);
                var distanceFactor = Vector2.Distance(region.Center, Settings.IslandRectangle.center) / Settings.IslandRectangle.width;
                //var factor = 1 - distance / distanceMax;

                var x = (region.Center.x - Settings.IslandRectangle.x) / (float)Settings.IslandRectangle.width * Settings.Sharpness;
                var y = (region.Center.y - Settings.IslandRectangle.y) / (float)Settings.IslandRectangle.height * Settings.Sharpness;
                var noise = Mathf.PerlinNoise(x + Seed, y + Seed);

                var heightFactor = distanceFactor - (1 - distanceFactor);
                var height = noise * Settings.MaxHeight + (-distanceFactor * Settings.MaxHeight) - Settings.MaxHeight * distanceFactor + 1;
                region.Height = (int)height;
                if (height >= 0)
                    region.Water = false;
                else if (height < 0) height *= 2;

                region.Height = (int)height;
            }
            SetCorrectWaterType(ref regions);
        }

        private void SetCorrectWaterType(ref List<Region> regions)
        {
            Region startingOceanRegion = regions.Find((r) => r.Water && r.Border);
            var ocean = new List<Region>();

            var queue = new Queue<Region>();

            if (startingOceanRegion != null)
                queue.Enqueue(startingOceanRegion);

            while (queue.Count > 0)
            {
                var r = queue.Dequeue();

                if (ocean.Contains(r))
                    continue;

                r.ConnectedToOcean = true;
                ocean.Add(r);

                foreach (var n in r.Neighbors)
                    if (n.Water)
                        queue.Enqueue(n);
            }
        }
    }
}