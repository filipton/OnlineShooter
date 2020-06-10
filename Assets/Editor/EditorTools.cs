using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;

public class CEditorTools : EditorWindow
{
    private GUIStyle guiStyle = new GUIStyle();

    [MenuItem("Window/C-Editor Tools")]
    static void Init()
    {
        // Get existing open window or if none, make a new one:
        CEditorTools window = (CEditorTools)GetWindow(typeof(CEditorTools));
        window.Show();
    }

    void OnGUI()
    {
        guiStyle.fontSize = 20;
        guiStyle.fontStyle = FontStyle.Bold;
        guiStyle.alignment = TextAnchor.MiddleCenter;
        guiStyle.normal.textColor = Color.white;

        GUILayout.Label("Scenes: ", guiStyle);
        string[] scenes = AllActiveScenes();
        for(int i = 0; i < scenes.Length; i++)
		{
            if (GUI.Button(new Rect(new Vector2(0, (i+1)*30), new Vector2(425, 30)), scenes[i]))
            {
                EditorApplication.ExecuteMenuItem("File/Save");
                EditorSceneManager.OpenScene(scenes[i]);
            }
        }
        GUILayout.Space((scenes.Length * 30) + 10);
        GUILayout.Label("Tools: ", guiStyle);
        if (GUI.Button(new Rect(new Vector2(0, ((scenes.Length + 2) * 30)), new Vector2(100, 30)), "SAVE SCENE"))
        {
            EditorApplication.ExecuteMenuItem("File/Save");
        }
        if (GUI.Button(new Rect(new Vector2(0, ((scenes.Length + 3) * 30)), new Vector2(100, 30)), "BUILD GUI"))
		{
            EditorApplication.ExecuteMenuItem("Build/Build And Deploy GUI");
        }
        if (GUI.Button(new Rect(new Vector2(100, ((scenes.Length + 3) * 30)), new Vector2(100, 30)), "BUILD WIN"))
        {
            EditorApplication.ExecuteMenuItem("Build/Build/Build Windows");
        }
        if (GUI.Button(new Rect(new Vector2(200, ((scenes.Length + 3) * 30)), new Vector2(100, 30)), "BUILD LINUX"))
        {
            EditorApplication.ExecuteMenuItem("Build/Build/Build Windows");
        }
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
}