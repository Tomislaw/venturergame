using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

[RequireComponent(typeof(MeshRenderer), typeof(MeshFilter))]
public class DynamicTree : MonoBehaviour
{
    public float MaximumBend = 0.2f;
    public float RetractionForce = 5;
    public float Mass = 0.5f;
    public Sprite Sprite;
    public Material Material;
    private List<(Vector2, float)> initialVerticlesPosition = new List<(Vector2, float)>();

    private Vector2 initialResolverPosition;

    private GameObject bendPositionResolver;

    private Mesh mesh;

    private void Awake()
    {
        Load();
    }

    private void Start()
    {
        bendPositionResolver = ConfigureBendPositionResolver();
    }

    private void Load()
    {
        mesh = SpriteToMesh(Sprite);
        GetComponent<MeshFilter>().sharedMesh = mesh;

        float minVectorPos = float.MaxValue;
        float maxVectorPos = float.MinValue;

        foreach (var v in Sprite.vertices)
        {
            if (v.y > maxVectorPos)
                maxVectorPos = v.y;
            if (v.y < minVectorPos)
                minVectorPos = v.y;
        }
        float distance = (maxVectorPos - minVectorPos);

        initialVerticlesPosition.Clear();
        foreach (var v in Sprite.vertices)
        {
            var factor = v.y / distance;
            initialVerticlesPosition.Add((new Vector2(v.x, v.y), factor));
        }

        var renderer = GetComponent<MeshRenderer>();
        renderer.sharedMaterial = new Material(Material);
        renderer.sharedMaterial.SetTexture("_MainTex", Sprite.texture);
        hash = GetHashCode();
    }

    private int hash = 0;

    private void OnValidate()
    {
        if (hash != GetHashCode())
            Load();
    }

    private Mesh SpriteToMesh(Sprite sprite)
    {
        Mesh mesh = new Mesh();
        mesh.SetVertices(Array.ConvertAll(sprite.vertices, i => (Vector3)i).ToList());
        mesh.SetUVs(0, sprite.uv.ToList());
        mesh.SetTriangles(Array.ConvertAll(sprite.triangles, i => (int)i), 0);

        return mesh;
    }

    // Update is called once per frame
    private void LateUpdate()
    {
        Vector2 posOffset = (Vector2)bendPositionResolver.transform.localPosition - initialResolverPosition;
        var rb = bendPositionResolver.GetComponent<Rigidbody2D>(); ;

        if (posOffset.x > MaximumBend)
        {
            posOffset.x = MaximumBend;
            bendPositionResolver.transform.localPosition = initialResolverPosition + new Vector2(posOffset.x, 0);
            rb.velocity = new Vector2();
            rb.inertia = 0;
        }
        if (posOffset.x < -MaximumBend)
        {
            posOffset.x = -MaximumBend;
            bendPositionResolver.transform.localPosition = initialResolverPosition + new Vector2(posOffset.x, 0);
            rb.velocity = new Vector2();
            rb.inertia = 0;
        }

        if (rb.velocity.y >= 0)
            rb.velocity += new Vector2(-rb.velocity.x * Mathf.Min(1, Time.deltaTime * 5), 0);

        var currentPos = transform.position;

        var vertsCopy = mesh.vertices;

        for (int i = 0; i < initialVerticlesPosition.Count; i++)
        {
            var newPos = initialVerticlesPosition[i].Item1 + posOffset * initialVerticlesPosition[i].Item2;
            vertsCopy[i].x = newPos.x;
            vertsCopy[i].y = newPos.y;
        }
        mesh.vertices = vertsCopy;
        mesh.RecalculateBounds();
        mesh.RecalculateNormals();
        GetComponent<MeshFilter>().sharedMesh = mesh;
    }

    private GameObject ConfigureBendPositionResolver()
    {
        var go = new GameObject();
        go.name = name + "_bendPositionResolver";

        go.transform.parent = transform;
        go.transform.localPosition = new Vector2(0, 0 + Sprite.bounds.size.y);
        initialResolverPosition = go.transform.localPosition;
        var rg = go.AddComponent<Rigidbody2D>();
        rg.mass = Mass;
        //make tree stand instead of fall
        rg.gravityScale = -RetractionForce;
        var j = go.AddComponent<DistanceJoint2D>();
        j.connectedAnchor = transform.position;
        j.maxDistanceOnly = true;
        j.autoConfigureDistance = false;

        return go;
    }
}