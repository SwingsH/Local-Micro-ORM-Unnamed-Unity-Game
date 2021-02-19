using System.IO;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;

namespace TIZSoft
{
    /// <summary>
    /// 提供 scene 列表。
    /// </summary>
    public class SceneViewWindow : EditorWindow
    {
        Vector2 scrollPos;
        
        [MenuItem("Window/Scene List")]
        public static void OpenSceneList()
        {
            GetWindow<SceneViewWindow>(false, "Scene List");
        }
        
        void OnGUI()
        {
            scrollPos = EditorGUILayout.BeginScrollView(scrollPos);
            EditorGUILayout.BeginVertical();
            {
                GUILayout.Label("Scenes In Build", EditorStyles.boldLabel);
                for (var i = 0; i != EditorBuildSettings.scenes.Length; ++i)
                {
                    var scene = EditorBuildSettings.scenes[i];
                    var sceneName = Path.GetFileNameWithoutExtension(scene.path);
                    var displayName = string.Format("[{0}] {1}", i, sceneName ?? string.Empty);
                    EditorGUILayout.BeginHorizontal();
                    {
                        scene.enabled = EditorGUILayout.ToggleLeft(displayName, scene.enabled, GUILayout.MaxWidth(160));
                        if (GUILayout.Button("Open"))
                        {
                            if (EditorSceneManager.SaveCurrentModifiedScenesIfUserWantsTo())
                            {
                                EditorSceneManager.OpenScene(scene.path);
                            }
                        }
                    }
                    EditorGUILayout.EndHorizontal();
                }
            }
            EditorGUILayout.EndVertical();
            EditorGUILayout.EndScrollView();
        }
    }
}
