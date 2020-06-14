using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEngine;

[CreateAssetMenu(fileName = "ChunkDecorators", menuName = "ScriptableObjects/GameWorld/ChunkDecorators", order = 1)]
[ExecuteInEditMode]
public class ChunkDecoratorDictionary : ScriptableObject
{
    [SerializeField]
    public Dictionary<string, GameObject> entries = new Dictionary<string, GameObject>();

    private void OnValidate()
    {
        var list = Resources.LoadAll<GameObject>("Prefabs/ChunkDecorators");
        foreach (var item in list)
        {
            entries.Add(item.name, item);
        }
    }
}

#if UNITY_EDITOR

[CustomEditor(typeof(ChunkDecoratorDictionary))]
public class ChunkDecoratorEditorDisplay : Editor
{
    private Dictionary<string, GameObject> entries;
    private Vector2 scrollPos;

    private void OnEnable()
    {
        entries = (serializedObject.targetObject as ChunkDecoratorDictionary).entries;
    }

    public override void OnInspectorGUI()
    {
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