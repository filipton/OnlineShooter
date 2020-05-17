using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using System.IO;
using System.Text;
using System.IO.Compression;
using System.Diagnostics;
using System;
using System.Net;
using UnityEngine.Windows.WebCam;

public class BuildGame : EditorWindow
{
    int x = 0;
    int y = 0;
    int z = 0;
    int w = 0;

    bool buildWin = false;
    bool buildLin = false;

    void OnGUI()
    {
        float offsetY = -300;

        //release number area
        GUILayout.BeginArea(new Rect((Screen.width - 275) * 0.5f, (Screen.height - 100 + offsetY) * 0.5f, 275, 100));

        //x
        if (GUI.Button(new Rect(0, 0, 50, 25), "+")) { x++; }
        GUI.TextArea(new Rect(0, 25, 50, 50), x.ToString(), 2, new GUIStyle() { alignment = TextAnchor.MiddleCenter, fontSize = 20});
        if (GUI.Button(new Rect(0, 75, 50, 25), "-")) { x--; }

        //y
        if (GUI.Button(new Rect(75, 0, 50, 25), "+")) { y++; }
        GUI.TextArea(new Rect(75, 25, 50, 50), y.ToString(), 2, new GUIStyle() { alignment = TextAnchor.MiddleCenter, fontSize = 20 });
        if (GUI.Button(new Rect(75, 75, 50, 25), "-")) { y--; }

        //z
        if (GUI.Button(new Rect(150, 0, 50, 25), "+")) { z++; }
        GUI.TextArea(new Rect(150, 25, 50, 50), z.ToString(), 2, new GUIStyle() { alignment = TextAnchor.MiddleCenter, fontSize = 20 });
        if (GUI.Button(new Rect(150, 75, 50, 25), "-")) { z--; }

        //w
        if (GUI.Button(new Rect(225, 0, 50, 25), "+")) { w++; }
        GUI.TextArea(new Rect(225, 25, 50, 50), w.ToString(), 2, new GUIStyle() { alignment = TextAnchor.MiddleCenter, fontSize = 20 });
        if (GUI.Button(new Rect(225, 75, 50, 25), "-")) { w--; }

        GUILayout.EndArea();


        //windows and linux build toggle
        GUILayout.BeginArea(new Rect((Screen.width - 300) * 0.5f, (Screen.height + 150 + offsetY) * 0.5f, 300, 100));

        buildWin = EditorGUILayout.Toggle("Windows", buildWin);
        buildLin = EditorGUILayout.Toggle("Linux Server", buildLin);

        GUILayout.EndArea();


        //build and deploy area
        GUILayout.BeginArea(new Rect((Screen.width - 300) * 0.5f, (Screen.height - 100), 300, 100));

        if (GUI.Button(new Rect(75, 0, 150, 50), "BUILD & DEPLOY"))
        {
            if (buildLin)
            {
                BuildLinuxServer();
                DeployLinuxServer();
            }
            if (buildWin)
            {
                BuildWin();
                DeployWin();
            }
            SaveCurrentRelease();
        }

        GUILayout.EndArea();
    }

    [MenuItem("Build/Build/Build Windows")]
    public static void BuildWin()
    {
        BuildPlayerOptions buildPlayerOptions = new BuildPlayerOptions();
        buildPlayerOptions.scenes = AllActiveScenes();
        buildPlayerOptions.locationPathName = "Builds/WindowsBuild/OnlineShooter-Online.exe";
        buildPlayerOptions.assetBundleManifestPath = "Builds/WindowsBuild/OnlineShooter-Online";
        buildPlayerOptions.target = BuildTarget.StandaloneWindows64;
        buildPlayerOptions.options = BuildOptions.None;
        BuildPipeline.BuildPlayer(buildPlayerOptions);
    }
    [MenuItem("Build/Build/Build Linux Server")]
    public static void BuildLinuxServer()
    {
        BuildPlayerOptions buildPlayerOptions2 = new BuildPlayerOptions();
        buildPlayerOptions2.scenes = AllActiveScenes();
        buildPlayerOptions2.locationPathName = "Builds/LinuxBuild/OnlineShooterServer";
        buildPlayerOptions2.target = BuildTarget.StandaloneLinux64;
        buildPlayerOptions2.options = BuildOptions.EnableHeadlessMode;
        BuildPipeline.BuildPlayer(buildPlayerOptions2);
    }

    public static void DeployLinuxServer()
    {
        Process.Start(Path.Combine(BuildsPatch, "ServerDeployer.exe"), "linux");
    }

    public static void DeployWin()
    {
        Process.Start(Path.Combine(BuildsPatch, "ServerDeployer.exe"), "win");
    }

    [MenuItem("Build/Build And Deploy GUI")]
    public static void Test()
    {
        GetCurrentBuild();

        BuildGame window = (BuildGame)GetWindow(typeof(BuildGame));
        window.Show();
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

    public static void GetCurrentBuild()
    {
        string json = new WebClient().DownloadString("http://34.89.232.15/launcher/gameVersion.json");

        GameVersion gameVersion = JsonUtility.FromJson<GameVersion>(json);

        BuildGame window = (BuildGame)GetWindow(typeof(BuildGame));
        string[] vsts = gameVersion.version.Split('.');
        window.x = int.Parse(vsts[0]);
        window.y = int.Parse(vsts[1]);
        window.z = int.Parse(vsts[2]);
        window.w = int.Parse(vsts[3]);
    }

    public static void SaveCurrentRelease()
    {
        string json = new WebClient().DownloadString("http://34.89.232.15/launcher/gameVersion.json");
        GameVersion gameVersion = JsonUtility.FromJson<GameVersion>(json);

        BuildGame window = (BuildGame)GetWindow(typeof(BuildGame));

        new WebClient().DownloadString($"http://34.89.232.15/launcher/updateGameVersion.php?v={$"{window.x}.{window.y}.{window.z}.{window.w}"}&a={gameVersion.author}");
    }
}

[Serializable]
public struct GameVersion
{
    public string version;
    public string author;
}