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

    public SpriteRenderer spriteRenderer { get; private set; }

    private List<SimpleAnimation.SpriteAnimation> animationsInternal = new List<SimpleAnimation.SpriteAnimation>();

    [SerializeField]
    private string _currentAnimation = "idle";

    private SimpleAnimation.SpriteAnimation currentAnimation;

    public bool IsAnimationFinished { get => currentAnimation == null || AnimationFrame == currentAnimation.frames.Count - 1 || currentAnimation.looped; }
    public bool IsAnimationLooped { get => currentAnimation != null || currentAnimation.looped; }

    private int sortingOrder = 0;

    public string Animation
    {
        get { return currentAnimation == null ? null : currentAnimation.name; }
        set
        {
            SetAnimation(value);
        }
    }

    private int _animationFrame = 0;

    public int AnimationFrame
    {
        get => _animationFrame;
        set
        {
            if (currentAnimation != null && currentAnimation.frames.Count > 0 && currentAnimation.frames.Count > value)
            {
                var frame = currentAnimation.frames[value];

                if (frame.sprite == spriteRenderer.sprite)
                    return;

                spriteRenderer.sprite = frame.sprite;
                spriteRenderer.sortingOrder = sortingOrder + frame.order;
            }
            else
            {
                spriteRenderer.sprite = null;
                spriteRenderer.sortingOrder = sortingOrder;
            }
        }
    }

    public float TimeForAnimation { get; set; }

    public void Sync(Animator other)
    {
        Animation = other.Animation;
        AnimationFrame = other.AnimationFrame;
        TimeForAnimation = other.TimeForAnimation;
    }

    public void SetAnimation(string value, bool restartIfSame = true)
    {
        if (!restartIfSame && currentAnimation != null)
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
            AnimationFrame = 0;
            TimeForAnimation = currentAnimation.animationTime;
            break;
        }
    }

    private void Awake()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();
        sortingOrder = spriteRenderer.sortingOrder;

        ReloadAnimations();

        AnimationFrame = 0;
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

        if (TimeForAnimation < 0 && currentAnimation.looped)
        {
            TimeForAnimation = currentAnimation.animationTime;
        }

        int _id = 0;
        if (currentAnimation.animationTime > 0)
            _id = (int)((1f - (TimeForAnimation / currentAnimation.animationTime)) * currentAnimation.frames.Count);

        AnimationFrame = Mathf.Min(currentAnimation.frames.Count - 1, _id);

        TimeForAnimation -= Time.deltaTime;
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

//[CustomEditor(typeof(SpriteAnimator)), CanEditMultipleObjects]
//internal class SimpleAnimatiorEditor : Editor
//{
//    private SpriteAnimator animation;

//    private string currentAnimation;
//    private int currentAnimationId = -1;
//    private string[] animations;

//    private void OnEnable()
//    {
//        animation = (serializedObject.targetObject as SpriteAnimator);
//        if (animation.Animation != null)
//            currentAnimation = animation.Animation;
//        else
//            currentAnimation = "";
//    }

//    public override void OnInspectorGUI()
//    {
//        //DrawDefaultInspector();
//        serializedObject.Update();

//        EditorGUILayout.PropertyField(serializedObject.FindProperty("texture"), true);
//        EditorGUILayout.PropertyField(serializedObject.FindProperty("pixelsPerUnit"), true);
//        EditorGUILayout.PropertyField(serializedObject.FindProperty("clip"), true);

//        if (animation.clip == null)
//        {
//            serializedObject.ApplyModifiedProperties();
//            return;
//        }

//        animations = animation.clip.animations.Select(it => it == null ? "" : it.name).ToArray();

//        int i = 0;

//        foreach (var anim in animations)
//        {
//            if (anim == animation.Animation)
//            {
//                currentAnimationId = i;
//            }
//            i++;
//        }

//        if (currentAnimationId >= animations.Length)
//        {
//            currentAnimationId = 0;
//        }

//        if (currentAnimationId >= 0 && currentAnimationId < animations.Length)
//        {
//            currentAnimationId = EditorGUILayout.Popup("CurrentAnimation", currentAnimationId, animations);
//            currentAnimation = animations[currentAnimationId];
//            if (animation.Animation == null || currentAnimation != animation.Animation)
//                animation.Animation = currentAnimation;
//        }

//        serializedObject.ApplyModifiedProperties();
//    }
//}

#endif