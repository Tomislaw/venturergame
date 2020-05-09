using Assets.Scripts.Chunk.ChunkDecorators;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class Rigidbody2DExtension
{
    public static void AddExplosionForce(this Rigidbody2D body, float explosionForce, Vector3 explosionPosition, float explosionRadius)
    {
        Vector2 dir = (body.transform.position - explosionPosition);
        float wearoff = 1 - (dir.magnitude / explosionRadius);
        if (wearoff < 0)
            return;
        body.AddForce(dir.normalized * explosionForce * wearoff);
    }

    public static void AddExplosionForce(this Rigidbody2D body, float explosionForce, Vector3 explosionPosition, float explosionRadius, float upliftModifier)
    {
        Vector2 dir = (body.transform.position - explosionPosition);
        float wearoff = 1 - (dir.magnitude / explosionRadius);
        if (wearoff < 0)
            return;

        Vector3 baseForce = dir.normalized * explosionForce * wearoff;
        body.AddForce(baseForce);

        float upliftWearoff = 1 - upliftModifier / explosionRadius;
        Vector3 upliftForce = Vector2.up * explosionForce * upliftWearoff;
        body.AddForce(upliftForce);
    }
}

[RequireComponent(typeof(DistanceJoint2D))]
public class ExpensiveGrassStalk : MonoBehaviour
{
    public Color color;
    public Vector2 attach;
    public float length = 1;

    private Rigidbody2D rb;

    private void Start()
    {
        rb = GetComponent<Rigidbody2D>();
    }

    private void Update()
    {
        //dont let grass fall into ground
        if (transform.position.y <= attach.y)
        {
            transform.position = new Vector2(transform.position.x, attach.y);
            rb.velocity = new Vector2();
        }
        //slowdown
        if (rb.velocity.y >= 0)
            rb.velocity += new Vector2(-rb.velocity.x * Mathf.Min(1, Time.deltaTime * 5), 0);
    }

    public List<(Vector2, Color)> GetLineVertices()
    {
        var line = new List<(Vector2, Color)>();
        line.Add((attach, color - new Color(0.2f, 0.2f, 0.2f, 0)));
        var p1 = Vector2.Lerp(attach, transform.position, 0.2f);
        p1 += new Vector2(0, (transform.position.y - p1.y) * 0.3f);
        var p2 = Vector2.Lerp(attach, transform.position, 0.6f);
        p2 += new Vector2(0, (transform.position.y - p2.y) * 0.5f);

        line.Add((p1, color - new Color(0.2f, 0.2f, 0.2f, 0)));
        line.Add((p1, color - new Color(0.1f, 0.1f, 0.1f, 0)));
        line.Add((p2, color - new Color(0.1f, 0.1f, 0.1f, 0)));
        line.Add((p2, color));
        line.Add((transform.position, color));
        return line;
    }
}

public class ExpensiveGrass : MonoBehaviour, IChunkDecoratorObject
{
    public static int StalkCount = 32;
    public float Width = 1;
    public float Height = 1;
    public float MaxNoise = 0.1f;

    public List<Color> StalkColors = new List<Color>() { Color.green };
    public float StalkMass = 0.1f;
    public int Seed = 0;
    public bool RandomSeed = true;

    public float strenght = 2;

    public bool CornerLeft = false;
    public bool CornerRight = false;

    private List<GameObject> stalks = new List<GameObject>();

    private Vector3 previousPosition;

    // Start is called before the first frame update

    private void Start()
    {
        previousPosition = transform.position;
        GenerateGrass();
    }

    // Update is called once per frame
    private void Update()
    {
        if (previousPosition != transform.position)
        {
            GenerateGrass();
            previousPosition = transform.position;
        }
        //if (transform.hasChanged)
        //{
        //    int i = 0;
        //    foreach (var stalk in stalks)
        //    {
        //        var joint = stalk.GetComponent<DistanceJoint2D>();
        //        joint.anchor = transform.position + new Vector3(Width * i / (StalkCount) + (Width * 1 / StalkCount) / 2, 0, 0);
        //        stalk.transform.position = joint.anchor + new Vector2(0, joint.distance);
        //        i++;
        //    }
        //}
    }

    private void GenerateGrass()
    {
        if (RandomSeed)
            Seed = Random.Range(int.MinValue, int.MaxValue);

        var _seed = GetComponent<ChunkDecorator>()?.GetProperty("seed", Seed);
        if (_seed.HasValue)
            Seed = _seed.Value;
        var rnd = new System.Random(Seed);
        stalks.Clear();

        for (int i = 0; i < StalkCount; i++)
        {
            float stalkHeight = Height;
            if (CornerLeft && i < StalkCount / 2)
                stalkHeight *= (float)i / (StalkCount / 2);
            else if (CornerRight && i > StalkCount / 2)
                stalkHeight *= (float)(StalkCount - i) / (StalkCount / 2);

            stalkHeight += (float)rnd.NextDouble() * MaxNoise - MaxNoise / 2;
            stalkHeight = Mathf.Abs(stalkHeight);

            var go = CreateStalk(
                Width * i / (StalkCount) + (Width * 1 / StalkCount) / 2,
                StalkColors[rnd.Next(0, StalkColors.Count - 1)],
                stalkHeight);

            go.name = name + "_" + "stalk" + i;
            stalks.Add(go);
        }
    }

    private GameObject CreateStalk(float xPos, Color col, float length)
    {
        var pos = transform.position + new Vector3(xPos, 0, 0);
        var go = new GameObject();
        go.transform.localPosition = new Vector2(pos.x, pos.y + length);
        go.transform.parent = transform;

        var rg = go.AddComponent<Rigidbody2D>();
        rg.mass = StalkMass;
        //make grass stand instead of fall
        rg.gravityScale = -strenght;
        var j = go.AddComponent<DistanceJoint2D>();
        j.connectedAnchor = pos;
        j.distance = length;
        j.maxDistanceOnly = true;
        j.autoConfigureDistance = false;
        var egs = go.AddComponent<ExpensiveGrassStalk>();
        egs.color = col;
        egs.attach = pos;
        egs.length = length;

        return go;
    }

    public List<(Vector2, Color)> GetLines()
    {
        var lines = new List<(Vector2, Color)>();

        foreach (var child in GetComponentsInChildren<ExpensiveGrassStalk>())
        {
            lines.AddRange(child.GetLineVertices());
        }

        return lines;
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.green;
        var size = new Vector2(Width, Height);
        Gizmos.DrawWireCube((Vector2)transform.position + size / 2, size);

        if (CornerLeft)
            Gizmos.DrawLine(transform.position, (Vector2)transform.position + new Vector2(size.x / 2, size.y));
        if (CornerRight)
            Gizmos.DrawLine((Vector2)transform.position + new Vector2(size.x, 0), (Vector2)transform.position + new Vector2(size.x / 2, size.y));
        Gizmos.color = Color.white;
    }

    public void SaveChunkDecorator()
    {
        //throw new System.NotImplementedException();
    }

    public void LoadFromChunkDecorator()
    {
        var cd = GetComponent<ChunkDecorator>();
        if (cd == null)
            return;

        Seed = cd.GetProperty("seed", 0);
        CornerLeft = cd.GetProperty("cornerLeft", false);
        CornerRight = cd.GetProperty("cornerRight", false);
    }
}