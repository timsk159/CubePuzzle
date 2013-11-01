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
				if(FindObjectOfType(typeof(T)) == null)
				{
					_instance = new GameObject("(Singleton)" + typeof(T).Name).AddComponent<T>();
					DontDestroyOnLoad(_instance);
				}
			}
			return _instance;
		}
	}
}
