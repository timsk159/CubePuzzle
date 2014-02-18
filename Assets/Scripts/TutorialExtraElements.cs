using UnityEngine;
using System.Collections;
using System.Linq;

public class TutorialExtraElements : MonoBehaviour 
{
	[SerializeField]
	private GameObject[] elements;

	public GameObject GetElement(string gameobjectName)
	{
		return elements.Where(element => element.name == gameobjectName).FirstOrDefault();
	}
}
