using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using System.IO;
using System.Text;
using System.IO.Compression;
using System.Diagnostics;

public class BuildGame : MonoBehaviour
{
    [MenuItem("Build/Build All")]
    public static void BuildAll()
    {
        BuildLinuxServer();
        BuildWin();
    }

    [MenuItem("Build/Build Windows")]
    public static void BuildWin()
    {
        BuildPlayerOptions buildPlayerOptions = new BuildPlayerOptions();
        buildPlayerOptions.scenes = AllActiveScenes();
        buildPlayerOptions.locationPathName = "Builds/WindowsBuild/OnlineShooter-Online.exe";
        buildPlayerOptions.assetBundleManifestPath = "Builds/WindowsBuild/OnlineShooter-Online";
        buildPlayerOptions.target = BuildTarget.StandaloneWindows64;
        buildPlayerOptions.options = BuildOptions.None;
        BuildPipeline.BuildPlayer(buildPlayerOptions);

        print("BUILDED, TARGET: WINDOWS");

        Process.Start(Path.Combine(BuildsPatch, "ServerDeployer.exe"), "win");
    }
    [MenuItem("Build/Build Linux Server")]
    public static void BuildLinuxServer()
    {
        BuildPlayerOptions buildPlayerOptions2 = new BuildPlayerOptions();
        buildPlayerOptions2.scenes = AllActiveScenes();
        buildPlayerOptions2.locationPathName = "Builds/LinuxBuild/OnlineShooterServer";
        buildPlayerOptions2.target = BuildTarget.StandaloneLinux64;
        buildPlayerOptions2.options = BuildOptions.EnableHeadlessMode;
        BuildPipeline.BuildPlayer(buildPlayerOptions2);

        print("BUILDED, TARGET: LINUX");

        Process.Start(Path.Combine(BuildsPatch, "ServerDeployer.exe"), "linux");
    }

    public static string[] AllActiveScenes()
    {
        List<string> scenes = new List<string>();
        foreach (EditorBuildSettingsScene scene in EditorBuildSettings.scenes)
        {
            if (scene.enabled)
                scenes.Add(scene.path);
        }
        return scenes.ToArray();
    }

    public static string BuildsPatch => Application.dataPath.Replace("/Assets", "/Builds");
}