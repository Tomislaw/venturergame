using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using WorldGenerator;
using WorldStructures;

public static class WorldMapRenderer
{
    internal static Color BiomeColor(this WorldStructures.Serializable.Region region)
    {
        if (region.Water)
            return Color.clear;
        switch (region.biome)
        {
            case Biome.Bare: return new Color32(80, 80, 80, 255);
            case Biome.Beach: return new Color32(212, 178, 144, 255);
            case Biome.DeciduousForest: return new Color32(21, 176, 31, 255);
            case Biome.Ice: return new Color32(100, 252, 252, 255);
            //case Biome.Lake: return new Color32(34, 116, 230, 255);
            case Biome.Lake: return new Color32(255, 0, 0, 255);
            case Biome.MixedForests: return new Color32(31, 163, 90, 255);
            case Biome.Mountians: return new Color32(71, 71, 71, 255);
            case Biome.Ocean: return new Color32(26, 72, 171, 255);
            case Biome.Shrubland: return new Color32(113, 163, 124, 255);
            case Biome.Snow: return new Color32(216, 230, 228, 255);
            case Biome.SubtropicalDesert: return new Color32(247, 219, 154, 255);
            case Biome.Taiga: return new Color32(31, 163, 129, 255);
            case Biome.TemperateDesert: return new Color32(255, 255, 204, 255);
            case Biome.TropicalRainForest: return new Color32(100, 217, 35, 255);
            case Biome.Tundra: return new Color32(133, 204, 160, 255);
            case Biome.None: return new Color32(0, 0, 0, 255);
        }
        return new Color32(0, 0, 0, 0);
    }

    public static void Draw(this Region cell, ref Texture2D texture)
    {
        float h = ((1 + cell.Height) / 4f);
        float t = (cell.Temperature / 8f);
        //var color = new Color(0, h, 0);

        //if (cell.Water)
        //    color = new Color(0.1f, 0.1f, 1);

        //if (cell.ConnectedToOcean)
        //    color = new Color(0, 0, 0.7f);

        //if (cell.Border)
        //    color = new Color(0, 0, 0);
        var color =// cell.BiomeColor();
            Color.white;

        if (cell.Height > 0)
        {
            color *= (1f + cell.Height / 11f);
        }

        foreach (var river in cell.Rivers)
        {
            if (river.size == 0)
                river.Edge.Draw(ref texture, new Color(0, 0, 1));
            else
                river.Edge.Draw(ref texture, new Color(1, 0, 0));
        }

        cell.Center.Draw(ref texture, color);
        //cell.Edges.Draw(ref texture, color);
        //cell.Position.Draw(ref texture, color);
    }

    public static void Draw(this List<Vector2Int> mesh, ref Texture2D texture, Color color)
    {
        for (int i = 0; i < mesh.Count; i++)
        {
            int j = (i == mesh.Count - 1) ? 0 : i + 1;
            DrawLine(mesh[i], mesh[j], ref texture, color);
        }
    }

    public static void Draw(this Edge edge, ref Texture2D texture, Color color)
    {
        DrawLine(edge.L, edge.R, ref texture, color);
    }

    public static void Draw(this IEnumerable<Edge> mesh, ref Texture2D texture, Color color)
    {
        foreach (var edge in mesh)
            DrawLine(edge.L, edge.R, ref texture, color);
    }

    public static void DrawLine(Vector2Int point1, Vector2Int point2, ref Texture2D texture, Color color)
    {
        int x0 = (int)point1.x;
        int y0 = (int)point1.y;

        int x1 = (int)point2.x;
        int y1 = (int)point2.y;

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

    public static void Draw(this Vector2Int point1, ref Texture2D texture, Color color)
    {
        for (int x = (int)point1.x - 3; x < point1.x + 3; x++)
            for (int y = (int)point1.y - 3; y < point1.y + 3; y++)
                texture.SetPixel(x, y, color);
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

[ExecuteInEditMode]
public class WorldRenderer : MonoBehaviour
{
    public WorldMap World;
    public Material Material;

    //private Texture2D m_map;
    private Mesh mesh;

    public bool UseCustomRenderer = false;

    public int PixelsPerUnit = 32;

    private int worldHash = 0;

    public void Reload()
    {
        if (World == null)
            return;

        var regions = World.Regions;

        var meshes = GenerateMeshes(regions).ToList();
        CombineInstance[] combine = new CombineInstance[meshes.Count];
        for (int i = 0; i < meshes.Count; i++)
        {
            combine[i].mesh = meshes[i];
            combine[i].transform = Matrix4x4.identity;
        }
        mesh = new Mesh();
        mesh.CombineMeshes(combine, true);

        var filter = GetComponent<MeshFilter>();
        if (filter != null)
        {
            filter.sharedMesh = mesh;
            filter.sharedMesh.name = "worldmap";
        }
    }

    private void Start()
    {
        Reload();
    }

    private void OnValidate()
    {
        Reload();
    }

    // Update is called once per frame
    private void Update()
    {
        if (World == null)
            return;

        if (worldHash != World.GetHashCode())
        {
            worldHash = World.GetHashCode();
            Reload();
        }

        //foreach (var mesh in meshes)
        if (UseCustomRenderer)
            Graphics.DrawMesh(mesh, transform.localToWorldMatrix, Material, 0);

        //var matrix = gameObject.transform.localToWorldMatrix;
        //foreach (var mesh in meshes)
        //    Graphics.DrawMesh(mesh, matrix, Material, 31);
    }

    private void OnGUI()
    {
        //GUI.DrawTexture(new Rect(10, 10, 350, 350), m_map);
    }

    private Mesh GenerateMesh(WorldStructures.Serializable.Region region)
    {
        var mesh = new Mesh();
        var indices = new List<int>();
        var verticles = new List<Vector3>();
        verticles.Add(new Vector3(region.Center.x, region.Center.y, 0) / (float)PixelsPerUnit);

        foreach (var edge in region.Edges)
        {
            if (ClockWiseTurns(region.Center, edge.L, edge.R) > 0)
            {
                indices.Add(0);
                indices.Add(verticles.Count);
                verticles.Add(new Vector3(edge.R.x, edge.R.y, 0) / (float)PixelsPerUnit);
                indices.Add(verticles.Count);
                verticles.Add(new Vector3(edge.L.x, edge.L.y, 0) / (float)PixelsPerUnit);
            }
            else
            {
                indices.Add(0);
                indices.Add(verticles.Count);
                verticles.Add(new Vector3(edge.L.x, edge.L.y, 0) / (float)PixelsPerUnit);
                indices.Add(verticles.Count);
                verticles.Add(new Vector3(edge.R.x, edge.R.y, 0) / (float)PixelsPerUnit);
            }
        }

        mesh.vertices = verticles.ToArray();
        mesh.triangles = indices.ToArray();
        mesh.uv = verticles.Select((v) => new Vector2(v.x, v.y)).ToArray();

        //float h = ((1 + region.Height) / 4f);
        //float h = region.Moisture;
        //var color = new Color(0, h, 0);

        //if (region.Water)
        //    color = new Color(0.1f, 0.1f, 1);

        //if (region.ConnectedToOcean)
        //    color = new Color(0, 0, 0.7f);
        var color = region.BiomeColor();
        var a = color.a;

        // make color darker or lighter depenting on height
        //if (region.Height > 0)
        //    color *= 1f + (region.Height / World.MinHeight);
        //else if (region.Height < 0)
        //    color *= 1f - Mathf.Abs(region.Height) / World.MinHeight;

        color.a = a;

        mesh.colors = mesh.vertices.Select((vector) => color).ToArray();
        mesh.RecalculateNormals();
        return mesh;
    }

    private IEnumerable<Mesh> GenerateMeshes(IEnumerable<WorldStructures.Serializable.Region> regions)
    {
        foreach (var region in regions)
            yield return GenerateMesh(region);
    }

    public static int ClockWiseTurns(Vector2Int a, Vector2Int b, Vector2Int c)
    {
        return (b.x - a.x) * (c.y - a.y) - (c.x - a.x) * (b.y - a.y);
    }
}