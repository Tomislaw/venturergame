using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

namespace WorldStructures
{
    namespace Serializable
    {
        [System.Serializable]
        public struct Edge
        {
            public Vector2Int L;
            public Vector2Int R;
            public int Left;
            public int Right;

            public static Edge Serialize(WorldStructures.Edge edge)
            {
                if (edge == null)
                    return new Edge
                    {
                        Left = -1,
                        Right = -1,
                    };

                return new Edge
                {
                    L = edge.L,
                    R = edge.R,
                    Left = edge.Left != null ? edge.Left.Id : -1,
                    Right = edge.Right != null ? edge.Right.Id : -1,
                };
            }

            public static List<Edge> Serialize(List<WorldStructures.Edge> edges)
            {
                return edges.Select(it => Edge.Serialize(it)).ToList();
            }
        }

        [System.Serializable]
        public struct Region
        {
            public Vector2Int Position;
            public Vector2Int Center;
            public Vector2Int RegionId;

            public int Id;
            public int Height;
            public float Moisture;
            public float Temperature;
            public Biome biome;
            public Structure structure;

            public bool Border;
            public bool Coast;
            public bool Water;
            public bool ConnectedToOcean;

            public List<int> Neighbors;
            public List<Edge> Edges;
            public List<River> Rivers;
            public List<Road> Roads;

            public static Region Serialize(WorldStructures.Region region)
            {
                return new Region
                {
                    Id = region.Id,
                    Height = region.Height,
                    Moisture = region.Moisture,
                    Temperature = region.Temperature,
                    biome = region.biome,
                    structure = region.structure,
                    Border = region.Border,
                    Coast = region.Coast,
                    Water = region.Water,
                    ConnectedToOcean = region.ConnectedToOcean,
                    Neighbors = region.Neighbors.Select(it => it.Id).ToList(),
                    Edges = Edge.Serialize(region.Edges),
                    Rivers = River.Serialize(region.Rivers),
                    Roads = Road.Serialize(region.Roads),
                    Center = region.Center,
                    RegionId = region.RegionId,
                };
            }

            public static List<Region> Serialize(List<WorldStructures.Region> regions)
            {
                return regions.Select(it => Region.Serialize(it)).ToList();
            }
        }

        [System.Serializable]
        public struct Road
        {
            public int Id;
            public int Type;
            public int Left;
            public int Right;

            public static Road Serialize(WorldStructures.Road road)
            {
                return new Road
                {
                    Id = road.Id,
                    Type = road.Type,
                    Left = road.Left.Id,
                    Right = road.Right.Id,
                };
            }

            public static List<Road> Serialize(List<WorldStructures.Road> rivers)
            {
                return rivers.Select(it => Road.Serialize(it)).ToList();
            }
        }

        [System.Serializable]
        public struct River
        {
            public int RiverId;
            public int Id;
            public int Top;
            public int Bottom;
            public Edge Edge;
            public int Size;

            public static River Serialize(WorldStructures.River river)
            {
                return new River
                {
                    Id = river.Id,
                    Top = river.Top != null ? river.Top.Id : -1,
                    Bottom = river.Bottom != null ? river.Bottom.Id : -1,
                    Edge = Edge.Serialize(river.Edge),
                    Size = river.size,
                    RiverId = river.RiverId,
                };
            }

            public static List<River> Serialize(List<WorldStructures.River> rivers)
            {
                return rivers.Select(it => River.Serialize(it)).ToList();
            }
        }
    }
}