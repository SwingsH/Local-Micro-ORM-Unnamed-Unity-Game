using UnityEngine;

public class UnitySingletonBase<T> : MonoBehaviour where T : Component
{
	private static T _instance;
	private static readonly object _instanceLock = new object();
	private static bool _isQuitting = false;

	public static T instance
	{
		get
		{
			lock (_instanceLock)
			{
				if (_instance == null && !_isQuitting)
				{

					_instance = FindObjectOfType<T>();
					if (_instance == null)
					{
						GameObject go = new GameObject();
						go.name = string.Format("{0}Singleton", typeof(T).ToString());
						go.hideFlags = HideFlags.DontSave;
						_instance = go.AddComponent<T>();
					}
				}

				return _instance;
			}
		}
	}

	protected virtual void Awake(){
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
