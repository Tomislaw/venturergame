// https://forum.unity.com/threads/compute-shader-for-line-drawing.599989/
using UnityEngine;

[ExecuteAlways]
public class Grass : MonoBehaviour
{
    public ComputeShader CS;

    public Vector2Int PixelSize;
    private RenderTexture RT;

    private void Start()
    {
        RT = new RenderTexture(PixelSize.x, PixelSize.y, 0);
        RT.enableRandomWrite = true;
        RT.format = RenderTextureFormat.ARGB32;
        RT.filterMode = FilterMode.Point;
        RT.Create();
        CS.SetTexture(0, "surface", RT);
        CS.SetInts("stalks", 16, 32, 16, 100, 48, 32, 10, 200);
        CS.SetVector("colorA", new Vector4(0, 1, 0, 1));
        CS.Dispatch(0, RT.width / 8, RT.height / 8, 1);
        GetComponent<Renderer>().material.mainTexture = RT;
    }

    private int ahh = 4;

    private void Update()
    {
        CS.Dispatch(0, RT.width / 8, RT.height / 8, 1);

        ahh--;
    }
}