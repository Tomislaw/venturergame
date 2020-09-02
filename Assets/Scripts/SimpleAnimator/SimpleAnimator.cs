using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using System.Linq;

[RequireComponent(typeof(SpriteRenderer))]
public class SimpleAnimator : MonoBehaviour
{
    public Texture2D texture;
    public int pixelsPerUnit = 32;
    public List<SimpleAnimation> animations = new List<SimpleAnimation>();

    private float timeForAnimation;
    private int animationFrame = 0;
    public SpriteRenderer spriteRenderer { get => GetComponent<SpriteRenderer>(); }

    private List<SpriteAnimation> animationsInternal = new List<SpriteAnimation>();

    [SerializeField]
    private string _currentAnimation = "idle";

    private SpriteAnimation currentAnimation;

    public bool IsAnimationFinished { get => currentAnimation == null || animationFrame == currentAnimation.frames.Count - 1 || currentAnimation.looped; }
    public bool IsAnimationLooped { get => currentAnimation != null || currentAnimation.looped; }

    public string Animation
    {
        get { return currentAnimation == null ? null : currentAnimation.name; }
        set
        {
            SetAnimation(value);
        }
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
            if (!currentAnimation.looped && !IsAnimationFinished)
                return;
            if (currentAnimation.looped)
                return;
        }

        currentAnimation = null;
        foreach (var a in animationsInternal)
        {
            if (a.name != value)
                continue;

            currentAnimation = a;
            _currentAnimation = a.name;
            animationFrame = -1;
            timeForAnimation = currentAnimation.animationTime;
            if (currentAnimation.frames.Count > 0)
                spriteRenderer.sprite = currentAnimation.frames[0];
            break;
        }
    }

    private void OnEnable()
    {
        Animation = _currentAnimation;
        if (currentAnimation != null)
            timeForAnimation = currentAnimation.animationTime;
    }

    public void Sync(SimpleAnimator other)
    {
        Animation = other.Animation;
        animationFrame = other.animationFrame;
        timeForAnimation = other.timeForAnimation;
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

        if (timeForAnimation < 0)
        {
            if (currentAnimation.looped)
            {
                timeForAnimation = currentAnimation.animationTime;
            }
        }

        int _id = 0;
        if (currentAnimation.animationTime > 0)
            _id = (int)((1f - (timeForAnimation / currentAnimation.animationTime)) * currentAnimation.frames.Count);

        int id = Mathf.Min(currentAnimation.frames.Count - 1, _id);

        if (id != animationFrame)
        {
            if (spriteRenderer.sprite != currentAnimation.frames[id])
                spriteRenderer.sprite = currentAnimation.frames[id];
            animationFrame = id;
        }
        timeForAnimation -= Time.deltaTime;
    }

    [System.Serializable]
    public class SpriteAnimation
    {
        public string name;
        public List<Sprite> frames;
        public float animationTime;
        public bool looped;
    }

    private SpriteAnimation FromSimpleAnimation(SimpleAnimation animation)
    {
        if (animation == null)
            Debug.LogError("how it is possible");
        var _animation = new SpriteAnimation();
        _animation.name = animation.name;
        _animation.animationTime = animation.time;
        _animation.frames = new List<Sprite>();
        _animation.looped = animation.loop;
        int i = 0;
        foreach (var frame in animation.frames)
        {
            var rect = frame.rect;
            var pivot = new Vector2(frame.pivot.x / rect.width, frame.pivot.y / rect.height);
            if (!MathExtension.IsBetweenRange(pivot.x, 0, 1))
                pivot.x = 0;
            if (!MathExtension.IsBetweenRange(pivot.y, 0, 1))
                pivot.y = 0;

            var sprite = Sprite.Create(texture, rect, pivot, pixelsPerUnit);
            sprite.name = animation.name + "_" + i;
            _animation.frames.Add(sprite);
            i++;
        }
        return _animation;
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

        foreach (var animation in animations)
        {
            if (animation != null)
                animationsInternal.Add(FromSimpleAnimation(animation));
        }
    }
}

#if UNITY_EDITOR

[CustomEditor(typeof(SimpleAnimator)), CanEditMultipleObjects]
internal class SimpleAnimatiorEditor : Editor
{
    private SimpleAnimator animation;

    private string currentAnimation;
    private int currentAnimationId = -1;
    private string[] animations;

    private void OnEnable()
    {
        animation = (serializedObject.targetObject as SimpleAnimator);
        if (animation.Animation != null)
            currentAnimation = animation.Animation;
        else
            currentAnimation = "";

        animation.animations.Select(it => it.name).ToArray();
    }

    public override void OnInspectorGUI()
    {
        //DrawDefaultInspector();
        serializedObject.Update();

        EditorGUILayout.PropertyField(serializedObject.FindProperty("texture"), true);
        EditorGUILayout.PropertyField(serializedObject.FindProperty("pixelsPerUnit"), true);
        EditorGUILayout.PropertyField(serializedObject.FindProperty("animations"), true);

        animations = animation.animations.Select(it => it == null ? "" : it.name).ToArray();

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