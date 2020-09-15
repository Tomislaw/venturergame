using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using System.Linq;

public class TransformAnimator : MonoBehaviour, Animator
{
    public TransformAnimationClip clip;

    private List<TransformAnimation> animationsInternal = new List<TransformAnimation>();

    [SerializeField]
    private string _currentAnimation = "idle";

    private TransformAnimation currentAnimation;

    public bool IsAnimationFinished { get => currentAnimation == null || AnimationFrame == currentAnimation.frames.Count - 1 || currentAnimation.loop; }
    public bool IsAnimationLooped { get => currentAnimation != null || currentAnimation.loop; }

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
            if (currentAnimation != null && currentAnimation.name == value)
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
            TimeForAnimation = currentAnimation.time;
            if (currentAnimation.frames.Count > 0)
                currentAnimation.frames[0].Apply(gameObject);
            break;
        }
    }

    private void OnEnable()
    {
        Animation = _currentAnimation;
        if (currentAnimation != null)
            TimeForAnimation = currentAnimation.time;
    }

    private void LateUpdate()
    {
        if (currentAnimation == null || currentAnimation.frames == null)
        {
            return;
        }

        if (currentAnimation.frames.Count == 0)
        {
            return;
        }

        if (TimeForAnimation < 0)
        {
            if (currentAnimation.loop)
            {
                TimeForAnimation = currentAnimation.time;
            }
        }

        int _id = 0;
        if (currentAnimation.time > 0)
            _id = (int)((1f - (TimeForAnimation / currentAnimation.time)) * currentAnimation.frames.Count);

        int id = Mathf.Min(currentAnimation.frames.Count - 1, _id);

        if (id != AnimationFrame)
        {
            currentAnimation.frames[id].Apply(gameObject);
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
            currentAnimation.frames[0].Apply(gameObject);
    }

    private void ReloadAnimations()
    {
        animationsInternal.Clear();
        animationsInternal.AddRange(clip.GetAnimations());
    }
}

#if UNITY_EDITOR

//[CustomEditor(typeof(TransformAnimator)), CanEditMultipleObjects]
//internal class TransformAnimatiorEditor : Editor
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