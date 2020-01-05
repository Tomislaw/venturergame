using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[ExecuteAlways]
[RequireComponent(typeof(ExpensiveGrass))]
public class ExpensiveGrassRenderer : MonoBehaviour
{
    private ExpensiveGrass expensiveGrass;
    private Texture2D texture;
    private Mesh mesh;
    private Material material;

    public Material debugLineMat;

    public int width = 32;
    public int height = 32;
    public int freeSpaceXOffset = 32;
    public Shader shader;

    private void Start()
    {
        texture = new Texture2D(width + freeSpaceXOffset * 2, height);
        texture.filterMode = FilterMode.Point;
        expensiveGrass = GetComponent<ExpensiveGrass>();
        mesh = CreateMesh();
        material = new Material(shader);
        material.mainTexture = texture;
    }

    // Update is called once per frame
    private void Update()
    {
        Draw();

        Graphics.DrawMesh(mesh, transform.localToWorldMatrix, material, 0);
    }

    private void Draw()
    {
        texture.SetAllPixels(new Color32(0, 0, 0, 0));

        var linesPoints = expensiveGrass.GetLines();
        for (int i = 0; i < linesPoints.Count; i += 2)
        {
            var a = linesPoints[i].Item1;
            var b = linesPoints[i + 1].Item1;
            var c = linesPoints[i].Item2;
            DrawLine(ToLocalIntVector(a), ToLocalIntVector(b), ref texture, c);
        }
        texture.Apply();
    }

    //public void OnRenderObject()
    //{
    //    var linesPoints = expensiveGrass.GetLines();
    //    for (int i = 0; i < linesPoints.Count; i += 2)
    //    {
    //        var a = linesPoints[i].Item1;
    //        var b = linesPoints[i + 1].Item1;
    //        var c = linesPoints[i].Item2;
    //        //DrawLine(ToLocalIntVector(a), ToLocalIntVector(b), ref texture, c);

    //        GL.Begin(GL.LINES);
    //        debugLineMat.SetPass(0);
    //        GL.Color(c);
    //        GL.Vertex(a);
    //        GL.Vertex(b);
    //        GL.End();
    //    }
    //}

    public Vector2Int ToLocalIntVector(Vector2 vector)
    {
        var v = (vector - (Vector2)transform.position) * (float)(width) / expensiveGrass.Width;
        return new Vector2Int((int)v.x + freeSpaceXOffset, (int)v.y);
    }

    private void DrawLine(Vector2Int point1, Vector2Int point2, ref Texture2D texture, Color color)
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

    private Mesh CreateMesh()
    {
        var mesh = new Mesh();

        float offsetSize = freeSpaceXOffset / width * expensiveGrass.Width;
        float start = -offsetSize;
        float end = expensiveGrass.Width + offsetSize;
        var vertices = new Vector3[4]
        {
            new Vector3(start, 0, 0),
            new Vector3(end, 0, 0),
            new Vector3(start, 1, 0),
            new Vector3(end, 1, 0)
        };
        mesh.vertices = vertices;

        var tris = new int[6]
        {
            // lower left triangle
            0, 1, 2,
            // upper right triangle
            1, 2, 3
        };
        mesh.triangles = tris;

        var normals = new Vector3[4]
        {
            -Vector3.forward,
            -Vector3.forward,
            -Vector3.forward,
            -Vector3.forward
        };
        mesh.normals = normals;

        var uv = new Vector2[4]
        {
            new Vector2(0, 0),
            new Vector2(1, 0),
            new Vector2(0, 1),
            new Vector2(1, 1)
        };
        mesh.uv = uv;
        return mesh;
    }

    private void OnDrawGizmosSelected()
    {
        float offsetSize = freeSpaceXOffset / width * expensiveGrass.Width;
        var start = new Vector2(transform.position.x - offsetSize, transform.position.y);
        var size = new Vector2(expensiveGrass.Width, expensiveGrass.Height);
        size += new Vector2(offsetSize * 2, 0);

        Gizmos.DrawCube(start + size / 2, size);

        Gizmos.color = Color.white;
    }
}