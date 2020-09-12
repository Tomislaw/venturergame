using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

[System.Serializable]
[CreateAssetMenu(fileName = "Animation clip", menuName = "Venturer/SimpleAnimator/Transform animation clip", order = 1)]
public class TransformAnimationClip : ScriptableObject
{
    public List<TransformAnimation> animations = new List<TransformAnimation>();

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

    virtual public TransformAnimation GetAnimation(string name)
    {
        int id = -1;
        if (map.TryGetValue(name, out id))
        {
            return animations[id];
        }
        return null;
    }

    virtual public List<TransformAnimation> GetAnimations()
    {
        return animations;
    }
}