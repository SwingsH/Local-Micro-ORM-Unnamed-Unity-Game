using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

namespace TIZSoft
{
    public class InputDialog : EditorWindow
    {
        public static void Show(string content, System.Action<string> onComplete)
        {
            InputDialog window = CreateInstance<InputDialog>();
            window.callBack = onComplete;
            window.content = content;
            window.position = new Rect(Screen.width / 2, Screen.height / 2, 250, 150);
            window.ShowPopup();
            window.firstEnter = true;
            window.Focus();
        }
        System.Action<string> callBack;
        string content = string.Empty;
        string inputString = string.Empty;
        private bool firstEnter;

        void OnGUI()
        {
            EditorGUILayout.LabelField(content);
            GUILayout.Space(20);

            GUI.SetNextControlName("InputField");
            inputString = GUILayout.TextField(inputString);
            if (firstEnter)
            {
                EditorGUI.FocusTextInControl("InputField");
                firstEnter = false;
            }
            if (Event.current.keyCode == KeyCode.KeypadEnter
                || Event.current.keyCode == KeyCode.Return)
            {
                if (callBack != null)
                    callBack(inputString);
                Close();
            }
            GUILayout.Space(20);
            GUILayout.BeginHorizontal();
            if(GUILayout.Button("Ok"))
            {
                if (callBack != null)
                    callBack(inputString);
                Close();
            }
            if (GUILayout.Button("Cancel"))
            {
                if (callBack != null)
                    callBack(string.Empty);
                Close();
            }
            GUILayout.EndHorizontal();
            var lastRect = GUILayoutUtility.GetLastRect();
            var rect = position;
            rect.yMax= lastRect.yMin;
            position = rect;
        }
    }
}