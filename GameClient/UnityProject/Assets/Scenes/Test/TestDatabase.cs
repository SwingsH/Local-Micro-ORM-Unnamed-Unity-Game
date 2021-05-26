using UnityEngine;
using TIZSoft.Database;

public class TestDatabase : MonoBehaviour
{
    public DatabaseManager databaseManager;

    // Start is called before the first frame update
    void Start()
    {
        databaseManager.Init();
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
