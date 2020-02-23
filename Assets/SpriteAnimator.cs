using System.Collections;
using System.Collections.Generic;
using UnityEngine;

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
    public List<SpriteAnimation> animations;

    private float timeForAnimation;
    private SpriteAnimation currentAnimation;
    private int animationFrame = 0;
    private SpriteRenderer sr;

    public bool IsAnimationFinished { get => animationFrame == currentAnimation.frames.Count - 1 || currentAnimation.looped; }
    public bool IsAnimationLooped { get => currentAnimation.looped; }

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
        if (currentAnimation.name == name)
        {
            if (!currentAnimation.looped && !IsAnimationFinished)
                return;
            if (currentAnimation.looped)
                return;
        }

        Debug.Log(name);

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
        this.sr = GetComponent<SpriteRenderer>();
        if (animations.Count > 0)
        {
            currentAnimation = animations[0];
            timeForAnimation = currentAnimation.animationTime;
        }
    }

    private void LateUpdate()
    {
        if (currentAnimation != null)
        {
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
                        sr.sprite = currentAnimation.frames[animationFrame];
                    }

                    return;
                }
            }

            int id = (int)Mathf.Lerp(currentAnimation.frames.Count - 1, 0, timeForAnimation / currentAnimation.animationTime);

            if (id != animationFrame)
            {
                if (sr.sprite != currentAnimation.frames[id])
                    sr.sprite = currentAnimation.frames[id];
                animationFrame = id;
            }
            timeForAnimation -= Time.deltaTime;
        }
    }
}