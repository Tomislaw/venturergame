using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using WorldStructures;
using RiverSearch;

namespace RiverSearch
{
    internal static class EdgeExtensions
    {
        public static Node ToNode(this Edge edge, Node parent)
        {
            var node = new Node();
            node.parent = parent;
            node.distance = parent != null ? parent.distance + 1 : 0;
            node.edge = edge;
            return node;
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

    public class Node
    {
        public Edge edge;
        public int distance = 0;
        public Node parent;
        public bool visited = false;

        public bool IsSame(Node node)
        {
            return node.edge.L == edge.L && node.edge.R == edge.R;
        }

        public bool IsTarget()
        {
            if (edge.Left != null)
            {
                if (edge.Left.Water == true)
                    return true;
                if (edge.Left.Rivers.Find((r) => r.Edge.IsConnecting(edge)) != null)
                    return true;
            }
            if (edge.Right != null)
            {
                if (edge.Right.Water == true)
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
        public Node Start;
        private HashSet<Node> nodes;

        public List<Node> GetNodes()
        {
            nodes = new HashSet<Node>();
            var priorityQueue = new PrioQueue.PriorityQueue<Node, int>();
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

        private List<Node> FindAdjecentNodes(Node node)
        {
            var possibleNodes = new List<Node>();

            if (node.IsTarget())
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

            var nodeNodes = new List<Node>();
            foreach (var possibleNode in possibleNodes)
            {
                nodeNodes.Add(possibleNode);
            }

            return nodeNodes;
        }

        private Node FindNode(Node node)
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

        public void GenerateRivers(ref List<Region> regions)
        {
            random = new System.Random(Seed);

            for (int i = 0; i < Settings.Count; i++)
            {
                var sortedRegions = regions.FindAll(r => r.Rivers.Count == 0 && !r.Water && !r.Coast)
                    .OrderByDescending((region) => (float)region.Height / 2 + random.NextDouble() * 5 - 999 * region.Rivers.Count);

                if (sortedRegions.Count() > 0)
                    GenerateRiver(sortedRegions.First(), 0);
            }
        }

        private void GenerateRiver(Region region, int riverId)
        {
            int id = random.Next(0, region.Edges.Count());

            var riverSearch = new RiverSearch.RiverSearch();
            riverSearch.Start = region.Edges[id].ToNode(null);

            var nodes = riverSearch.GetNodes();

            var ends = nodes.FindAll((item) => item.IsTarget()).OrderBy((item) => (float)Math.Abs(item.distance - 5) + random.NextDouble() * 2);
            if (ends.Count() == 0)
                return;

            var node = ends.First();

            //skip if connected to water
            if ((node.edge.Left.Water || node.edge.Right.Water == true) && node.parent != null)
                node = node.parent;

            while (node != null)
            {
                if (node.parent == null)
                    if (node.edge.Left.Water || node.edge.Right.Water == true)
                        break;

                River river = new River();
                river.Id = riverId;
                river.Edge = node.edge;
                node.edge.Left?.Rivers.Add(river);
                node.edge.Right?.Rivers.Add(river);

                node = node.parent;
            }

            //foreach (var node in nodes)
            //{
            //    River river = new River();
            //    river.Id = riverId;
            //    river.Edge = node.edge;
            //    node.edge.Left?.Rivers.Add(river);
            //    node.edge.Right?.Rivers.Add(river);
            //    if (node.parent == null)
            //        river.size = 1;
            //}

            //// get random verticle
            //var verts = new HashSet<Vector2Int>();
            //foreach (var e in region.Edges)
            //{
            //    verts.Add(e.L);
            //    verts.Add(e.R);
            //}
            //int id = random.Next(0, verts.Count() - 1);
            //var statingVert = verts.ElementAt(id);

            //River previousRiver = null;

            //for (; ; )
            //{
            //    // fist edge with lowest nearby nodes
            //    var edge = LowestEdge(GetEdges(start, statingVert));

            //    // finish if water
            //    if (edge.Left == null || edge.Right == null || edge.Left.Water || edge.Right.Water)
            //        break;

            //    // create new river node
            //    var newRiver = new River();
            //    newRiver.Edge = edge;
            //    newRiver.Id = riverId;
            //    newRiver.Top = previousRiver;

            //    bool shouldStop = false;
            //    foreach (var nodeRiver in edge.Left.Rivers)
            //        if (nodeRiver.Edge.Equals(edge)) // if rivers colliding
            //        {
            //            if (nodeRiver.Id == riverId) // create lake if collides with same river
            //                edge.Left.Water = true;
            //            else // merge rivers
            //                newRiver.Bottom = nodeRiver;
            //            shouldStop = true;
            //            break;
            //        }

            //    if (previousRiver != null)
            //        previousRiver.Bottom = newRiver;
            //    newRiver.Edge.Left.Rivers.Add(newRiver);
            //    newRiver.Edge.Right.Rivers.Add(newRiver);
            //    previousRiver = newRiver;
            //    if (shouldStop)
            //        return;

            //    start = newRiver.Edge.Left;
            //    statingVert = newRiver.Edge.L == statingVert ? newRiver.Edge.R : newRiver.Edge.L;
        }
    }

    //private bool ContainsSameRiver(Edge edge,Region region, int riverId)
    //{
    //    foreach(var river in region.Rivers)
    //    {
    //        if (river.Edge.Equals(edge))
    //        {
    //            if (river.Id == riverId) ;
    //                //createLake
    //            //else
    //                //merge
    //        }
    //    }
    //}

    //    private Edge LowestEdge(List<Edge> edges)
    //    {
    //        Edge lowest = null;
    //        int height = int.MaxValue;
    //        foreach (var edge in edges)
    //        {
    //            int eHeight = edge.Left.Height + edge.Right.Height;
    //            if (eHeight < height)
    //                lowest = edge;
    //        }
    //        return lowest;
    //    }

    //    private (List<Edge>, int) GetEdges(Region region, Vector2Int verticle)
    //    {
    //        foreach (var e in edges)
    //        {
    //            var newRegion = e.Left;
    //            if (newRegion == region)
    //                newRegion = e.Right;

    //            foreach (var e2 in newRegion.Edges)
    //            {
    //                if (e2.L == verticle || e2.R == verticle)
    //                    edges.Add(e);
    //            }
    //        }
    //        return edges.ToList();
    //    }
    //}
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