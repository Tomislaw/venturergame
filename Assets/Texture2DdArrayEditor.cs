using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(Texture2DArray))]
public class Texture2DdArrayEditor : Editor
{
    private static int MAX_TEXTURE_SIZE = 16384;

    [MenuItem("Assets/Create/Texture 2D Array", false, 310)]
    private static void Create()
    {
        string path = AssetDatabase.GetAssetPath(Selection.activeObject);
        if (path == "")
            path = "Assets";
        else if (Path.GetExtension(path) != "")
            path = path.Replace(Path.GetFileName(AssetDatabase.GetAssetPath(Selection.activeObject)), "");

        var asset = new Texture2DArray(1, 1, 1, TextureFormat.RGBA32, false);
        asset.SetPixels(new Color[] { Color.white }, 0);
        AssetDatabase.CreateAsset(asset, AssetDatabase.GenerateUniqueAssetPath(path + "/Texture2DArray.asset"));
    }

    private string[] sizeTypes = new string[] { "Match first image", "Custom" };
    private int selectedSizeType = 0;

    private Texture2DArrayData data;
    private SerializedObject serializedData;

    private void OnEnable()
    {
        var texture = (Texture2DArray)serializedObject.targetObject;
        data = new Texture2DArrayData(texture);
        serializedData = new SerializedObject(data);
    }

    public override void OnInspectorGUI()
    {
        //DrawDefaultInspector();

        if (serializedData == null)
        {
            var color = GUI.contentColor;
            GUI.contentColor = Color.red;
            EditorGUILayout.LabelField("Fatal error!");
            GUI.contentColor = color;
            return;
        }

        var hash = serializedData.GetHashCode();
        serializedObject.Update();

        selectedSizeType = EditorGUILayout.Popup("Size policy", selectedSizeType, sizeTypes);

        if (selectedSizeType == 1)
        {
            data.size = EditorGUILayout.Vector2IntField("Size", data.size);
        }
        else
        {
            if (data.textures?.Count > 0 && data.textures[0])
                data.size = new Vector2Int(data.textures[0].width, data.textures[0].height);
            else
                data.size = new Vector2Int(1, 1);

            if (data.textures?.Count == 0 || data.textures == null || data.textures[0] == null)
            {
                var color = GUI.contentColor;
                GUI.contentColor = Color.red;
                EditorGUILayout.LabelField("Invalid first texture!");
                GUI.contentColor = color;
            }
            GUI.enabled = false;
            EditorGUILayout.Vector2IntField("Size", data.size);
            GUI.enabled = true;
        }

        EditorGUILayout.PropertyField(serializedData.FindProperty("mipChain"));
        EditorGUILayout.PropertyField(serializedData.FindProperty("minMapBias"));

        EditorGUILayout.PropertyField(serializedData.FindProperty("wrapMode"));
        EditorGUILayout.PropertyField(serializedData.FindProperty("filterMode"));
        EditorGUILayout.PropertyField(serializedData.FindProperty("anisoLevel"));

        EditorGUILayout.PropertyField(serializedData.FindProperty("textures"));

        serializedData.ApplyModifiedProperties();
        var edited = hash != serializedData.GetHashCode();

        if (edited)
        {
            var texture = (Texture2DArray)serializedObject.targetObject;
            if (data.CanApplyDataToTarget(texture))
                data.ApplyDataToTarget(texture);
        }
    }

    [Serializable]
    private class Texture2DArrayData : ScriptableObject
    {
        public Vector2Int size;
        public TextureWrapMode wrapMode;
        public FilterMode filterMode;
        public TextureFormat format;
        public float minMapBias = 0;
        public int anisoLevel = -1;
        public int depth = 1;
        public bool mipChain = false;

        public List<Texture2D> textures = new List<Texture2D>();

        public Texture2DArrayData(Texture2DArray texture)
        {
            Load(texture);
        }

        public void Load(Texture2DArray data)
        {
            anisoLevel = data.anisoLevel;
            filterMode = data.filterMode;
            wrapMode = data.wrapMode;
            size.y = data.height;
            size.x = data.width;

            if (size.x < 1)
                size.x = 1;
            if (size.x > MAX_TEXTURE_SIZE)
                size.x = MAX_TEXTURE_SIZE;

            if (size.y < 1)
                size.y = 1;
            if (size.y > MAX_TEXTURE_SIZE)
                size.y = MAX_TEXTURE_SIZE;

            format = data.format;
            minMapBias = data.mipMapBias;
            depth = data.depth;

            textures = new List<Texture2D>();

            for (int i = 0; i < depth; i++)
            {
                Texture2D _texture = new Texture2D(size.x, size.y);
                _texture.SetPixels32(data.GetPixels32(i));
                textures.Add(_texture);
            }
        }

        public Texture2DArray Create()
        {
            Texture2DArray texture = new Texture2DArray(size.x, size.y, depth, format, mipChain);
            texture.mipMapBias = minMapBias;
            texture.filterMode = filterMode;
            texture.wrapMode = wrapMode;
            texture.anisoLevel = anisoLevel;
            for (int i = 0; i < depth; i++)
            {
                Texture2D _texture = new Texture2D(textures[i].width, textures[i].height);
                Graphics.CopyTexture(textures[i], _texture);
                _texture.width = size.x;
                _texture.height = size.y;
                texture.SetPixels32(_texture.GetPixels32(), i);
            }
            return texture;
        }

        public bool CanApplyDataToTarget(Texture2DArray target)
        {
            return target.format == format && target.depth == depth && (mipChain ? target.mipmapCount > 1 : target.mipmapCount == 1);
        }

        public void ApplyDataToTarget(Texture2DArray target)
        {
            Texture2DArray texture = new Texture2DArray(size.x, size.y, depth, format, mipChain);
            target.mipMapBias = minMapBias;
            target.filterMode = filterMode;
            target.wrapMode = wrapMode;
            target.anisoLevel = anisoLevel;
            for (int i = 0; i < depth; i++)
            {
                Texture2D _texture = new Texture2D(textures[i].width, textures[i].height);
                Graphics.CopyTexture(textures[i], _texture);
                _texture.width = size.x;
                _texture.height = size.y;
                target.SetPixels32(_texture.GetPixels32(), i);
            }
        }
    }
}