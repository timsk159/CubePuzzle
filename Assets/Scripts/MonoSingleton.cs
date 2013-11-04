using UnityEngine;
using System.Collections;

public class MonoSingleton<T> : MonoBehaviour where T : MonoBehaviour
{
	private static T _instance;

	public static T Instance 
	{
		get
		{
			if(_instance == null)
			{
				_instance = (T)FindObjectOfType(typeof(T));

				if(_instance == null)
				{
					_instance = new GameObject("(Singleton)" + typeof(T).Name).AddComponent<T>();
					DontDestroyOnLoad(_instance);
				}
			}
			return _instance;
		}
	}
}
