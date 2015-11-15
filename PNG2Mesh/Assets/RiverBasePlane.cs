using UnityEngine;
using System.Collections;

public class RiverBasePlane : MonoBehaviour {
	
	// Use this for initialization
	void Start () {
		
	}
	void OnCollisionEnter(Collision collision)
	{
		
		if (collision.collider is SphereCollider) {
			var thing = collision.gameObject.GetComponent<RiverDrop> ();
			if (thing != null)
			{
				thing.Disable ();
			}
			else
			{
				Debug.LogError ("RiverBasePlane casting sphere collider as riverDrop failed. Collider: " + collision.collider.name);
			}
		}
		else
		{
			Debug.LogError ("RiverBasePlane experienced collision with non-sphere collider. Collider: " + collision.collider.name);
		}
	}
	// Update is called once per frame
	void Update () {
		
	}
}
