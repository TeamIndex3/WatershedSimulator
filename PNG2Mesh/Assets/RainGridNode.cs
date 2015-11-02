using UnityEngine;
using System.Collections;

public class RainGridNode : MonoBehaviour {

	public RainGridNode parent;
	public RainGridNode[] children;
	public int numChildren;
	public Vector3 location;

	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
	
	}

	private int GetNextChild()
	{
		return 0;
	}

	public void NextChild()
	{
		if (numChildren == 0) {
			DropRain ();
		} else {
			children[GetNextChild()].NextChild();
		}
	}

	private void DropRain()
	{
		// Take a drop from the pool, 
		// give it this location, 
		// give it a yield return new waitforseconds that is random
		// reduce the availabledrop counter;
		// Then enable the drop.
		Debug.LogError ("Dropping drop at location " + location[0] + "," + location[1] + "," + location[2]);
	}
}
