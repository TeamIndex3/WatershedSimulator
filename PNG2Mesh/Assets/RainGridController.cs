/*
Watershed Simulator Rain Controller
Author: Max Kohl 
Contributors in alphabetic order by last name:
        Abdulmajeed Kadi, Garrett Morrison, Hannah Smith, Joshua Yang
*/

using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;


public class RainGridController : MonoBehaviour {

	// Public data members - can be modified from the inspector. 
	// Some of these are required to be populated for this script to run.

	// REQUIRED:
	public RainGridNode root;
	public GameObject dropPrefab;
	public GameObject onSwitch;
	public GameObject IntensitySlider;
	public int maxNumDrops;
	public int numDrops;
	public int numZSteps;
	public int numXSteps;
	public float centerX;
	public float centerY;
	public float centerZ;
	public float scaleX;
	public float scaleZ;
	public float frequency;
	public float lengthX;
	public float lengthZ;
	public float height;
	public int numDropsAvailable;
	public Seed seed;
	public float maxTimeBetweenDrops;
	public Queue<GameObject> availableDrops;
	public GameObject surface;
	public float startX;
	//public float endX;
	public float startZ;
	//public float endZ;
	public float xStep;
	public float zStep;

	// Private data members - Not available to the Inspector
	private Vector3 location;
	private GameObject[] drops;
	private GameObject currentDrop;
	private float dropCreationDelaySeconds = 1;
	private int numDropsPerCreationCycle = 100;
	private Quaternion identity;
	private Vector3 origin;
	private GameObject dropPointer;
	private Toggle on;
	private Slider intensity;

	// Every MonoBehavior has a Start function which is called upon instantiation into the Scene / Hierarchy
	void Start () {
		// Set class specific constants
		origin = new Vector3 (0, 0, 0);
		identity = Quaternion.identity;
		on = onSwitch.GetComponent<Toggle> ();
		intensity = IntensitySlider.GetComponent<Slider> ();

		// Initialize the random number generator
		// Use any integer seed value to track your procedural rainfall
		seed = new Seed ();
		// Initialize drop pool and living drop counter. 
		// Ideally numDropsAvailable will track how many drops have not been processed into rain.
		// This should end up being slightly less than or equal to the current size of the Queue in any given frame.
		availableDrops = new Queue<GameObject>();
		numDropsAvailable = 0;
		// Create the root node of the showerhead tree
		root = (RainGridNode)ScriptableObject.CreateInstance("RainGridNode");
		// Give it a base location Vector3 (the center point of the rain grid)
		location = new Vector3 (centerX, centerY, centerZ);
		root.Init (location);
		// Recursively create the showerhead tree with a pointer to the following objects:
		// This class, a null parent, and the dimensions of the tree.
		root.CreateTree (this, null, numZSteps, numXSteps);
		// Now prime the pump by adding drops to the available drops queue
		StartCoroutine (PopulateDrops());
		// And now start dropping them on a regular timer
		StartCoroutine (StartDropping ());
		// Implied: Gravity is included in the drop prefab - Rain will begin with the instantion of this script.
	}

	void Awake()
	{
		
		GetDimensions ();
	}

	void GetDimensions()
	{

		var bounds = surface.GetComponentsInChildren<MeshRenderer> ();
		float minX = 0.0f;
		float maxX = 0.0f;
		float minZ = 0.0f;
		float maxZ = 0.0f;
		bool updated = false;
		foreach (MeshRenderer mesh in bounds) {
			if (updated)
			{
				minZ = Mathf.Min (minZ,mesh.bounds.extents.z);
				maxZ = Mathf.Max (maxZ,mesh.bounds.extents.z);
				minX = Mathf.Min (minX,mesh.bounds.extents.x);
				maxX = Mathf.Max (maxX,mesh.bounds.extents.x);
			}
			else
			{
				minZ = mesh.bounds.extents.z;
				maxZ = mesh.bounds.extents.z;
				minX = mesh.bounds.extents.x;
				maxX = mesh.bounds.extents.x;
				updated = true;
			}
		}
		lengthX = scaleX*(maxX - minX);
		lengthZ = scaleZ*(maxZ - minZ);
		startX = centerX - (lengthX / 2);
		float endX = centerX + (lengthX / 2);
		startZ = centerZ - (lengthZ / 2);
		float endZ = centerZ + (lengthZ / 2);
		xStep = lengthX / numXSteps;
		zStep = lengthZ / numZSteps;
	}

	// GUI input handler functions
	public void HandleSwitch()
	{
		if (on == null) {
			return;
		}
		// All we care about is if the switch turns on
		if (on.isOn) {
			Debug.LogError("Turning rain on");
			StartCoroutine(StartDropping ());
		}
	}
	
	public void HandleIntensity()
	{
		if (intensity == null)
		{
			return;
		}
		frequency = intensity.normalizedValue;
	}

	private IEnumerator StartDropping()
	{	
		frequency = intensity.normalizedValue;
		// Only drop rain when the on switch is...on
		while (on.isOn) {
			// Invert the slider so that a frequency of 1 causes a very small amount of time to pass between dropping
			float delay = (1 - Mathf.Max (0.0001f, frequency)) * (maxTimeBetweenDrops);
			//Debug.LogError ("Waiting " + delay + " seconds for the next drop");
			root.Dispense ();
			yield return new WaitForSeconds (delay);
		}
		Debug.LogError ("Turning rain off.");
	}

	private IEnumerator PopulateDrops()
	{
		Drop dropScript;
		while (numDrops < maxNumDrops) {
			for (int i = 0; i < numDropsPerCreationCycle; i++)
			{
				dropPointer = Instantiate (dropPrefab,origin,identity) as GameObject;
				dropPointer.transform.parent = this.transform;
				dropScript = dropPointer.GetComponent<Drop>();
				dropScript.ID = numDrops;
				dropScript.Disable ();
				// Drop is already added to queue in disable function - this was the cause of drops being double counted.
				//AddToQueue (dropPointer);
				//availableDrops.Enqueue (dropPointer);
				//numDropsAvailable++;
				numDrops++;
			}
			yield return new WaitForSeconds (dropCreationDelaySeconds);
		}
	}

	public void RemoveDropFromQueue(Vector3 location)
	{
		if (numDropsAvailable > 50)
		{
			float delay = seed.GetValueAsPercent() * maxTimeBetweenDrops;
			numDropsAvailable--;
			GameObject drop = availableDrops.Dequeue ();
			drop.transform.position = location;
			StartCoroutine (BeginDropping(drop, delay));
		}
	}

	private IEnumerator BeginDropping(GameObject drop, float delay)
	{
		yield return new WaitForSeconds (delay);
		drop.GetComponent<Drop> ().Enable ();
		//drop.Enable ();
	}

	// Update is called once per frame
	void Update () {
		
	}

	public void AddToQueue(GameObject drop)
	{
		this.availableDrops.Enqueue (drop);
		numDropsAvailable ++;
	}
}
