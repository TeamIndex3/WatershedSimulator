using UnityEngine;
using System.Collections;

public class Rain : MonoBehaviour {

	// Public data members - can be modified from the inspector. 
	//Some of these are required to be populated for this script to run.
	// REQUIRED:
	public int numDrops;
	public GameObject dropPrefab;
	public float centerX;
	public float centerY;
	public float centerZ;
	// Optional:
	public float lengthX;
	public float lengthY;
	// Private data members - Not available to the Inspector
	private Vector3 location;
	private GameObject[] drops;
	private GameObject currentDrop;

	// Every MonoBehavior has a Start function which is called upon instantiation into the Scene / Hierarchy
	void Start () {
		// Make sure a value is defined in the numDrops location in the inspector for this object
		if (numDrops == 0 || numDrops == null) {
			numDrops = 10;
		}
		// Make sure a default location is defined for this object in the inspector.
		if (centerX == null || centerY == null || centerZ == null)
		{
			return;
		}
		// Store the default location as a vector.
		location = new Vector3(centerX, centerY, centerZ);
		// Set this object's location to the default location provided
		this.gameObject.transform.position = location;
		// Create the rain drop pool.
		CreatePool ();

		// Implied: Gravity is included in the drop prefab - Rain will begin with the instantion of this script.
	}
	
	// Update is called once per frame
	void Update () {
		
	}

	// This creates a pool of Rain Drop objects.
	void CreatePool()
	{
		// Make sure there is something defined in this script's inspector's dropPrefab slot 
		if (dropPrefab == null) {
			return;
		}
		// Create a non-rotated Quaternion matrix to be used in drop instantiation.
		Quaternion identity = Quaternion.identity;
		// Allocate the array which holds the available rain drops.
		drops = new GameObject[numDrops];
		// Populate the array with instances of rain drops, each located slightly above the next.
		for (int i = 0; i < numDrops; i++) {
			location = new Vector3(centerX,centerY+i,centerZ);
			currentDrop = Instantiate (dropPrefab,location,identity) as GameObject;
			drops[i] = currentDrop;
		}
	}
}
