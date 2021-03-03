using UnityEngine;

/// <summary>
/// DDOL singleton.
/// </summary>
public abstract class DDOLSingleton<T> : MonoBehaviour where T : DDOLSingleton<T>
{
	protected static T _Instance = null;
	
	public static T Instance
	{
        get{
			if (null == _Instance)
			{
				GameObject go = GameObject.Find("DDOLGameObject");
				if (null == go)
				{
					go = new GameObject("DDOLGameObject");
					DontDestroyOnLoad(go);
				}
				_Instance = go.GetOrAddComponent<T>();
				_Instance.Init ();

			}
			return _Instance;
		}
	}

	public virtual void Init()
	{

	}

	public virtual void Release()
	{

	}

	/// <summary>
	/// Raises the application quit event.
	/// </summary>
	private void OnApplicationQuit ()
	{
		Release ();
		_Instance = null;
	}
}

