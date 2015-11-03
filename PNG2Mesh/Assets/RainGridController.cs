/*
Watershed Simulator Rain Controller
Author: Max Kohl 
Contributors in alphabetic order by last name:
        Abdulmajeed Kadi, Garrett Morrison, Hannah Smith, Joshua Yang
*/

using UnityEngine;
using System.Collections;
using System.Collections.Generic;


public class RainGridController : MonoBehaviour {

	// Public data members - can be modified from the inspector. 
	// Some of these are required to be populated for this script to run.

	// REQUIRED:
	public RainGridNode root;
	public GameObject dropPrefab;
	public int maxNumDrops;
	public int numDrops;
	public int numRows;
	public int numCols;
	public float centerX;
	public float centerY;
	public float centerZ;
	public float scaleX;
	public float scaleY;
	public float frequency;
	public float lengthX;
	public float lengthY;
	public float height;
	public int numDropsAvailable;
	public Seed seed;
	public int maxTimeBetweenDrops;
	public Queue<GameObject> availableDrops;

	// Private data members - Not available to the Inspector
	private Vector3 location;
	private GameObject[] drops;
	private GameObject currentDrop;
	private float dropCreationDelaySeconds = 1;
	private int numDropsPerCreationCycle = 10;
	private Quaternion identity;
	private Vector3 origin;
	private GameObject dropPointer;

	// Every MonoBehavior has a Start function which is called upon instantiation into the Scene / Hierarchy
	void Start () {
		/*
		 * // Make sure a value is defined in the numDrops location in the inspector for this object
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
		//CreatePool ();
		*/
		origin = new Vector3 (0, 0, 0);
		identity = Quaternion.identity;
		seed = new Seed ();
		numDropsAvailable = 0;
		availableDrops = new Queue<GameObject>();
		int numRows = 5;
		int numCols = 3;
		root = (RainGridNode)ScriptableObject.CreateInstance("RainGridNode");
		root.Init (location);
		root.CreateTree (this, null, numRows, numCols);
		StartCoroutine (PopulateDrops());
		// Implied: Gravity is included in the drop prefab - Rain will begin with the instantion of this script.
	}

	private IEnumerator PopulateDrops()
	{
		while (numDrops < maxNumDrops) {
			for (int i = 0; i < numDropsPerCreationCycle; i++)
			{
				dropPointer = Instantiate (dropPrefab,origin,identity) as GameObject;
				availableDrops.Enqueue (dropPointer);
				numDropsAvailable++;
				numDrops++;
			}
			yield return new WaitForSeconds (dropCreationDelaySeconds);
		}
	}

	public void RemoveDropFromQueue(Vector3 location)
	{
		if (numDropsAvailable > 0)
		{
			float delay = seed.GetValueAsPercent() * maxTimeBetweenDrops;
			GameObject drop = availableDrops.Dequeue ();
			numDropsAvailable--;
			drop.transform.position = location;
			StartCoroutine (BeginDropping(drop, delay));
		}
	}

	private IEnumerator BeginDropping(GameObject drop, float delay)
	{
		yield return new WaitForSeconds (delay);
		Debug.LogError ("Drop it like it's hot!");
		//drop.GetComponent<Drop> ().Enable ();
		//drop.Enable ();
	}

	// Update is called once per frame
	void Update () {
		
	}

	// This creates a pool of Rain Drop objects.
	private void CreatePool()
	{
		// Make sure there is something defined in this script's inspector's dropPrefab slot 
		if (dropPrefab == null) {
			return;
		}
		// Create a non-rotated Quaternion matrix to be used in drop instantiation.
		Quaternion identity = Quaternion.identity;
		Vector3 origin = new Vector3 (0, 0, 0);
		// Allocate the array which holds the available rain drops.
		drops = new GameObject[numDrops];
		// Populate the array with instances of rain drops, each located slightly above the next.
		for (int i = 0; i < numDrops; i++) {
			location = new Vector3(centerX,centerY+i,centerZ);
			currentDrop = Instantiate (dropPrefab,origin,identity) as GameObject;
			currentDrop.transform.position = location;
			drops[i] = currentDrop;
		}
	}

	public void AddToQueue(GameObject drop)
	{
		this.availableDrops.Enqueue (drop);
		numDropsAvailable ++;
	}
}
