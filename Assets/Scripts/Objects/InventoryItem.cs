using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using UnityEngine.Rendering;

[RequireComponent(typeof(SpriteRenderer))]
public class InventoryItem : MonoBehaviour
{
    [System.Serializable]
    public class SpritePair
    {
        public int Key;
        public Sprite Value;
    }

    public List<SpritePair> inventorySprites = new List<SpritePair>();
    public List<SpritePair> gameSprites = new List<SpritePair>();

    // Start is called before the first frame update

    [SerializeField]
    private bool _isInGameWorld;

    public bool IsInGameWorld
    {
        get => _isInGameWorld;
        set
        {
            _isInGameWorld = value;
            InvalidateSprites();
        }
    }

    [SerializeField]
    private int _count;

    public int Count
    {
        get => _count;
        set
        {
            _count = value;
            InvalidateSprites();
        }
    }

    private void Start()
    {
        Count = _count;
    }

    private void InvalidateSprites()
    {
        var sprite = GetSprite();
        if (sprite != GetComponent<SpriteRenderer>().sprite)
            GetComponent<SpriteRenderer>().sprite = sprite;
    }

    // Update is called once per frame
    private void Update()
    {
        InvalidateSprites();
        if (IsInGameWorld)
        {
            if (transform.position.y > -2)
                transform.position -= new Vector3(0, Time.deltaTime * 3);
            if (transform.position.y < -2)
                transform.position = new Vector3(transform.position.x, -2, transform.position.z);
        }
    }

    private void OnValidate()
    {
        InvalidateSprites();
    }

    private Sprite GetSprite()
    {
        var list = _isInGameWorld ? gameSprites : inventorySprites;

        if (list.Count == 1)
            return list[0].Value;
        else if (list.Count > 1)
        {
            foreach (var item in list)
                if (item.Key <= _count)
                    return item.Value;
            return null;
        }
        return null;
    }
}