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
			thing.Disable ();
		}
	}
	// Update is called once per frame
	void Update () {
	
	}
}
