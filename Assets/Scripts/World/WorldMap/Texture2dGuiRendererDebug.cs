using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Texture2dGuiRendererDebug : MonoBehaviour
{
    public Texture Texture;
    public Rect Rect;

    // Start is called before the first frame update
    private void Start()
    {
    }

    // Update is called once per frame
    private void Update()
    {
    }

    private void OnGUI()
    {
        GUI.DrawTexture(Rect, Texture);
    }
}