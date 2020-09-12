using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using System.Linq;

[RequireComponent(typeof(SpriteRenderer))]
public class SpriteAnimator : MonoBehaviour, Animator
{
    public Texture2D texture;

    public SimpleAnimationClip clip;

    public SpriteRenderer spriteRenderer { get => GetComponent<SpriteRenderer>(); }

    private List<SimpleAnimation.SpriteAnimation> animationsInternal = new List<SimpleAnimation.SpriteAnimation>();

    [SerializeField]
    private string _currentAnimation = "idle";

    private SimpleAnimation.SpriteAnimation currentAnimation;

    public bool IsAnimationFinished { get => currentAnimation == null || AnimationFrame == currentAnimation.frames.Count - 1 || currentAnimation.looped; }
    public bool IsAnimationLooped { get => currentAnimation != null || currentAnimation.looped; }

    public string Animation
    {
        get { return currentAnimation == null ? null : currentAnimation.name; }
        set
        {
            SetAnimation(value);
        }
    }

    public int AnimationFrame { get; set; } = 0;
    public float TimeForAnimation { get; set; }

    public void Sync(Animator other)
    {
        Animation = other.Animation;
        AnimationFrame = other.AnimationFrame;
        TimeForAnimation = other.TimeForAnimation;
    }

    public void SetAnimation(string value, bool restartIfSame = true)
    {
        if (!restartIfSame)
        {
            if (currentAnimation.name == value)
                return;
        }
        else if (currentAnimation != null && currentAnimation.name == value)
        {
            if (!IsAnimationLooped && !IsAnimationFinished)
                return;
            if (IsAnimationLooped)
                return;
        }

        currentAnimation = null;
        foreach (var a in animationsInternal)
        {
            if (a.name != value)
                continue;

            currentAnimation = a;
            _currentAnimation = a.name;
            AnimationFrame = -1;
            TimeForAnimation = currentAnimation.animationTime;
            if (currentAnimation.frames.Count > 0)
                spriteRenderer.sprite = currentAnimation.frames[0];
            break;
        }
    }

    private void OnEnable()
    {
        Animation = _currentAnimation;
        if (currentAnimation != null)
            TimeForAnimation = currentAnimation.animationTime;
    }

    private void LateUpdate()
    {
        if (currentAnimation == null || currentAnimation.frames == null)
        {
            spriteRenderer.sprite = null;
            return;
        }

        if (currentAnimation.frames.Count == 0)
        {
            spriteRenderer.sprite = null;
            return;
        }

        if (TimeForAnimation < 0)
        {
            if (currentAnimation.looped)
            {
                TimeForAnimation = currentAnimation.animationTime;
            }
        }

        int _id = 0;
        if (currentAnimation.animationTime > 0)
            _id = (int)((1f - (TimeForAnimation / currentAnimation.animationTime)) * currentAnimation.frames.Count);

        int id = Mathf.Min(currentAnimation.frames.Count - 1, _id);

        if (id != AnimationFrame)
        {
            if (spriteRenderer.sprite != currentAnimation.frames[id])
                spriteRenderer.sprite = currentAnimation.frames[id];
            AnimationFrame = id;
        }
        TimeForAnimation -= Time.deltaTime;
    }

#if (UNITY_EDITOR)

    private void OnValidate()
    {
        ReloadAnimations();
    }

#endif

    private void Awake()
    {
        ReloadAnimations();

        if (currentAnimation?.frames?.Count > 0)
            spriteRenderer.sprite = currentAnimation.frames[0];
    }

    private void ReloadAnimations()
    {
        if (texture == null)
        {
            Debug.LogError("Object " + name + " don't have selected texture.");
            return;
        }

        animationsInternal.Clear();
        animationsInternal.AddRange(clip.GetSpriteAnimations(texture));
    }
}

#if UNITY_EDITOR

[CustomEditor(typeof(SpriteAnimator)), CanEditMultipleObjects]
internal class SimpleAnimatiorEditor : Editor
{
    private SpriteAnimator animation;

    private string currentAnimation;
    private int currentAnimationId = -1;
    private string[] animations;

    private void OnEnable()
    {
        animation = (serializedObject.targetObject as SpriteAnimator);
        if (animation.Animation != null)
            currentAnimation = animation.Animation;
        else
            currentAnimation = "";
    }

    public override void OnInspectorGUI()
    {
        //DrawDefaultInspector();
        serializedObject.Update();

        EditorGUILayout.PropertyField(serializedObject.FindProperty("texture"), true);
        EditorGUILayout.PropertyField(serializedObject.FindProperty("pixelsPerUnit"), true);
        EditorGUILayout.PropertyField(serializedObject.FindProperty("clip"), true);

        if (animation.clip == null)
        {
            serializedObject.ApplyModifiedProperties();
            return;
        }

        animations = animation.clip.animations.Select(it => it == null ? "" : it.name).ToArray();

        int i = 0;

        foreach (var anim in animations)
        {
            if (anim == animation.Animation)
            {
                currentAnimationId = i;
            }
            i++;
        }

        if (currentAnimationId >= animations.Length)
        {
            currentAnimationId = 0;
        }

        if (currentAnimationId >= 0 && currentAnimationId < animations.Length)
        {
            currentAnimationId = EditorGUILayout.Popup("CurrentAnimation", currentAnimationId, animations);
            currentAnimation = animations[currentAnimationId];
            if (animation.Animation == null || currentAnimation != animation.Animation)
                animation.Animation = currentAnimation;
        }

        serializedObject.ApplyModifiedProperties();
    }
}

#endif