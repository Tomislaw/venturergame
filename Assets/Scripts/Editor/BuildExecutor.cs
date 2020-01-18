using UnityEditor;
using UnityEngine;
using System;

namespace Pipeline
{
    public class BuildExecutor
    {
        public static string[] scenes = { "Assets/Scenes/SampleScene.unity" };
        public static string name = "Venturer";

        private static void Build(string buildName, BuildTarget target)
        {
            Console.WriteLine("Current directory: " + Environment.CurrentDirectory.ToString());
            Console.WriteLine("Building " + buildName);
            Console.WriteLine("Target: " + target.ToString());
            var report = BuildPipeline.BuildPlayer(scenes, buildName, target, BuildOptions.None);
            Console.WriteLine(report.summary.result.ToString());
            Console.WriteLine(report.summary);
            Console.WriteLine("File count: " + report.files.Length);
        }

        public static void Test()
        {
            //Todo: delete later
            Console.WriteLine("Test method");
        }

        [MenuItem("Build/Build WebGL")]
        public static void BuildWebGL()
        {
            Build("./Build/" + name + "_Web/" + name, BuildTarget.WebGL);
        }

        [MenuItem("Build/Build Windows")]
        public static void BuildWindows()
        {
            Build("./Build/" + name + "_Windows/" + name + ".exe", BuildTarget.StandaloneWindows64);
        }

        [MenuItem("Build/Build Linux")]
        public static void BuildLinux()
        {
            Build("./Build/" + name + "_Linux/" + name, BuildTarget.StandaloneLinux64);
        }

        [MenuItem("Build/Build All")]
        public static void BuildAll()
        {
            BuildLinux();
            BuildWindows();
            BuildWebGL();
        }
    }
}