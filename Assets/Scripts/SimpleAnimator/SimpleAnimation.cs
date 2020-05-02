using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

[System.Serializable]
[CreateAssetMenu(fileName = "EquipmentItem", menuName = "ScriptableObjects/SimpleAnimator/Animation", order = 1)]
public class SimpleAnimation : ScriptableObject
{
    [System.Serializable]
    public struct Frame
    {
        public Vector2 pivot;
        public Rect rect;
    }

    public string name = "";
    public float time = 1;
    public bool loop = true;

    public List<Frame> frames;

#if UNITY_EDITOR
    public List<Sprite> sprites = new List<Sprite>();
#endif
}

#if UNITY_EDITOR

[CustomEditor(typeof(SimpleAnimation)), CanEditMultipleObjects]
internal class SimpleAnimationEditor : Editor
{
    private SimpleAnimation animation;

    private bool customEditor = true;
    private bool defaultEditor = true;

    public void OnEnable()
    {
        animation = (serializedObject.targetObject as SimpleAnimation);
    }

    public override void OnInspectorGUI()
    {
        //DrawDefaultInspector();

        serializedObject.Update();

        if (animation == null)
            return;

        defaultEditor = EditorGUILayout.BeginFoldoutHeaderGroup(defaultEditor, "Basic");
        EditorGUI.indentLevel++;
        if (defaultEditor)
        {
            EditorGUILayout.PropertyField(serializedObject.FindProperty("name"), true);
            EditorGUILayout.PropertyField(serializedObject.FindProperty("time"), true);
            EditorGUILayout.PropertyField(serializedObject.FindProperty("loop"), true);
            EditorGUILayout.PropertyField(serializedObject.FindProperty("frames"), true);
        }
        EditorGUI.indentLevel--;
        EditorGUILayout.EndFoldoutHeaderGroup();

        customEditor = EditorGUILayout.BeginFoldoutHeaderGroup(customEditor, "Set frames using sprites");
        EditorGUI.indentLevel++;
        if (customEditor)
        {
            EditorGUILayout.PropertyField(serializedObject.FindProperty("sprites"), true);
            GUILayout.BeginHorizontal();
            if (GUILayout.Button("Clear"))
            {
                animation.frames.Clear();
            }
            if (GUILayout.Button("Add"))
            {
                foreach (var sprite in animation.sprites)
                {
                    if (sprite)
                        animation.frames.Add(new SimpleAnimation.Frame { rect = sprite.rect, pivot = sprite.pivot });
                    else
                        animation.frames.Add(new SimpleAnimation.Frame());
                }
            }

            GUILayout.EndHorizontal();
        }
        EditorGUI.indentLevel--;
        EditorGUILayout.EndFoldoutHeaderGroup();

        serializedObject.ApplyModifiedProperties();
    }
}

#endif