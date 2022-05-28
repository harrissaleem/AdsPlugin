using System.IO;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace NaveedGameSet
{
    public class SceneViewer : EditorWindow
    {
        [MenuItem("Window/SceneViewer")]
        public static void Init()
        {
            GetWindow(typeof(SceneViewer));
        }

        private readonly Color[] color = {Color.red, Color.yellow, Color.green, Color.blue, Color.magenta};

        private void OnGUI()
        {
            // GUI.contentColor = Color.white;
            GUILayout.Space(3);
            var sytle = new GUIStyle
            {
                fontSize = 13, fontStyle = FontStyle.Bold,
                alignment = TextAnchor.UpperCenter
            };
            // sytle.normal.textColor = Color.black;

            GUILayout.Space(1);
            GUILayout.Label("Scene Viewer By Naveed", sytle);
            GUILayout.Label("----------------", sytle);

            GUILayout.Space(2);
            GUILayout.Label("[Build Scenes]", sytle);
            GUILayout.Label(".................................", sytle);
            GUILayout.Space(1);

            var styleBtn = new GUIStyle(GUI.skin.button)
                {normal = {textColor = Color.black}, fixedHeight = 30, fontSize = 15};

            var totalScenes = SceneManager.sceneCountInBuildSettings;
            for (var i = 0; i < totalScenes; i++)
            {
                GUI.color = color[i % color.Length];
                GUI.contentColor = Color.white;
                GUILayout.Space(3);
                var path = SceneUtility.GetScenePathByBuildIndex(i);
                var sceneName = Path.GetFileNameWithoutExtension(path);
                if (!GUILayout.Button(sceneName, styleBtn)) continue;
                EditorSceneManager.SaveCurrentModifiedScenesIfUserWantsTo();
                EditorSceneManager.OpenScene(path);
            }
        }
    }
}