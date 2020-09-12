using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

[System.Serializable]
[CreateAssetMenu(fileName = "Animation", menuName = "Venturer/SimpleAnimator/Animation", order = 1)]
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
    public float pixelsPerUnit = 32;

    public List<Frame> frames = new List<Frame>();

    private Dictionary<string, SpriteAnimation> spriteAnimations = new Dictionary<string, SpriteAnimation>();

    public SpriteAnimation GetSpriteAnimation(Texture2D texture)
    {
        if (spriteAnimations.ContainsKey(texture.name))
            return spriteAnimations[texture.name];

        var anim = new SpriteAnimation(texture, this, pixelsPerUnit);
        spriteAnimations[texture.name] = anim;
        return anim;
    }

#if UNITY_EDITOR
    public List<Sprite> sprites = new List<Sprite>();
#endif

    [System.Serializable]
    public class SpriteAnimation
    {
        public string name;
        public List<Sprite> frames;
        public float animationTime;
        public bool looped;

        public SpriteAnimation(Texture2D texture, SimpleAnimation animation, float pixelsPerUnit)
        {
            if (animation == null)
                Debug.LogError("how it is possible"); ;
            this.name = animation.name;
            this.animationTime = animation.time;
            this.frames = new List<Sprite>();
            this.looped = animation.loop;
            int i = 0;
            foreach (var frame in animation.frames)
            {
                var rect = frame.rect;
                var pivot = new Vector2(frame.pivot.x / rect.width, frame.pivot.y / rect.height);
                if (!MathExtension.IsBetweenRange(pivot.x, 0, 1))
                    pivot.x = 0;
                if (!MathExtension.IsBetweenRange(pivot.y, 0, 1))
                    pivot.y = 0;
                try
                {
                    var sprite = Sprite.Create(texture, rect, pivot, pixelsPerUnit);
                    sprite.name = animation.name + "_" + i;
                    this.frames.Add(sprite);
                    i++;
                }
                catch (System.Exception e)
                {
                    Debug.LogException(e);
                }
            }
        }
    }
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