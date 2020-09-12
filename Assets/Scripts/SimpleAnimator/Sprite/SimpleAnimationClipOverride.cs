using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

[System.Serializable]
[CreateAssetMenu(fileName = "Animation clip override", menuName = "Venturer/SimpleAnimator/Animation clip override", order = 1)]
public class SimpleAnimationClipOverride : SimpleAnimationClip
{
    public SimpleAnimationClip parent;

    override public SimpleAnimation GetAnimation(string name)
    {
        var anim = base.GetAnimation(name);

        return anim != null ? anim : parent.GetAnimation(name);
    }

    override public List<SimpleAnimation.SpriteAnimation> GetSpriteAnimations(Texture2D texture)
    {
        var list = base.GetSpriteAnimations(texture);

        foreach (var anim in parent.animations)
        {
            if (list.Exists(it => it.name.Equals(anim.name)))
                continue;
            list.Add(anim.GetSpriteAnimation(texture));
        }

        return list;
    }

    override public List<SimpleAnimation> GetAnimations()
    {
        var list = base.GetAnimations();

        foreach (var anim in parent.GetAnimations())
        {
            if (list.Exists(it => it.name.Equals(anim.name)))
                continue;
            list.Add(anim);
        }

        return list;
    }
}