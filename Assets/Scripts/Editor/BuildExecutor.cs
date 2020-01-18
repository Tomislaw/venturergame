using UnityEditor;
using UnityEngine;

public class BuildExecutor
{
    public static string[] scenes = { "Assets/Scenes/SampleScene.unity" };
    public static string name = "Venturer";

    [MenuItem("Build/Build WebGL")]
    public static void BuildWebGL()
    {
        BuildPipeline.BuildPlayer(scenes, "./Build/" + name + "_Web/" + name, BuildTarget.WebGL, BuildOptions.None);
    }

    [MenuItem("Build/Build Windows")]
    public static void BuildWindows()
    {
        BuildPipeline.BuildPlayer(scenes, "./Build/" + name + "_Windows/" + name + ".exe", BuildTarget.StandaloneWindows64, BuildOptions.None);
    }

    [MenuItem("Build/Build Linux")]
    public static void BuildLinux()
    {
        BuildPipeline.BuildPlayer(scenes, "./Build/" + name + "_Linux/" + name, BuildTarget.StandaloneLinux64, BuildOptions.None);
    }

    [MenuItem("Build/Build All")]
    public static void BuildAll()
    {
        BuildLinux();
        BuildWindows();
        BuildWebGL();
    }
}