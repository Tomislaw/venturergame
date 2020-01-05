using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using VoronoiLib.Structures;
using WorldStructures;

namespace WorldGenerator
{
    [System.Serializable]
    public struct SizeSettings
    {
        public Vector2Int Size;
        public Vector2Int Rows;
    }

    internal static class VEdgeExtension
    {
        public static Edge ToEdge(this VEdge vedge)
        {
            var edge = new Edge();
            edge.L = new Vector2Int((int)vedge.Start.X, (int)vedge.Start.Y);
            edge.R = new Vector2Int((int)vedge.End.X, (int)vedge.End.Y);
            edge.Left = (vedge.Left as ExtendedFortuneSite).Region;
            edge.Right = (vedge.Right as ExtendedFortuneSite).Region;
            return edge;
        }
    }

    public class ExtendedFortuneSite : FortuneSite
    {
        public Region Region;

        public ExtendedFortuneSite(double x, double y) : base(x, y)
        {
        }
    }

    public class VornoiRegionCreator
    {
        public int Seed = 0;
        public SizeSettings Settings;

        public List<Region> Generate()
        {
            return CreateRegions(GenerateSities());
        }

        private List<ExtendedFortuneSite> GenerateSities()
        {
            var points = new List<ExtendedFortuneSite>() { };
            var random = new System.Random(Seed);

            int margin = 2;

            float rowSizeX = (float)Settings.Size.x / (float)Settings.Rows.x;
            float rowSizeY = (float)Settings.Size.y / (float)Settings.Rows.y;

            for (int rx = 0; rx < Settings.Rows.x; rx++)
                for (int ry = 0; ry < Settings.Rows.y; ry++)
                {
                    int rsx = (int)Mathf.Ceil(rowSizeX * rx) + margin;
                    int rsy = (int)Mathf.Ceil(rowSizeY * ry) + margin;
                    var pointX = random.Next(rsx, rsx + (int)rowSizeX - 2 * margin);
                    var pointY = random.Next(rsy, rsy + (int)rowSizeY - 2 * margin);
                    points.Add(new ExtendedFortuneSite(pointX, pointY));
                }

            return points;
        }

        private List<Region> CreateRegions(List<ExtendedFortuneSite> cells)
        {
            var edges = VoronoiLib.FortunesAlgorithm.Run(cells.Cast<FortuneSite>().ToList(), 0, 0, Settings.Size.x, Settings.Size.y);
            foreach (var cell in cells)
            {
                cell.Region = new Region();
                cell.Region.Position = new Vector2Int((int)cell.X, (int)cell.Y);
            }
            foreach (var cell in cells)
                cell.Region.Neighbors = cell.Neighbors.Select((c) => (c as ExtendedFortuneSite).Region).ToList();

            foreach (var edge in edges)
            {
                var x1 = (int)edge.Start.X;
                var y1 = (int)edge.Start.Y;
                var x2 = (int)edge.End.X;
                var y2 = (int)edge.End.Y;

                int x, y;
                if (x1 == 0 || x2 == 0)
                {
                    (edge.Left as ExtendedFortuneSite).Region.Borders |= 1;
                    (edge.Right as ExtendedFortuneSite).Region.Borders |= 1;
                    x = 0;
                }
                if (x1 == Settings.Size.x || x2 == Settings.Size.x)
                {
                    (edge.Left as ExtendedFortuneSite).Region.Borders |= 2;
                    (edge.Right as ExtendedFortuneSite).Region.Borders |= 2;
                    x = Settings.Size.x;
                }
                if (y1 == 0 || y2 == 0)
                {
                    (edge.Left as ExtendedFortuneSite).Region.Borders |= 4;
                    (edge.Right as ExtendedFortuneSite).Region.Borders |= 4;
                    y = 0;
                }
                if (y1 == Settings.Size.y || y2 == Settings.Size.y)
                {
                    (edge.Left as ExtendedFortuneSite).Region.Borders |= 8;
                    (edge.Right as ExtendedFortuneSite).Region.Borders |= 8;
                    y = Settings.Size.y;
                }

                var redge = edge.ToEdge();
                (edge.Left as ExtendedFortuneSite).Region.Edges.Add(redge);
                (edge.Right as ExtendedFortuneSite).Region.Edges.Add(redge);
            }

            foreach (var cell in cells)
            {
                //cell.Region.Edges = GetEdges(cell);
                cell.Region.Center = GetCentroid(GetMesh(cell));
                if (cell.Region.Border)
                    cell.Region.Edges.AddRange(GetMissingEdges(cell));
            }

            var regions = cells.Select((cell) => cell.Region).ToList();
            return regions;
        }

        private List<Edge> GetMissingEdges(ExtendedFortuneSite site)
        {
            var poly = new List<Edge>();

            var edge2 = new List<Vector2Int>();
            foreach (var vedge in site.Cell)
            {
                var edge = vedge.ToEdge();

                if (edge.L.Equals(edge.R))
                    continue;

                if (edge.L.x <= 0 || edge.L.x >= Settings.Size.x || edge.L.y == 0 || edge.L.y >= Settings.Size.y)
                    edge2.Add(edge.L);
                if (edge.R.x <= 0 || edge.R.x >= Settings.Size.x || edge.R.y == 0 || edge.R.y >= Settings.Size.y)
                    edge2.Add(edge.R);

                poly.Add(edge);
            }

            if (edge2.Count == 2)
            {
                var edge = new Edge();
                edge.L = edge2[0];
                edge.R = edge2[1];
                poly.Add(edge);
            }

            //if (site.Region.Corner)
            //{
            //    int x = 0;
            //    int y = 0;

            //    if (site.Region.Borders % 2 == 0)
            //        x = Settings.Size.x;
            //    if (site.Region.Borders / 8 == 1)
            //        y = Settings.Size.y;
            //    poly.Add(new Vector2Int(x, y));
            //}

            //var center = site.Region.Center;
            //var list = poly.OrderBy((a => { return Mathf.Atan2(a.x - center.x, a.y - center.y); })).ToList();

            return poly;
        }

        private List<Vector2Int> GetMesh(ExtendedFortuneSite site)
        {
            var poly = new HashSet<Vector2Int>();

            foreach (var edge in site.Cell)
            {
                var a = new Vector2Int((int)edge.Start.X, (int)edge.Start.Y);
                var b = new Vector2Int((int)edge.End.X, (int)edge.End.Y);
                poly.Add(a);
                poly.Add(b);
            }

            if (site.Region.Corner)
            {
                int x = 0;
                int y = 0;

                if (site.Region.Borders % 2 == 0)
                    x = Settings.Size.x;
                if (site.Region.Borders / 8 == 1)
                    y = Settings.Size.y;
                poly.Add(new Vector2Int(x, y));
            }

            var center = site.Region.Center;
            var list = poly.OrderBy((a => { return Mathf.Atan2(a.x - center.x, a.y - center.y); })).ToList();

            return list;
        }

        private Vector2Int GetCentroid(List<Vector2Int> poly)
        {
            var result = poly.Aggregate(Vector2.zero, (current, next) => current + next);
            result /= poly.Count;
            return new Vector2Int((int)result.x, (int)result.y);
        }
    }
}