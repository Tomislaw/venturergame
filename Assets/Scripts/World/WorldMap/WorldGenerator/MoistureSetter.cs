using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using UnityEngine;
using WorldStructures;

namespace WorldGenerator
{
    [System.Serializable]
    public struct MoistureSetterSettings
    {
        [UnityEngine.Range(0, 1)]
        public float Moisture;
    }

    public class MoistureSetter
    {
        public MoistureSetterSettings Settings;

        public Task<List<Region>> CalculateMoisture(List<Region> regions, CancellationToken cancellationToken = new CancellationToken())
        {
            Task<List<Region>> task = null;
            task = Task.Run(() =>
            {
                //self moisture
                foreach (var region in regions)
                {
                    cancellationToken.ThrowIfCancellationRequested();

                    if (region.Water)
                    {
                        region.Moisture = 1;
                        continue;
                    }
                    var tempFactor = 1 - region.Temperature;
                    var riversFactor = region.Rivers.Count / (float)region.Edges.Count;
                    var waterFactor = region.Neighbors.FindAll(r => r.Water).Count / (float)region.Neighbors.Count;
                    var lakeFactor = region.Neighbors.FindAll(r => r.Water && !r.ConnectedToOcean).Count / (float)region.Neighbors.Count;
                    var globalMoistureFactor = Settings.Moisture * 2 - 1;
                    var moisture = Math.Min(1, (lakeFactor + riversFactor * 2f + waterFactor));
                    moisture -= 0.4f * tempFactor * moisture;
                    moisture += globalMoistureFactor;
                    moisture = Math.Max(0, Math.Min(1, moisture));
                    region.Moisture = moisture;
                }

                //neighbour moisture
                var visited = new HashSet<Region>();
                var queue = new PrioQueue.PriorityQueue<Region, float>();

                foreach (var region in regions.FindAll(r => r.Coast || r.Rivers.Count > 0))
                {
                    cancellationToken.ThrowIfCancellationRequested();

                    queue.Enqueue(region, -region.Moisture);
                }

                while (!queue.IsEmpty())
                {
                    cancellationToken.ThrowIfCancellationRequested();

                    var r = queue.Dequeue();

                    if (r.Water)
                        continue;

                    if (visited.Contains(r))
                        continue;
                    visited.Add(r);

                    foreach (var n in r.Neighbors)
                    {
                        cancellationToken.ThrowIfCancellationRequested();

                        if (r.Moisture > n.Moisture)
                        {
                            var maxIncrease = r.Moisture - n.Moisture;
                            var increase = 0.5f * maxIncrease;
                            if (n.Height > r.Height)
                                increase *= 1 / (float)(n.Height - r.Height);
                            n.Moisture += increase;
                        }

                        queue.Enqueue(n, -n.Moisture);
                    }
                }
                return regions;
            });
            return task;
        }
    }
}