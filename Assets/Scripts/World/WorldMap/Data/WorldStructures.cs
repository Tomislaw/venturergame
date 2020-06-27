using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

namespace WorldStructures
{
    public class Region
    {
        public Region()
        {
        }

        public List<River> Rivers = new List<River>();
        public List<Region> Neighbors = new List<Region>();
        public List<Road> Roads = new List<Road>();
        public Vector2Int Position;
        public Vector2Int Center;

        public List<Edge> Edges = new List<Edge>();

        public int Id = 0;
        public Vector2Int RegionId;

        public int Height = -1;
        public float Moisture = 0;
        public float Temperature = 0;
        public Biome biome = Biome.None;
        public Structure structure = Structure.None;

        internal int Borders = 0;
        public bool Corner { get => (Borders % 4) > 0 && ((Borders >> 2) % 4) > 0; }
        public bool Border { get => Borders > 0; }
        public bool Coast { get => Neighbors.Find(r => r.Water) != null && !Water; }
        public bool Water = true;
        public bool ConnectedToOcean = false;
    }

    public class Edge
    {
        public Edge()
        {
        }

        public Edge(Vector2Int L, Vector2Int R)
        {
            this.L = L;
            this.R = R;
        }

        public Vector2Int L;
        public Vector2Int R;
        public Region Left = null;
        public Region Right = null;

        public override string ToString()
        {
            return "{" + L + "," + R + "}";
        }

        public override bool Equals(object obj)
        {
            if (obj is Edge)
            {
                return (obj as Edge).L == L && (obj as Edge).R == R;
            }
            return false;
        }
    }

    public class Road
    {
        public int Id = 0;
        public int Type = 0;
        public Region Left = null;
        public Region Right = null;

        public Road(Region a, Region b)
        {
            if (a.RegionId.x < b.RegionId.x || a.RegionId.y < b.RegionId.y)
            {
                Left = a;
                Right = b;
            }
            else
            {
                Left = b;
                Right = a;
            }
        }
    }

    public class River
    {
        public int RiverId = 0;
        public int Id = 0;
        public River Top;
        public River Bottom;
        public Edge Edge;
        public int size = 1;
    }

    public enum Biome
    {
        None,
        Ocean,
        Lake,
        Ice,
        Beach,
        Snow,
        Tundra,
        Bare,
        Taiga,
        Shrubland,
        MixedForests,
        TemperateDesert,
        DeciduousForest,
        TropicalRainForest,
        SubtropicalDesert,
        Mountians
    }

    public enum Structure
    {
        None,
        IronDeposit,
        SaltDeposit,
        Village,
        Town,
        TownWithPort
    }
}