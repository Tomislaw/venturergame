using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

[System.Serializable]
[CreateAssetMenu(fileName = "Animation clip", menuName = "Venturer/SimpleAnimator/Animation clip", order = 1)]
public class SimpleAnimationClip : ScriptableObject
{
    public List<SimpleAnimation> animations = new List<SimpleAnimation>();

    public Dictionary<string, int> map = new Dictionary<string, int>();

    private void Invalidate()
    {
        map.Clear();
        for (int id = 0; id < animations.Count; id++)
        {
            map[animations[0].name] = id;
        }
    }

    private void Awake()
    {
        Invalidate();
    }

    private void OnValidate()
    {
        Invalidate();
    }

    virtual public SimpleAnimation GetAnimation(string name)
    {
        int id = -1;
        if (map.TryGetValue(name, out id))
        {
            return animations[id];
        }
        return null;
    }

    virtual public List<SimpleAnimation> GetAnimations()
    {
        return animations;
    }

    virtual public List<SimpleAnimation.SpriteAnimation> GetSpriteAnimations(Texture2D texture)
    {
        var list = new List<SimpleAnimation.SpriteAnimation>();
        foreach (var anim in animations)
            list.Add(anim.GetSpriteAnimation(texture));
        return list;
    }
}