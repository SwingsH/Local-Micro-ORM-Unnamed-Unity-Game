using UnityEditor;

namespace TIZSoft.BuildSystem
{
    public static class BuildSystemMenu
    {
        [MenuItem("Tools/Version Editor")]
        public static void OpenVersionEditor()
        {
            EditorWindow.GetWindow<VersionEditorWindow>("Version Editor");
        }
    }
}
