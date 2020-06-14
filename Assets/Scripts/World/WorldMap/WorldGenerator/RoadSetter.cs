using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using UnityEngine;
using WorldStructures;
using System.Linq;
using AStarSharp;

namespace Assets.Scripts.World.WorldMap.WorldGenerator
{
    public class RoadSetter
    {
        public Task<List<Region>> GenerateRoads(List<Region> regions, CancellationToken cancellationToken = new CancellationToken())
        {
            Task<List<Region>> task = null;
            task = Task.Run(() =>
            {
                cancellationToken.ThrowIfCancellationRequested();

                return regions;
            });
            return task;
        }

        private void GenerateTownRoads(List<Region> regions, CancellationToken cancellationToken = new CancellationToken())
        {
            var towns = regions.FindAll(it => it.structure == Structure.Town);
            var townsWithRoad = new List<Region>();

            foreach (var town in towns)
            {
                towns.FindAll(it => !townsWithRoad.Contains(it)); //// TODO
            }
        }

        private void GenerateRoad(Region a, Region b, List<Region> regions, int size = 1, CancellationToken cancellationToken = new CancellationToken())
        {
            var start = RegionToNode(a);

            var path = AStarSharp.Astar.FindPath(RegionToNode(a, regions), RegionToNode(b, regions));
            Road previous = null;
            foreach (var p in path)
            {
                cancellationToken.ThrowIfCancellationRequested();

                var r = regions.Find(it => it.Id == p.Id);
                Road road = new Road();
                road.Left = previous?.Center;
                road.Center = r;
                if (previous != null)
                    previous.Right = r;
                r.Roads.Add(road);
                road.size = size;
                previous = road;
            }
        }

        private class RegionAdjecentNodes : AStarSharp.IAdjecentNodes
        {
            public Node node;
            public List<Region> regions;

            public List<Node> GetAdjacentNodes()
            {
                return regions.Find(it => it.Id == node.Id).Neighbors.Select(it => RegionToNode(it, node)).ToList();
            }
        }

        private static Node RegionToNode(Region r, Node parent = null)
        {
            int weight = r.Rivers.Count;
            if (r.Water)
                weight += 10000;
            if (r.Moisture < 0.2 || r.Moisture > 0.8)
                weight += 10;
            weight += r.Height;

            bool walkable = r.Roads.Count < 2;

            var node = new Node(r.Position, walkable, weight);
            node.Parent = parent;
            return node;
        }

        private static Node RegionToNode(Region r, List<Region> regions)
        {
            var node = RegionToNode(r);
            var adjecents = new RegionAdjecentNodes();
            adjecents.node = node;
            adjecents.regions = regions;
            node.function = adjecents;
            return node;
        }
    }
}