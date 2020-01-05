using System;
using System.Collections.Generic;
using UnityEngine;

public class Chunk : MonoBehaviour
{
    [Serializable]
    public struct Decorator
    {
        public Vector3 Offset;
        public Sprite Sprite;
    }

    public Vector2 Size = new Vector2(4, 1);
    public List<Decorator> Decorators = new List<Decorator>();
    // Start is called before the first frame update

    private void Start()
    {
        //AddDecorators();
    }

    private void Update()
    {
    }

#if UNITY_EDITOR

    private void OnValidate()
    {
        UnityEditor.EditorApplication.delayCall += () =>
        {
            if (this == null)
                return;
            for (int i = this.transform.childCount; i > 0; --i)
                DestroyImmediate(this.transform.GetChild(0).gameObject);
            AddDecorators();
        };
    }

#endif

    private void OnDestroy()
    {
        for (int i = this.transform.childCount; i > 0; --i)
            Destroy(this.transform.GetChild(0).gameObject);
    }

    private void AddDecorators()
    {
        int counter = 0;
        foreach (var decorator in Decorators)
        {
            var obj = new GameObject();
            obj.name = gameObject.name + "_decorator" + counter;
            var renderer = obj.AddComponent<SpriteRenderer>();
            renderer.sprite = decorator.Sprite;
            obj.transform.position = gameObject.transform.position + decorator.Offset;
            obj.transform.SetParent(gameObject.transform, true);
            counter++;
            //Instantiate(obj);
        }
    }
}