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
    private Material _material;

    public int width = 32;
    public int height = 32;
    public int freeSpaceXOffsetLeft = 32;
    public int freeSpaceXOffsetRight = 32;
    public Material material;

    private void Awake()
    {
        enabled = false;
    }

    private void Start()
    {
        texture = new Texture2D(width + freeSpaceXOffsetLeft + freeSpaceXOffsetRight, height);
        texture.filterMode = FilterMode.Point;
        expensiveGrass = GetComponent<ExpensiveGrass>();
        mesh = CreateMesh();
        _material = new Material(material);
        _material.mainTexture = texture;
    }

    private void OnBecameVisible()
    {
        enabled = true;
        Graphics.DrawMesh(mesh, transform.localToWorldMatrix, _material, 0);
    }

    private void OnBecameInvisible()
    {
        enabled = false;
    }

    // Update is called once per frame
    private void Update()
    {
        Draw();
        Graphics.DrawMesh(mesh, transform.localToWorldMatrix, _material, 0);
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
        if (v.x < -freeSpaceXOffsetLeft)
            v.x = -freeSpaceXOffsetLeft;
        else if (v.x > width + freeSpaceXOffsetRight)
            v.x = width + freeSpaceXOffsetRight;
        if (v.y < 0)
            v.y = 0;
        else if (v.y > height)
            v.y = height;

        return new Vector2Int((int)v.x + freeSpaceXOffsetLeft, (int)v.y);
    }

    private void DrawLine(Vector2Int point1, Vector2Int point2, ref Texture2D texture, Color color)
    {
        int x0 = point1.x;
        int y0 = point1.y;

        int x1 = point2.x;
        int y1 = point2.y;

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

        float offsetSizeL = (float)(freeSpaceXOffsetLeft) / width * expensiveGrass.Width;
        float offsetSizeR = (float)(freeSpaceXOffsetRight) / width * expensiveGrass.Width;
        float start = -offsetSizeL;
        float end = expensiveGrass.Width + offsetSizeR;
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
        float offsetSizeL = (float)(freeSpaceXOffsetLeft) / (float)width * expensiveGrass.Width;
        float offsetSizeR = (float)(freeSpaceXOffsetRight) / (float)width * expensiveGrass.Width;
        var start = new Vector2(transform.position.x - offsetSizeL, transform.position.y);
        var size = new Vector2(expensiveGrass.Width, 1);
        size += new Vector2(offsetSizeL + offsetSizeR, 0);

        Gizmos.DrawCube(start + size / 2, size);

        Gizmos.color = Color.white;
    }
}