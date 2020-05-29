using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using WorldStructures;

namespace LineHelper
{
    internal static class Liner
    {
        public static (Vector2, Vector2) Neighbours(float width, Vector2 start, Vector2 end, Vector2? previous = null)
        {
            Vector2 begin = previous == null ? start : previous.Value;
            var perp = Vector2.Perpendicular(end - begin).normalized;

            return (start + perp * width, start - perp * width);
        }

        public static Mesh LineMesh(List<Vector2> linePoints, Color color, float width, float scale)
        {
            var mesh = new Mesh();

            var points = new List<Vector3>();
            var indices = new List<int>();
            var colors = new List<Color>();

            Vector2? previous = null;
            Vector2 start = linePoints.First();

            for (int i = 0; i < linePoints.Count; i++)
            {
                var (a, b) = i != linePoints.Count - 1
                    ? Neighbours(width, linePoints[i], linePoints[i + 1], previous)
                    : Neighbours(width, linePoints[i], linePoints[i - 1], null);

                points.Add(a * scale);
                points.Add(b * scale);
                indices.Add(indices.Count);
                indices.Add(indices.Count);
                previous = linePoints[i];
                colors.Add(color);
                colors.Add(color);
            }

            mesh.vertices = points.ToArray();
            mesh.SetIndices(indices.ToArray(), MeshTopology.Quads, 0);
            mesh.SetColors(colors);
            mesh.RecalculateNormals();
            mesh.Optimize();
            mesh.UploadMeshData(false);
            return mesh;
        }
    }
}

[ExecuteInEditMode]
[RequireComponent(typeof(MeshFilter))]
public class WorldRiversRenderer : MonoBehaviour
{
    public WorldMap World;
    public Material Material;
    public int PixelsPerUnit = 32;
    public float RiverWidth = 2;
    public Color Color;

    public bool UseCustomRenderer = false;

    private Mesh mesh;
    private int worldHash = 0;

    // Start is called before the first frame update
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

        if (UseCustomRenderer)
            Graphics.DrawMesh(mesh, transform.localToWorldMatrix, Material, 0);
    }

    public void Reload()
    {
        if (World == null)
            return;

        var meshes = GenerateMeshes(World.Regions).ToList();
        CombineInstance[] combine = new CombineInstance[meshes.Count];
        for (int i = 0; i < meshes.Count; i++)
        {
            combine[i].mesh = meshes[i];
            combine[i].transform = Matrix4x4.identity;
        }
        mesh = new Mesh();
        try
        {
            mesh.CombineMeshes(combine, true);
        }
        catch (Exception e)
        {
            Debug.LogError(e);
        }
        //mesh.CombineMeshes(combine, true);

        var filter = GetComponent<MeshFilter>();
        if (filter != null)
        {
            filter.sharedMesh = mesh;
            filter.sharedMesh.name = "worldmap_rivers";
        }
    }

    private IEnumerable<Mesh> GenerateMeshes(IEnumerable<Region> regions)
    {
        foreach (var region in regions)
            foreach (var river in region.Rivers)
            {
                var mesh = LineHelper.Liner.LineMesh(new List<Vector2>() { river.Edge.L, river.Edge.R }, Color, RiverWidth, 1 / (float)PixelsPerUnit);

                yield return mesh;
                //var mesh = new Mesh();
                //var indices = new[] { 0, 1 };
                //var verticles = new List<Vector3>()
                //{
                //    new Vector3(river.Edge.L.x, river.Edge.L.y, 0) / (float)PixelsPerUnit,
                //    new Vector3(river.Edge.R.x, river.Edge.R.y, 0) / (float)PixelsPerUnit
                //};
                //var colors = new List<Color>() { Color, Color };
                //mesh.SetVertices(verticles);
                //mesh.SetIndices(indices, MeshTopology.LineStrip, 0);
                //mesh.SetColors(colors);
                //yield return mesh;
            }
    }
}