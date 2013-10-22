using UnityEngine;
using System.Collections;

public class CameraFollow : MonoBehaviour 
{
	public Transform target;
	private float distance = 6.0f;
	private float height = 7.0f;
	public float damp = 5.0f;
	public Vector3 offset;
	float dotCamToPlayer;

	IEnumerator Start()
	{
		while(target == null)
		{
			yield return new WaitForEndOfFrame();
		}
		transform.LookAt(target, target.up);

	}

	void FixedUpdate () 
	{
		if(target != null)
		{
			if(dotCamToPlayer == 0)
			{
				var forward = transform.TransformDirection(Vector3.forward);
				var toOther = target.position - transform.position;
				dotCamToPlayer = Vector3.Dot(forward, toOther);
			}
			var newPos = Vector3.zero;
			newPos.x = target.position.x;
			newPos.y = target.position.y + height;
			if(dotCamToPlayer > 0)
			{
				newPos.z = target.position.z - distance;
			}
			else
			{
				newPos.z = target.position.z + distance;
			}
			newPos += offset;

			transform.position = Vector3.Lerp(transform.position, newPos, damp * Time.deltaTime);

			transform.LookAt(target, Vector3.up);
		}
	}
}