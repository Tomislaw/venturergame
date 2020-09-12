using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

[System.Serializable]
[CreateAssetMenu(fileName = "Animation", menuName = "Venturer/SimpleAnimator/Transform animation", order = 1)]
public class TransformAnimation : ScriptableObject
{
    [System.Serializable]
    public struct Frame
    {
        public Vector3 position;
        public Quaternion rotation;
        public Vector3 scale;

        public bool SetPosition;
        public bool SetRotation;
        public bool SetScale;

        public float pixelsPerUnit;

        public void Apply(GameObject gameObject)
        {
            if (SetPosition)
                gameObject.transform.localPosition = position / pixelsPerUnit;
            if (SetScale)
                gameObject.transform.localScale = scale;
            if (SetRotation)
                gameObject.transform.localRotation = rotation;
        }
    }

    public string name = "";
    public float time = 1;
    public bool loop = true;

    public List<Frame> frames = new List<Frame>();

#if UNITY_EDITOR
    public List<Sprite> sprites = new List<Sprite>();
#endif
}

#if UNITY_EDITOR

[CustomEditor(typeof(TransformAnimation)), CanEditMultipleObjects]
internal class TransformAnimationEditor : Editor
{
    private TransformAnimation animation;

    private bool customEditor = true;
    private bool defaultEditor = true;

    public void OnEnable()
    {
        animation = (serializedObject.targetObject as TransformAnimation);
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
                        animation.frames.Add(new TransformAnimation.Frame { position = sprite.pivot, pixelsPerUnit = sprite.pixelsPerUnit });
                    else
                        animation.frames.Add(new TransformAnimation.Frame());
                }
            }

            GUILayout.EndHorizontal();
        }
        EditorGUI.indentLevel--;
        EditorGUILayout.EndFoldoutHeaderGroup();

        serializedObject.ApplyModifiedProperties();

        TransformAnimation copy = null;
        copy = (TransformAnimation)EditorGUILayout.ObjectField(copy, typeof(TransformAnimation), true);
        if (copy != null)
        {
            animation.loop = copy.loop;
            animation.name = copy.name;
            animation.sprites = copy.sprites;
            animation.frames = copy.frames;
            animation.time = copy.time;
        }
    }
}

#endif