using UnityEngine;
using UnityEditor;
using TIZSoft.Database;


[CustomEditor(typeof(DeployDatabase))]
public class DeloyDatabaseEditor : Editor
{
    public override void OnInspectorGUI()
    {
        base.OnInspectorGUI();
        
        if (GUILayout.Button("Batch Create Table"))
        {
            Debug.Log("Batch Create Table in " + target.name);
            DeployDatabase deployDB = base.target as DeployDatabase;
            deployDB.BatchCreateTable();
        }
    }
}
