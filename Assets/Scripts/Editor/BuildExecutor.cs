using UnityEditor;
using UnityEngine;
using System;

namespace Pipeline
{
    public class BuildExecutor
    {
        public static string[] scenes = { "Assets/Scenes/SampleScene.unity" };
        public static string name = "venturer";

        private static int Build(string buildName, BuildTarget target)
        {
            Debug.Log("--- Building " + buildName + " ---");
            Debug.Log("Current directory: " + Environment.CurrentDirectory.ToString());

            UnityEditor.Build.Reporting.BuildReport report;
            Debug.Log("Target: " + target.ToString());
            try
            {
                report = BuildPipeline.BuildPlayer(scenes, buildName, target, BuildOptions.None);
            }
            catch (Exception e)
            {
                Debug.Log(e.ToString());
                return 1;
            }

            Debug.Log("Result: " + report.summary.result.ToString());
            Debug.Log("Output path " + report.summary.outputPath.ToString());
            Debug.Log("Total time " + report.summary.totalTime.ToString());
            Debug.Log("Total size " + report.summary.totalSize.ToString());
            Debug.Log("Total warnings " + report.summary.totalWarnings.ToString());

            var files = "File count: " + report.files.Length + "\n";
            foreach (var file in report.files)
                files += file.path + "\n";
            Debug.Log(files);

            return 0;
        }

        [MenuItem("Build/Build WebGL")]
        public static int BuildWebGL()
        {
            return Build("build/" + name + "_web/" + name, BuildTarget.WebGL);
        }

        [MenuItem("Build/Build Windows")]
        public static int BuildWindows()
        {
            return Build("build/" + name + "_windows/" + name + ".exe", BuildTarget.StandaloneWindows64);
        }

        [MenuItem("Build/Build Linux")]
        public static int BuildLinux()
        {
            return Build("build/" + name + "_linux/" + name, BuildTarget.StandaloneLinux64);
        }

        [MenuItem("Build/Build All")]
        public static int BuildAll()
        {
            return BuildLinux() | BuildWindows() | BuildWebGL();
        }
    }
}