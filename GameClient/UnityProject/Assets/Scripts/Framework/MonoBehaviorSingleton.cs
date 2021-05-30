using UnityEngine;
using TIZSoft.Utils;

public class MonoBehaviorSingleton<T> : MonoBehaviour where T : MonoBehaviour
{
	protected static T _instance;
	private static readonly object _instanceLock = new object();
	private static bool _isQuitting = false;

	public static T Instance
	{
		get
		{
			lock (_instanceLock)
			{
				if (_instance == null)
				{
					Init();
				}

				if (_isQuitting)
				{
					Debug.LogError(string.Format("You cannot access {0} while application is quit.", typeof(T).FullName));
					return null;
				}

				return _instance;
			}
		}
	}

	protected static void Init()
    {
		if (_instance != null)
        {
			Debug.LogWarning(string.Format("_instance != null {0} no init require.", typeof(T).FullName));
			return;
		}
		if (_isQuitting)
		{
			Debug.LogError(string.Format("You cannot access {0} while application is quit.", typeof(T).FullName));
			return;
		}

		var candidates = UnityUtils.FindObjectsOfType<T>(true);
		if (candidates.Length > 0)
		{
			if (candidates.Length > 1)
			{
				Debug.LogWarning(string.Format("Multiple instance of {0} detected. Count = {1}.", typeof(T).FullName, candidates.Length));
			}

			_instance = candidates[0];
			return;
		}

		if (_instance == null)
		{
			string goName = string.Format("{0}Singleton", typeof(T).ToString());
			GameObject go = new GameObject(goName, typeof(T));
			go.hideFlags = HideFlags.DontSave;
		}
	}

	protected virtual void Awake(){
		if(!_instance){
			Init();
		}
		DontDestroyOnLoad(_instance.gameObject);

		if (_instance == null){
			_instance = gameObject.GetComponent<T>();
		}
		
		if(_instance == null)
        {
			_instance = this as T;
			throw new System.Exception(string.Format("Instance of {0} implement in unnormal way.", GetType().FullName));

		}
		else if (_instance.GetInstanceID() != GetInstanceID()){
			Destroy(gameObject);
			throw new System.Exception(string.Format("Instance of {0} already exists, removing {1}", GetType().FullName, ToString()));
		}
	}

	protected virtual void OnApplicationQuit()
	{
		_isQuitting = true;
	}
}
