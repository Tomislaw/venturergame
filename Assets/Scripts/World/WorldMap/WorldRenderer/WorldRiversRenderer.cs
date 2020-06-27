using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using WorldStructures;

[ExecuteInEditMode]
public class WorldRiversRenderer : MonoBehaviour
{
    public WorldMap World;
    public int PixelsPerUnit = 32;
    public float WidthScale = 2;

    private int worldHash = 0;

    public LineRenderer prefab;

    // Start is called before the first frame update
    private void OnEnable()
    {
        Reload();
    }

    // Update is called once per frame
    private void Update()
    {
        if (World == null)
            return;

        if (worldHash != World.Version)
        {
            worldHash = World.Version;
            UnityEditor.EditorApplication.delayCall += () =>
            {
                Reload();
            };
        }
    }

    public void Reload()
    {
        if (World == null)
            return;

        if (prefab)
            GenerateLineRenderers();
    }

    private void GenerateLineRenderers()
    {
        ;
        foreach (var child in transform.GetComponentsInChildren<LineRenderer>())
        {
            if (Application.isPlaying)
                GameObject.Destroy(child.gameObject);
            else
                GameObject.DestroyImmediate(child.gameObject);
        }

        var riverLines = GetLines();

        foreach (var river in riverLines)
        {
            var riverRenderer = Instantiate(prefab);

            riverRenderer.loop = false;
            riverRenderer.positionCount = river.Count;
            riverRenderer.SetPositions(river.Select(it =>
            new Vector3(
                it.Item1.x / PixelsPerUnit,
                it.Item1.y / PixelsPerUnit)
            ).ToArray());

            var curve = new AnimationCurve();
            var maxSize = river.Max(it => it.Item2);
            for (int i = 0; i < river.Count; i++)
            {
                var time = (float)i / (float)(river.Count - 1);
                var value = river[i].Item2 / maxSize;
                curve.AddKey(new Keyframe(time, value));
            }
            riverRenderer.widthCurve = curve;
            riverRenderer.name = "river";
            riverRenderer.transform.SetParent(transform, false);
            riverRenderer.widthMultiplier = maxSize / PixelsPerUnit * WidthScale;
        }
    }

    private List<List<(Vector2, float)>> GetLines()
    {
        var sources = World.Rivers.FindAll(it => it.Top == -1);

        var riverPoints = new List<List<(Vector2, float)>>();

        foreach (var source in sources)
        {
            var list = new List<(Vector2, float)>();
            var river = source;

            if (river.Bottom != -1)
            {
                var bottomRiver = World.Rivers.Find(it => it.Id == river.Bottom);
                if (river.Edge.L == bottomRiver.Edge.L || river.Edge.L == bottomRiver.Edge.R)
                    list.Add((river.Edge.R, 0.01f));
                else
                    list.Add((river.Edge.L, 0.01f));
            }

            while (true)
            {
                if (river.Edge.L == list.Last().Item1)
                    list.Add((river.Edge.R, river.Size));
                else
                    list.Add((river.Edge.L, river.Size));

                if (river.Bottom == -1)
                    break;

                river = World.Rivers.Find(it => it.Id == river.Bottom);
            }

            riverPoints.Add(list);
        }

        return riverPoints;
    }
}