using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

public abstract class Item : ScriptableObject
{
    virtual public string ItemName { get; }
    virtual public Sprite UiSprite { get; }
    virtual public Sprite GameSprite { get; }
    virtual public Vector2Int Size { get; }
    virtual public int MaxCountPerStack { get; }

    override public string ToString()
    {
        var str = "{";
        str += "ItemName:" + ItemName;
        str += ", UiSprite:" + UiSprite;
        str += ", GameSprite:" + GameSprite;
        str += ", Size:" + Size;
        str += ", MaxCountPerStack:" + MaxCountPerStack;
        str += "}";
        return str;
    }
}