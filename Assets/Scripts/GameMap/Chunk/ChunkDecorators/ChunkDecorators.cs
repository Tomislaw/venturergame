using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEngine;
using Venturer;

[CreateAssetMenu(fileName = "ChunkDecorators", menuName = "ScriptableObjects/GameWorld/ChunkDecorators", order = 1)]
[ExecuteAlways]
public class ChunkDecorators : Venturer.ScriptableSingleton<ChunkDecorators>
{
    public Dictionary<string, GameObject> decoratos { get; private set; } = new Dictionary<string, GameObject>();

    public string Directory = "Prefabs/ChunkDecorators";

    private void OnEnable()
    {
        var list = Resources.LoadAll<GameObject>(Directory);
        foreach (var item in list)
        {
            decoratos.Add(item.name, item);
        }
    }
}

#if UNITY_EDITOR

[CustomEditor(typeof(ChunkDecorators))]
public class ChunkDecoratorEditorDisplay : Editor
{
    private Dictionary<string, GameObject> entries;
    private Vector2 scrollPos;

    private void OnEnable()
    {
        entries = (serializedObject.targetObject as ChunkDecorators).decoratos;
    }

    public override void OnInspectorGUI()
    {
        DrawDefaultInspector();

        EditorGUILayout.LabelField("Entries - " + entries.Count());

        int size = entries.Count() * 16;
        if (size > 400)
            size = 400;
        scrollPos = EditorGUILayout.BeginScrollView(scrollPos, GUILayout.Height(size));

        EditorGUI.indentLevel++;

        foreach (var entry in entries)
            EditorGUILayout.LabelField(entry.Key);
        EditorGUI.indentLevel--;

        EditorGUILayout.EndScrollView();
    }
}

#endif