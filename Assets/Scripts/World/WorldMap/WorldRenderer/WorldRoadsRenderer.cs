using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEngine;

[ExecuteInEditMode]
public class WorldRoadsRenderer : MonoBehaviour
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
#if UNITY_EDITOR
        if (World == null)
            return;

        if (worldHash != World.Version)
        {
            worldHash = World.Version;

            UnityEditor.EditorApplication.delayCall += () =>
            {
                Reload();
            };
#endif
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
        foreach (var child in transform.GetComponentsInChildren<LineRenderer>())
        {
            if (Application.isPlaying)
                GameObject.Destroy(child.gameObject);
            else
                GameObject.DestroyImmediate(child.gameObject);
        }

        var roadLines = GetLines();

        foreach (var road in roadLines)
        {
            var roadRenderer = Instantiate(prefab);

            roadRenderer.loop = false;
            roadRenderer.positionCount = road.Count;
            roadRenderer.SetPositions(road.Select(it =>
            new Vector3(
                it.Item1.x / PixelsPerUnit,
                it.Item1.y / PixelsPerUnit)
            ).ToArray());

            var curve = new AnimationCurve();
            var maxSize = road.Max(it => it.Item2);
            for (int i = 0; i < road.Count; i++)
            {
                var time = (float)i / (float)(road.Count - 1);
                var value = road[i].Item2 / maxSize;
                curve.AddKey(new Keyframe(time, value));
            }
            roadRenderer.widthCurve = curve;
            roadRenderer.name = "road";
            roadRenderer.transform.SetParent(transform, false);
            roadRenderer.widthMultiplier = maxSize / PixelsPerUnit * WidthScale;
        }
    }

    private List<List<(Vector2, float)>> GetLines()
    {
        var sources = World.Rivers.FindAll(it => it.Top == -1);

        var roadPoints = new List<List<(Vector2, float)>>();

        foreach (var road in World.Roads)
        {
            var list = new List<(Vector2, float)>();

            var l = World.Regions.Find(it => it.Id == road.Left);
            var r = World.Regions.Find(it => it.Id == road.Right);
            list.Add((l.Center, 1));
            list.Add((r.Center, 1));
            roadPoints.Add(list);
        }

        return roadPoints;
    }
}