using UnityEngine;
using System.Collections;

public class MapRoot : MonoBehaviour 
{
	[SerializeThis]
	public int mapSize;

	void Start()
	{
		UniqueIdentifier id = GetComponent<UniqueIdentifier>();
		if(id == null)
		{
			id = gameObject.AddComponent<UniqueIdentifier>();
		}

		StoreInformation infoStore = GetComponent<StoreInformation>();
		if(infoStore == null)
		{
			infoStore = gameObject.AddComponent<StoreInformation>();
		}
	}
}
