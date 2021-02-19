using UnityEditor;
using UnityEngine;

namespace TIZSoft.Extensions
{
    public static class SerializedPropertyExtensions
    {
        static readonly GUIContent Empty = new GUIContent();

        public static GUIContent GetGuiContent(this SerializedProperty serializedProperty)
        {
            if (serializedProperty == null)
            {
                return Empty;
            }
            return new GUIContent(serializedProperty.displayName, serializedProperty.tooltip);
        }
    }
}