using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using WorldStructures;
using RiverSearch;
using System.Threading;
using System.Threading.Tasks;

namespace RiverSearch
{
    internal static class EdgeExtensions
    {
        public static RiverNode ToNode(this Edge edge, RiverNode parent)
        {
            return new RiverNode(edge, parent);
        }

        public static int Height(this Edge edge, Edge previousEdge)
        {
            var neighbours = new HashSet<Region>();

            if (edge.Left != null)
                neighbours.Add(edge.Left);
            if (edge.Right != null)
                neighbours.Add(edge.Right);

            if (previousEdge.Left != null)
                neighbours.Add(previousEdge.Left);
            if (previousEdge.Right != null)
                neighbours.Add(previousEdge.Right);

            return neighbours.Aggregate(0, (a, b) => a += b.Height);
        }

        public static bool IsConnecting(this Edge e1, Edge e2)
        {
            return e1.L == e2.L || e1.L == e2.R || e1.R == e2.L || e1.R == e2.R;
        }
    }

    public class RiverNode
    {
        public Edge edge;
        public int distance = 0;
        public RiverNode parent;
        public bool visited = false;

        public bool connectedToWater;
        public River connectedRiver;

        public RiverNode(Edge edge, RiverNode parent)
        {
            this.parent = parent;
            this.distance = parent != null ? parent.distance + 1 : 0;
            this.edge = edge;
            connectedToWater = IsConnectedToWater();
            connectedRiver = FindConnectedRivers();
        }

        public bool IsSame(RiverNode node)
        {
            return node.edge.L == edge.L && node.edge.R == edge.R;
        }

        private bool IsConnectedToWater()
        {
            if (edge.Left != null)
                if (edge.Left.Water == true)
                    return true;

            if (edge.Right != null)
                if (edge.Right.Water == true)
                    return true;
            return false;
        }

        private River FindConnectedRivers()
        {
            River river = null;

            if (edge.Left != null)
                river = edge.Left.Rivers.Find((r) => r.Edge.IsConnecting(edge));

            if (river != null)
                return river;

            if (edge.Right != null)
                river = edge.Right.Rivers.Find((r) => r.Edge.IsConnecting(edge));

            return river;
        }

        public bool IsTarget()
        {
            if (connectedToWater)
                return true;

            if (connectedRiver != null)
            {
                if (connectedRiver.Top == null)
                    return false;

                return true;
            }
            return false;
        }

        public int Height()
        {
            if (parent != null)
                return edge.Height(parent.edge);
            else return 999;
        }

        public override string ToString()
        {
            return edge + " d:" + distance + " h:" + Height() + " t:" + IsTarget();
        }
    }

    public class RiverSearch
    {
        public RiverNode Start;
        private HashSet<RiverNode> nodes;

        public List<RiverNode> GetNodes()
        {
            nodes = new HashSet<RiverNode>();
            var priorityQueue = new PrioQueue.PriorityQueue<RiverNode, int>();
            priorityQueue.Enqueue(Start, 0);

            while (!priorityQueue.IsEmpty())
            {
                var node = priorityQueue.Dequeue();

                if (FindNode(node) != null)
                    continue;

                nodes.Add(node);

                foreach (var adjecentNode in FindAdjecentNodes(node))
                {
                    if (adjecentNode.Height() > node.Height())
                        continue;

                    priorityQueue.Enqueue(adjecentNode, adjecentNode.distance);
                }
                node.visited = true;
            }

            return nodes.ToList();
        }

        private List<RiverNode> FindAdjecentNodes(RiverNode node)
        {
            var possibleNodes = new List<RiverNode>();

            if (node.connectedToWater || node.connectedRiver != null)
                return possibleNodes;

            if (node.parent == null)
            {
                possibleNodes.AddRange(node.edge.LEdges().Select((edge) => edge.ToNode(node)));
                possibleNodes.AddRange(node.edge.REdges().Select((edge) => edge.ToNode(node)));
            }
            else
            {
                if (node.parent.edge.L == node.edge.L || node.parent.edge.R == node.edge.L)
                    possibleNodes.AddRange(node.edge.REdges().Select((edge) => edge.ToNode(node)));
                else
                    possibleNodes.AddRange(node.edge.LEdges().Select((edge) => edge.ToNode(node)));
            }

            var nodeNodes = new List<RiverNode>();
            foreach (var possibleNode in possibleNodes)
            {
                nodeNodes.Add(possibleNode);
            }

            return nodeNodes;
        }

        private RiverNode FindNode(RiverNode node)
        {
            foreach (var n in nodes)
                if (n.IsSame(node))
                    return n;
            return null;
        }
    }
}

namespace WorldGenerator
{
    [Serializable]
    public struct RiverSettings
    {
        public int Count;
    }

    public class RiverCreator
    {
        public RiverSettings Settings;
        public int Seed = 0;
        private System.Random random;

        public Task<List<Region>> GenerateRivers(List<Region> regions, CancellationToken cancellationToken = new CancellationToken())
        {
            random = new System.Random(Seed);

            Task<List<Region>> task = null;
            task = Task.Run(() =>
            {
                for (int i = 0; i < Settings.Count; i++)
                {
                    cancellationToken.ThrowIfCancellationRequested();

                    var sortedRegions = regions
                        .FindAll(r => CanGenerateRiver(r))
                        .OrderByDescending(r => HowMuchRiverIsNeeded(r, random));

                    if (sortedRegions.Count() > 0)
                        GenerateRiver(sortedRegions.First(), 0, cancellationToken);
                }
                return regions;
            });
            return task;
        }

        private bool CanGenerateRiver(Region r)
        {
            if (r.Rivers.Count != 0 || r.Water || r.Coast || r.Height <= 1)
                return false;
            else
            {
                foreach (var neighbour in r.Neighbors)
                {
                    if (neighbour.Rivers.Count == 0)
                        continue;

                    foreach (var river in neighbour.Rivers)
                    {
                        var edges = river.Edge.LEdges();
                        edges.AddRange(river.Edge.REdges());

                        foreach (var regionEdge in r.Edges)
                            if (edges.Contains(regionEdge))
                                return false;
                    }
                }
            }
            return true;
        }

        private double HowMuchRiverIsNeeded(Region r, System.Random random)
        {
            var value = r.Height - random.Next(0, 2);

            return value;
        }

        private void GenerateRiver(Region region, int riverId, CancellationToken cancellationToken)
        {
            int id = random.Next(0, region.Edges.Count());

            var riverSearch = new RiverSearch.RiverSearch();
            riverSearch.Start = region.Edges[id].ToNode(null);

            var nodes = riverSearch.GetNodes();

            var ends = nodes
                .FindAll((item) => item.IsTarget())
                .OrderBy((item) =>
                {
                    cancellationToken.ThrowIfCancellationRequested();

                    double weight = item.distance;

                    if (item.connectedToWater)
                        weight -= random.NextDouble() * 16;

                    if (item.distance < 4)
                        weight += 100;

                    weight -= (random.NextDouble() * 2 - 1) * 5;

                    return weight;
                });

            if (ends.Count() == 0)
                return;

            var node = ends.First();

            //skip if connected to water
            if (node.connectedToWater && node.parent != null)
                node = node.parent;

            River previousRiver = null;
            while (node != null)
            {
                cancellationToken.ThrowIfCancellationRequested();

                // remove last when connected to water
                if (node.parent == null)
                    if (node.connectedToWater)
                        break;
                // todo add top and bottom for river
                River river = new River();
                river.Id = riverId;
                river.Edge = node.edge;
                node.edge.Left?.Rivers.Add(river);
                node.edge.Right?.Rivers.Add(river);

                node = node.parent;

                river.Bottom = previousRiver;
                if (previousRiver != null)
                    previousRiver.Top = river;

                previousRiver = river;
            }
        }
    }
}

internal static class Helper
{
    public static List<Edge> LEdges(this Edge edge)
    {
        return edge.GetAdjecentEdges(edge.L);
    }

    public static List<Edge> REdges(this Edge edge)
    {
        return edge.GetAdjecentEdges(edge.R);
    }

    public static List<Edge> GetAdjecentEdges(this Edge mainEdge, Vector2Int v)
    {
        var edges = new List<Edge>();

        var edgePoint = v;
        if (mainEdge.Left != null)
            foreach (var e in mainEdge.Left.Edges)
            {
                if (mainEdge.Equals(e))
                    continue;

                if (e.L == edgePoint || e.R == edgePoint)
                {
                    edges.Add(e);
                    break;
                }
            }
        if (mainEdge.Right != null)
            foreach (var e in mainEdge.Right.Edges)
            {
                if (mainEdge.Equals(e))
                    continue;

                if (e.L == edgePoint || e.R == edgePoint)
                {
                    edges.Add(e);
                    break;
                }
            }

        return edges;
    }
}