using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace World
{
    public static class IslandGenerator
    {
        public static void GenerateIsland(ref List<World.Region> regions, RectInt islandBounds, int seed)
        {
            var rt = new Vector2(islandBounds.width, islandBounds.height);
            var lb = new Vector2(islandBounds.x, islandBounds.y);
            var ct = new Vector2(islandBounds.x + islandBounds.width / 2, islandBounds.y + islandBounds.height / 2);
            foreach (var region in regions)
            {
                if (islandBounds.Contains(region.Position))
                {
                    var factor = 1f - Vector2.Distance(ct, region.Position)
                        / Vector2.Distance(ct, islandBounds.IntersectionWithRayFromCenter(region.Position));
                    var normalizedPosition = (region.Position - lb);
                    var granularity = 4;
                    normalizedPosition.x *= 1 / rt.x * granularity;
                    normalizedPosition.y *= 1 / rt.y * granularity;
                    var noise = Mathf.FloorToInt(Mathf.PerlinNoise(seed + normalizedPosition.x, seed + normalizedPosition.y) * factor * 4 + factor);
                    region.Height += noise;
                }
            }
        }
    }
}