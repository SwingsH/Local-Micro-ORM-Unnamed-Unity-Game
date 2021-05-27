using UnityEngine;
using TIZSoft.Database;
using TIZSoft.Utils.Log;

public class TestDatabase : MonoBehaviour
{
    public DatabaseManager databaseManager;

    static readonly TIZSoft.Utils.Log.Logger logger = LogManager.Default.FindOrCreateCurrentTypeLogger();

    // Start is called before the first frame update
    void Start()
    {
        databaseManager.Init();
        logger.Log<string>(LogLevel.Info, name);
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
