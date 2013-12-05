using UnityEngine;
using System.Collections;

public class Reciever : MonoBehaviour 
{
	void Start()
	{
		Messenger.AddListener(EventMessage.OnEvent.ToString(), OnEvent);
		Sender.onEvent += DelegateResponse;
	}

	void OnEvent()
	{
		transform.Translate(Vector3.right);
	}

	void DelegateResponse()
	{
		transform.Translate(Vector3.left);
	}
}

