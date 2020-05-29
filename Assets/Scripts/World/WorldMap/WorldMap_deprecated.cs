using System.Collections;
using System.Collections.Generic;
using DelaunatorSharp.Interfaces;
using DelaunatorSharp.Models;
using UnityEngine;
using System.Linq;
using System;
using WorldMapGenerator;

namespace WorldMapGenerator
{
    public static class RegionExtension
    {
        public static bool Adjecent(this Region region, Region region2)
        {
            foreach (var point in region.Points)
                if (region2.Points.Contains(point))
                    return true;
            return false;
        }
    }

    public class Region
    {
        public IPoint Position;
        public IPoint Center;
        public List<IPoint> Points = new List<IPoint>();
        public List<Region> AdjecentRegions = new List<Region>();

        private int height = -1;

        public Region()
        {
        }

        private void PropagateHeight()
        {
            foreach (var r in AdjecentRegions)
            {
                if (r.height >= height - 1)
                    continue;
                else
                {
                    r.height = height - 1;
                    r.PropagateHeight();
                }
            }
        }

        public static List<Region> GenerateRegions(int seed, int rowsX, int rowsY, int width, int height)
        {
            IEnumerable<IPoint> points = Helper.GeneratePoints(seed, rowsX, rowsY, width, height);
            var delaunator = new DelaunatorSharp.Delaunator(points);

            List<Region> regions = new List<Region>();
            var edges = delaunator.GetVoronoiEdges();
            var cells = delaunator.GetVoronoiCells();
            Debug.Log("cells: " + cells.Count());
            foreach (var cell in cells)
            {
                var point = delaunator.Points.ElementAt(cell.Index);
                if (point.X == 0 || point.X == width || point.Y == 0 || point.Y == height)
                    continue;
                //hack for removing incorrect cells
                //if (cell.Points.Count() == 3)
                //    if (!Helper.IsValidEdges(cell.Points, edges))
                //       continue;
                var newRegion = new Region();
                newRegion.Points = cell.Points.ToList();
                newRegion.Center = delaunator.GetCentroid(cell.Points); ;
                newRegion.Position = point;

                foreach (var region in regions)
                    if (region.Adjecent(newRegion))
                    {
                        newRegion.AdjecentRegions.Add(region);
                        region.AdjecentRegions.Add(newRegion);
                    }
                regions.Add(newRegion);
            }

            var rnd = new System.Random(seed);

            int randomStart = rnd.Next(0, regions.Count());

            var startingRegion = regions.ElementAt(randomStart);
            startingRegion.height = 2;
            startingRegion.PropagateHeight();

            return regions;
        }

        public void Draw(ref Texture2D texture, Color color)
        {
            var colorr = height == -1 ? color : Color.red * (height / 4f);
            Extension.DrawConvex(Points, ref texture, colorr);
            Center.Draw(ref texture, colorr);
            //Position.Draw(ref texture, color);
        }

        public override string ToString()
        {
            string str = "{C: " + Center.X + "," + Center.Y
                + "\n P: " + Position.X + "," + Position.Y
                + "\npoints: ";
            foreach (var p in Points)
                str += "" + p.X + "," + p.Y + ";";
            str += "}";
            return str;
        }
    }

    public static class Extension
    {
        public static void Draw(this IVoronoiCell cell, ref Texture2D texture, Color color)
        {
            DrawConvex(cell.Points, ref texture, color);
        }

        public static void DrawConvex(IEnumerable<IPoint> points, ref Texture2D texture, Color color)
        {
            for (int i = 0; i < points.Count(); i++)
            {
                int j = i == points.Count() - 1 ? 0 : i + 1;
                DrawLine(points.ElementAt(i), points.ElementAt(j), ref texture, color);
            }
        }

        public static void DrawLine(IPoint point1, IPoint point2, ref Texture2D texture, Color color)
        {
            int x0 = (int)point1.X;
            int y0 = (int)point1.Y;

            int x1 = (int)point2.X;
            int y1 = (int)point2.Y;

            int dx = Mathf.Abs(x1 - x0), sx = x0 < x1 ? 1 : -1;
            int dy = Mathf.Abs(y1 - y0), sy = y0 < y1 ? 1 : -1;
            int err = (dx > dy ? dx : -dy) / 2, e2;

            for (; ; )
            {
                texture.SetPixel(x0, y0, color);
                if (x0 == x1 && y0 == y1) break;
                e2 = err;
                if (e2 > -dx) { err -= dy; x0 += sx; }
                if (e2 < dy) { err += dx; y0 += sy; }
            }
        }

        public static void Draw(this IPoint point1, ref Texture2D texture, Color color)
        {
            for (int x = (int)point1.X - 3; x < point1.X + 3; x++)
                for (int y = (int)point1.Y - 3; y < point1.Y + 3; y++)
                    texture.SetPixel(x, y, color);
        }

        public static void Draw(this ITriangle triangle, ref Texture2D texture, Color color)
        {
            IPoint p1 = triangle.Points.ElementAt(0);
            IPoint p2 = triangle.Points.ElementAt(1);
            IPoint p3 = triangle.Points.ElementAt(2);

            DrawLine(p1, p2, ref texture, color);
            DrawLine(p2, p3, ref texture, color);
            DrawLine(p3, p1, ref texture, color);
        }

        public static void Draw(this IEdge edge, ref Texture2D texture, Color color)
        {
            DrawLine(edge.P, edge.Q, ref texture, color);
        }

        public static void SetAllPixels(this Texture2D texture, Color32 color)
        {
            Color32[] resetColorArray = texture.GetPixels32();

            for (int i = 0; i < resetColorArray.Length; i++)
            {
                resetColorArray[i] = color;
            }

            texture.SetPixels32(resetColorArray);
            texture.Apply();
        }
    }

    public static class Helper
    {
        public static IEnumerable<IPoint> GeneratePoints(int seed, int rowsX, int rowsY, int maxX, int maxY)
        {
            var points = new List<IPoint>()
        {
            new Point(0, 0),
            new Point(0, maxY),
            new Point(maxX, maxY),
            new Point(maxX, 0)
        };
            var random = new System.Random(seed);

            int margin = 2;

            float rowSizeX = (float)maxX / (float)rowsX;
            float rowSizeY = (float)maxY / (float)rowsY;

            for (int rx = 0; rx < rowsX; rx++)
                for (int ry = 0; ry < rowsY; ry++)
                {
                    int rsx = (int)Mathf.Ceil(rowSizeX * rx) + margin;
                    int rsy = (int)Mathf.Ceil(rowSizeY * ry) + margin;
                    var pointX = random.Next(rsx, rsx + (int)rowSizeX - 2 * margin);
                    var pointY = random.Next(rsy, rsy + (int)rowSizeY - 2 * margin);
                    points.Add(new Point(pointX, pointY));
                }

            return points;
        }

        public static bool SameEdge(this IEdge edge, IPoint p1, IPoint p2)
        {
            return edge.P.Equals(p1) && edge.Q.Equals(p2) || edge.P.Equals(p2) && edge.Q.Equals(p1);
        }

        public static bool IsValidEdges(IEnumerable<IPoint> points, IEnumerable<IEdge> edges)
        {
            for (int i = 0; i < points.Count(); i++)
            {
                int j = i == points.Count() - 1 ? 0 : i + 1;
                if (!edges.Any((edge) => { return edge.SameEdge(points.ElementAt(i), points.ElementAt(j)); }))
                    return false;
            }
            return true;
        }

        public static bool IsInPolygon(this IPoint testPoint, IEnumerable<IPoint> polygon)
        {
            bool result = false;
            int j = polygon.Count() - 1;
            for (int i = 0; i < polygon.Count(); i++)
            {
                if (polygon.ElementAt(i).Y < testPoint.Y && polygon.ElementAt(j).Y >= testPoint.Y || polygon.ElementAt(j).Y < testPoint.Y && polygon.ElementAt(i).Y >= testPoint.Y)
                {
                    if (polygon.ElementAt(i).X + (testPoint.Y - polygon.ElementAt(i).Y) / (polygon.ElementAt(j).Y - polygon.ElementAt(i).Y) * (polygon.ElementAt(j).X - polygon.ElementAt(i).X) < testPoint.X)
                    {
                        result = !result;
                    }
                }
                j = i;
            }
            return result;
        }

        //public static IEnumerable<DelaunatorSharp.Interfaces.IPoint> GeneratePoints(int seed, int amount, int maxX, int maxY)
        //{
        //    var spacingX = 5;
        //    var spacingY = 5;

        //    var points = new List<DelaunatorSharp.Interfaces.IPoint>() { };

        //    var random = new System.Random(seed);

        //    var rowCount = (int)Mathf.Sqrt(amount);
        //    var pointRangeX = maxX / rowCount;
        //    var pointRangeY = maxY / rowCount;
        //    int i = 0, y = 0;

        //    while (i < amount)
        //    {
        //        for (int x = 0; x < rowCount; x++)
        //        {
        //            var pointX = random.Next(spacingX + x * pointRangeX, x * pointRangeX + pointRangeX - spacingX);
        //            var pointY = random.Next(spacingY + y * pointRangeY, y * pointRangeY + pointRangeY - spacingY);

        //            points.Add(new DelaunatorSharp.Models.Point(pointX, pointY));

        //            i++;
        //            if (i == amount)
        //                break;
        //        }
        //        y++;
        //    }

        //    return points;
        //}
    }
}

public class WorldMap_deprecated : MonoBehaviour
{
    public int Width = 256;
    public int Height = 256;
    public int RowsX = 16;
    public int RowsY = 16;
    public int Seed = 0;
    private Texture2D m_map;

    private void Start()
    {
        m_map = new Texture2D(Width, Height);
        m_map.SetAllPixels(Color.blue);

        var regions = Region.GenerateRegions(Seed, RowsX, RowsY, Width, Height);
        foreach (var region in regions)
            region.Draw(ref m_map, Color.black);
        m_map.Apply();
        //foreach (var edge in ds.GetVoronoiEdges())
        //    edge.Draw(ref m_map, Color.black);

        //foreach (var edge in ds.Points)
        //    edge.Draw(ref m_map, Color.black);

        //foreach (var edge in ds.GetRellaxedPoints())
        //    edge.Draw(ref m_map, Color.red);
        //foreach (var edge in ds.GetVoronoiEdges())
        //    edge.Draw(ref m_map, Color.black);

        //foreach (var edge in points)
        //   edge.Draw(ref m_map, Color.black);

        //foreach (var r in regions)
        //   r.Draw(ref m_map, Color.black);

        //regions[5].Draw(ref m_map, Color.red);
        //foreach (var region in regions)
        //{
        //    foreach (var region2 in region.AdjecentRegions)
        //        if (region2.Center.IsInPolygon(region2.Points))
        //            Debug.Log("invalid region:" + region.Center);
        //}

        //var cellx = ds.Points.ElementAt(30);
        //cellx.Draw(ref m_map, Color.red);

        //ds.GetVoronoiCells().First((item) => item.Index == 30).Draw(ref m_map, Color.red);
        //var edges = ds.EdgesAroundPoint(30);
        //var triangles = edges.Select(x => ds.TriangleOfEdge(x));
        //var vertices = triangles.Select(x => ds.GetTriangleCenter(x));
        //Extension.DrawConvex(points, ref m_map, Color.yellow);
        //ds.
        //foreach (var edge
        //    in ds.GetEdges()
        //    .Select((value, id) => (value, id))
        //    .Where((item) => { return ed.Contains(item.id); })
        //    .Select(item => (item.value)))
        //{
        //    edge.Draw(ref m_map, Color.red);
        //}

        //foreach (var triangle in ds.GetEdges())
        //    triangle.Draw(ref m_map, Color.grey);
        //foreach (var cell in ds.GetVoronoiCells())
        //    cell.Draw(ref m_map, Color.red);
        //foreach (var point in points)
        //    point.Draw(ref m_map, Color.black);
    }

    // Update is called once per frame
    private void Update()
    {
    }

    private void OnGUI()
    {
        GUI.DrawTexture(new Rect(10, 10, 300, 300), m_map);
    }
}