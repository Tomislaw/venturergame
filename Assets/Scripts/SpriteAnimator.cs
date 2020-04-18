using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using System.Linq;

[System.Serializable]
public class SpriteAnimation
{
    public string name;
    public List<Sprite> frames;
    public float animationTime;
    public bool looped;
}

[RequireComponent(typeof(SpriteRenderer))]
public class SpriteAnimator : MonoBehaviour
{
    public Sprite _debugReplaceSprites;

#if (UNITY_EDITOR)

    private void OnValidate()
    {
        if (_debugReplaceSprites != null)
        {
            var name = _debugReplaceSprites.name;
            name = name.Substring(0, name.LastIndexOf('_'));
            var path = AssetDatabase.GetAssetPath(_debugReplaceSprites);

            Sprite[] objs = AssetDatabase.LoadAllAssetsAtPath(path).OfType<Sprite>().ToArray();

            foreach (var a in animations)
            {
                for (int i = 0; i < a.frames.Count; i++)
                {
                    var findSpriteName = name + a.frames[i].name.Substring(a.frames[i].name.LastIndexOf('_'));
                    var sprite = objs.FirstOrDefault(it => it.name == findSpriteName);
                    if (sprite != null)
                        a.frames[i] = sprite;
                }
            }

            var mainSprite = objs.FirstOrDefault(it => it.name == name + "_0");
            if (mainSprite != null)
                GetComponent<SpriteRenderer>().sprite = mainSprite;

            _debugReplaceSprites = null;
        }
    }

#endif
    public List<SpriteAnimation> animations;
    private SpriteAnimation currentAnimation;

    private float timeForAnimation;
    private int animationFrame = 0;
    public SpriteRenderer spriteRenderer { get; private set; } = null;

    public bool IsAnimationFinished { get => animationFrame == currentAnimation.frames.Count - 1 || currentAnimation.looped; }
    public bool IsAnimationLooped { get => currentAnimation.looped; }

    public void Sync(SpriteAnimator other)
    {
        animationFrame = other.animationFrame;
        timeForAnimation = other.timeForAnimation;
    }

    public void SetAnimation(int id)
    {
        if (id >= animations.Count || id < 0)
            return;
        if (currentAnimation == animations[id])
            return;

        currentAnimation = animations[id];
        animationFrame = -1;
        timeForAnimation = currentAnimation.animationTime;
    }

    public void SetAnimation(string name)
    {
        if (currentAnimation != null && currentAnimation.name == name)
        {
            if (!currentAnimation.looped && !IsAnimationFinished)
                return;
            if (currentAnimation.looped)
                return;
        }

        currentAnimation = null;
        foreach (var a in animations)
        {
            if (a.name != name)
                continue;

            currentAnimation = a;
            animationFrame = -1;
            timeForAnimation = currentAnimation.animationTime;
            break;
        }
    }

    public SpriteAnimation GetAnimation()
    {
        return currentAnimation;
    }

    private void Start()
    {
        this.spriteRenderer = GetComponent<SpriteRenderer>();
        if (animations.Count > 0)
        {
            currentAnimation = animations[0];
            timeForAnimation = currentAnimation.animationTime;
        }
    }

    private void LateUpdate()
    {
        if (currentAnimation == null)
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
            else
            {
                if (animationFrame != currentAnimation.frames.Count - 1)
                {
                    animationFrame = currentAnimation.frames.Count - 1;
                    spriteRenderer.sprite = currentAnimation.frames[animationFrame];
                }

                return;
            }
        }

        int id = (int)Mathf.Lerp(currentAnimation.frames.Count - 1, 0, timeForAnimation / currentAnimation.animationTime);

        if (id != animationFrame)
        {
            if (spriteRenderer.sprite != currentAnimation.frames[id])
                spriteRenderer.sprite = currentAnimation.frames[id];
            animationFrame = id;
        }
        timeForAnimation -= Time.deltaTime;
    }
}