using UnityEngine;
using UnityEditor;
using System.Collections.Generic;

public class SpriteCopy : EditorWindow
{
    private Object copyFrom;

    private Object copyTo;

    [SerializeField]
    private List<Texture2D> list;

    private SerializedObject serializedObject;

    private bool multiple = false;

    // Creates a new option in "Windows"
    [MenuItem("Venturer/Copy Spritesheet pivots and slices")]
    private static void Init()
    {
        // Get existing open window or if none, make a new one:

        SpriteCopy window = (SpriteCopy)EditorWindow.GetWindow(typeof(SpriteCopy));
        window.serializedObject = new SerializedObject(window);
        window.Show();
    }

    private void OnGUI()
    {
        GUILayout.BeginHorizontal();
        GUILayout.Label("Copy from:", EditorStyles.boldLabel);
        copyFrom = EditorGUILayout.ObjectField(copyFrom, typeof(Texture2D), false, GUILayout.Width(220));
        GUILayout.EndHorizontal();

        GUILayout.BeginHorizontal();
        GUILayout.Label("Multiple:", EditorStyles.boldLabel);
        multiple = EditorGUILayout.Toggle(multiple, GUILayout.Width(220));
        GUILayout.EndHorizontal();

        if (!multiple)
        {
            GUILayout.BeginHorizontal();
            GUILayout.Label("Target:", EditorStyles.boldLabel);
            copyTo = EditorGUILayout.ObjectField(copyTo, typeof(Texture2D), false, GUILayout.Width(220));
            GUILayout.EndHorizontal();
        }
        else
        {
            GUILayout.BeginHorizontal();
            GUILayout.Label("Targets:", EditorStyles.boldLabel);
            EditorGUILayout.PropertyField(serializedObject.FindProperty("list"), GUILayout.Width(220));
            GUILayout.EndHorizontal();
            serializedObject.ApplyModifiedProperties();
        }

        GUILayout.Space(25f);
        if (GUILayout.Button("Copy pivots and slices"))
        {
            CopyPivotsAndSlices();
        }
    }

    private void CopyPivotsAndSlices()
    {
        if (!copyFrom || copyTo && !multiple)
        {
            Debug.Log("Missing one object");
            return;
        }

        if (copyFrom.GetType() != typeof(Texture2D) || ((copyTo.GetType() != typeof(Texture2D)) && !multiple))
        {
            Debug.Log("Cant convert from: " + copyFrom.GetType() + "to: " + copyTo.GetType() + ". Needs two Texture2D objects!");
            return;
        }

        string copyFromPath = AssetDatabase.GetAssetPath(copyFrom);
        TextureImporter ti1 = AssetImporter.GetAtPath(copyFromPath) as TextureImporter;
        ti1.isReadable = true;

        if (multiple)
        {
            foreach (var item in list)
            {
                Apply(ti1, item);
            }
        }
        else
        {
            Apply(ti1, copyTo);
        }
    }

    private void Apply(TextureImporter ti1, Object asset)
    {
        string copyToPath = AssetDatabase.GetAssetPath(asset);
        TextureImporter ti2 = AssetImporter.GetAtPath(copyToPath) as TextureImporter;
        ti2.isReadable = true;

        ti2.spriteImportMode = SpriteImportMode.Multiple;

        List<SpriteMetaData> newData = new List<SpriteMetaData>();

        Debug.Log("Amount of slices found: " + ti1.spritesheet.Length);

        for (int i = 0; i < ti1.spritesheet.Length; i++)
        {
            SpriteMetaData d = ti1.spritesheet[i];
            newData.Add(d);
        }
        ti2.spritesheet = newData.ToArray();

        AssetDatabase.ImportAsset(copyToPath, ImportAssetOptions.ForceUpdate);
    }
}