using UnityEditor;
using UnityEngine;
using System;

namespace Pipeline
{
    public class FileLogger
    {
        private string filePath;

        public FileLogger(string path = @"./Build/build.log")
        {
            filePath = path;
            if (System.IO.File.Exists(filePath))
                System.IO.File.Delete(filePath);

            var dir = System.IO.Path.GetDirectoryName(filePath);
            if (!System.IO.Directory.Exists(dir))
                System.IO.Directory.CreateDirectory(dir);

            using (var writer = new System.IO.StreamWriter(System.IO.File.Create(path)))
            {
                writer.WriteLine("------ Build log ------");
                writer.Close();
            }
        }

        public void Log(string message)
        {
            Debug.Log(message);
            using (var writer = System.IO.File.AppendText(filePath))
            {
                writer.WriteLine(message);
                writer.Close();
            }
        }
    }

    public class BuildExecutor
    {
        public static string[] scenes = { "Assets/Scenes/SampleScene.unity" };
        public static string name = "Venturer";
        private static FileLogger logger = new FileLogger();

        private static int Build(string buildName, BuildTarget target)
        {
            logger.Log("--- Building " + buildName + " ---");
            logger.Log("Current directory: " + Environment.CurrentDirectory.ToString());

            UnityEditor.Build.Reporting.BuildReport report;
            logger.Log("Target: " + target.ToString());
            try
            {
                report = BuildPipeline.BuildPlayer(scenes, buildName, target, BuildOptions.None);
            }
            catch (Exception e)
            {
                logger.Log(e.ToString());
                return 1;
            }
            logger.Log(report.summary.result.ToString());
            logger.Log(report.summary.ToString());
            logger.Log("File count: " + report.files.Length);
            logger.Log("----------------\n");
            return 0;
        }

        [MenuItem("Build/af All")]
        public static int Test()
        {
            logger.Log("--- Test ---");
            logger.Log("Test method");
            logger.Log("Test");
            return 0;
        }

        [MenuItem("Build/Build WebGL")]
        public static int BuildWebGL()
        {
            return Build("./Build/" + name + "_Web/" + name, BuildTarget.WebGL);
        }

        [MenuItem("Build/Build Windows")]
        public static int BuildWindows()
        {
            return Build("./Build/" + name + "_Windows/" + name + ".exe", BuildTarget.StandaloneWindows64);
        }

        [MenuItem("Build/Build Linux")]
        public static int BuildLinux()
        {
            return Build("./Build/" + name + "_Linux/" + name, BuildTarget.StandaloneLinux64);
        }

        [MenuItem("Build/Build All")]
        public static int BuildAll()
        {
            return BuildLinux() | BuildWindows() | BuildWebGL();
        }
    }
}