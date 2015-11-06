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
	public float maxTimeBetweenDrops;
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
	private Toggle on;
	private Slider intensity;

	// Every MonoBehavior has a Start function which is called upon instantiation into the Scene / Hierarchy
	void Start () {
		// Set class specific constants
		origin = new Vector3 (0, 0, 0);
		identity = Quaternion.identity;
		on = onSwitch.GetComponent<Toggle> ();
		intensity = IntensitySlider.GetComponent<Slider> ();

		int numRows = 5;
		int numCols = 3;
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
		root.CreateTree (this, null, numRows, numCols);
		// Now prime the pump by adding drops to the available drops queue
		StartCoroutine (PopulateDrops());
		// And now start dropping them on a regular timer
		StartCoroutine (StartDropping ());
		// Implied: Gravity is included in the drop prefab - Rain will begin with the instantion of this script.
	}

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
			Debug.LogError ("Waiting " + delay + " seconds for the next drop");
			root.Dispense ();
			yield return new WaitForSeconds (delay);
		}
		Debug.LogError ("Turning rain off.");
	}

	private IEnumerator PopulateDrops()
	{
		while (numDrops < maxNumDrops) {
			for (int i = 0; i < numDropsPerCreationCycle; i++)
			{
				dropPointer = Instantiate (dropPrefab,origin,identity) as GameObject;
				dropPointer.GetComponent<Drop>().Disable ();
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
