using UnityEngine;
using System.Collections;

public class BasePlane : MonoBehaviour {

	// Use this for initialization
	void Start () {
	
	}
	void OnCollisionEnter(Collision collision)
	{

		if (collision.collider is SphereCollider) {
			var thing = collision.gameObject.GetComponent<Drop> ();
			if (thing != null)
			{
				thing.Disable ();
			}
			else
			{
				Debug.LogError ("BasePlane casting sphere collider as Drop failed. Collider: " + collision.collider.name);
			}
		}
		else
		{
			Debug.LogError ("BasePlane experienced collision with non-sphere collider. Collider: " + collision.collider.name);
		}
	}
	// Update is called once per frame
	void Update () {
	
	}
}
