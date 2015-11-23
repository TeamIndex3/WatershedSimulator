/*
Watershed Simulator River Controller
Author: Max Kohl 
Contributors in alphabetic order by last name:
        Abdulmajeed Kadi, Garrett Morrison, Hannah Smith, Joshua Yang
*/

using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;


public class RiverGridController : MonoBehaviour {
	
	// Public data members - can be modified from the inspector. 
	// Some of these are required to be populated for this script to run.
	
	// REQUIRED:
	public RiverGridNode root;
	public GameObject riverDropPrefab;
	public GameObject onSwitch;
	public GameObject DischargeSlider;
	public int maxNumDrops;
	public int numDrops;
	public int numZSteps;
	public int numYSteps;
	public float centerX;
	public float centerY;
	public float centerZ;
	public float scaleX;
	public float scaleY;
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
	public float startY;
	//public float endX;
	public float startZ;
	//public float endZ;
	public float yStep;
	public float zStep;
	public int minNumDropsAvailable;
	
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
	private Slider discharge;
	
	// Every MonoBehavior has a Start function which is called upon instantiation into the Scene / Hierarchy
	void Start () {
		// Set class specific constants
		origin = new Vector3 (0, 0, 0);
		identity = Quaternion.identity;
		on = onSwitch.GetComponent<Toggle> ();
		discharge = DischargeSlider.GetComponent<Slider> ();
		
		// Initialize the random number generator
		// Use any integer seed value to track your procedural river creation
		seed = new Seed ();
		// Initialize drop pool and living drop counter. 
		// Ideally numDropsAvailable will track how many drops have not been processed into rain.
		// This should end up being slightly less than or equal to the current size of the Queue in any given frame.
		availableDrops = new Queue<GameObject>();
		numDropsAvailable = 0;
		// Create the root node of the showerhead tree
		root = (RiverGridNode)ScriptableObject.CreateInstance("RiverGridNode");
		// Give it a base location Vector3 (the center point of the river grid)
		location = new Vector3 (startX, centerY, centerZ);
		root.Init (location);
		// Recursively create the showerhead tree with a pointer to the following objects:
		// This class, a null parent, and the dimensions of the tree.
		root.CreateTree (this, null, numZSteps, numYSteps);
		// Now prime the pump by adding drops to the available drops queue
		StartCoroutine (PopulateDrops());
		// And now start dropping them on a regular timer
		StartCoroutine (StartDropping ());
		// Implied: Gravity and constant force is included in the drop prefab - River will begin with the instantion of this script.
	}
	
	void Awake()
	{
		
		GetDimensions ();
	}
	
	void GetDimensions()
	{
		
		var surfaceMeshRenderers = surface.GetComponentsInChildren<MeshRenderer> ();
		float minX = 0.0f;
		float maxX = 0.0f;
		float minZ = 0.0f;
		float maxZ = 0.0f;
		bool updated = false;
		Bounds encapsulatedBounds;
		if (surfaceMeshRenderers!= null && surfaceMeshRenderers.Length > 0) {
			encapsulatedBounds = surfaceMeshRenderers [0].bounds;
			foreach (MeshRenderer mesh in surfaceMeshRenderers) {
				encapsulatedBounds.Encapsulate(mesh.bounds);
				if (updated) {
					minZ = Mathf.Min (minZ, mesh.bounds.extents.z);
					maxZ = Mathf.Max (maxZ, mesh.bounds.extents.z);
					minX = Mathf.Min (minX, mesh.bounds.extents.x);
					maxX = Mathf.Max (maxX, mesh.bounds.extents.x);
				} else {
					minZ = mesh.bounds.extents.z;
					maxZ = mesh.bounds.extents.z;
					minX = mesh.bounds.extents.x;
					maxX = mesh.bounds.extents.x;
					updated = true;
				}
			}
			centerX = encapsulatedBounds.center.x;
			centerY = encapsulatedBounds.max.y;
			centerZ = encapsulatedBounds.center.z;

			Vector3 p0;
			Vector3 p1;
			p0 = new Vector3(encapsulatedBounds.min.x,encapsulatedBounds.min.y,encapsulatedBounds.min.z);
			p1 = new Vector3(encapsulatedBounds.max.x,encapsulatedBounds.min.y,encapsulatedBounds.min.z);
			Debug.DrawLine (p0,p1,Color.red,2f,false);
			p0 = new Vector3(encapsulatedBounds.min.x,encapsulatedBounds.min.y,encapsulatedBounds.min.z);
			p1 = new Vector3(encapsulatedBounds.min.x,encapsulatedBounds.max.y,encapsulatedBounds.min.z);
			Debug.DrawLine (p0,p1,Color.red,2f,false);
			p0 = new Vector3(encapsulatedBounds.min.x,encapsulatedBounds.min.y,encapsulatedBounds.min.z);
			p1 = new Vector3(encapsulatedBounds.min.x,encapsulatedBounds.min.y,encapsulatedBounds.max.z);
			Debug.DrawLine (p0,p1,Color.red,2f,false);
			p0 = new Vector3(encapsulatedBounds.min.x,encapsulatedBounds.max.y,encapsulatedBounds.max.z);
			p1 = new Vector3(encapsulatedBounds.max.x,encapsulatedBounds.max.y,encapsulatedBounds.max.z);
			Debug.DrawLine (p0,p1,Color.red,2f,false);
			p0 = new Vector3(encapsulatedBounds.max.x,encapsulatedBounds.min.y,encapsulatedBounds.max.z);
			p1 = new Vector3(encapsulatedBounds.max.x,encapsulatedBounds.max.y,encapsulatedBounds.max.z);
			Debug.DrawLine (p0,p1,Color.red,2f,false);
			p0 = new Vector3(encapsulatedBounds.max.x,encapsulatedBounds.max.y,encapsulatedBounds.min.z);
			p1 = new Vector3(encapsulatedBounds.max.x,encapsulatedBounds.max.y,encapsulatedBounds.max.z);
			Debug.DrawLine (p0,p1,Color.red,2f,false);
			//Debug.DrawLine (encapsulatedBounds.min.y, encapsulatedBounds.max.y,Color.red,2f,false);
			//Debug.DrawLine (encapsulatedBounds.min.z, encapsulatedBounds.max.z,Color.red,2f,false);
			/*Debug.DrawLine (encapsulatedBounds.min, encapsulatedBounds.max,Color.red,2f,false);
			Debug.DrawLine (encapsulatedBounds.min, encapsulatedBounds.max,Color.red,2f,false);
			Debug.DrawLine (encapsulatedBounds.min, encapsulatedBounds.max,Color.red,2f,false);
			Debug.DrawLine (encapsulatedBounds.min, encapsulatedBounds.max,Color.red,2f,false);
			Debug.DrawLine (encapsulatedBounds.min, encapsulatedBounds.max,Color.red,2f,false);*/
		}
		lengthX = scaleX*(encapsulatedBounds.max.x - encapsulatedBounds.min.x);
		//lengthX = scaleX*(maxX - minX);
		lengthZ = scaleZ*(encapsulatedBounds.max.z - encapsulatedBounds.min.z);
		//lengthZ = scaleZ*(maxZ - minZ);
		startY = centerY;
		startX = encapsulatedBounds.min.x;
		//startX = centerX - (lengthX / 2);
		float endX = centerX + (lengthX / 2);
		startZ = centerZ - (lengthZ / 2);
		float endZ = centerZ + (lengthZ / 2);
		yStep = 20*scaleY;
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
			Debug.LogError("Turning River on");
			StartCoroutine(StartDropping ());
		}
	}
	
	public void Handledischarge()
	{
		if (discharge == null)
		{
			return;
		}
		frequency = discharge.normalizedValue;
	}
	
	private IEnumerator StartDropping()
	{	
		frequency = discharge.normalizedValue;
		// Only run River when the on switch is...on
		while (on.isOn) {
			// Invert the slider so that a frequency of 1 causes a very small amount of time to pass between dropping
			float delay = (1 - Mathf.Max (0.0001f, frequency)) * (maxTimeBetweenDrops);
			//Debug.LogError ("Waiting " + delay + " seconds for the next drop");
			root.Dispense ();
			yield return new WaitForSeconds (delay);
		}
		Debug.LogError ("Turning River off.");
	}
	
	private IEnumerator PopulateDrops()
	{
		RiverDrop dropScript;
		while (numDrops < maxNumDrops) {
			for (int i = 0; i < numDropsPerCreationCycle; i++)
			{
				dropPointer = Instantiate (riverDropPrefab,origin,identity) as GameObject;
				if (dropPointer == null)
				{
					Debug.LogError ("Drop instantiation failed!");
					// Exit the loop early
					yield break;
				}
				dropPointer.transform.parent = this.transform;
				dropScript = dropPointer.GetComponent<RiverDrop>();
				if (dropScript == null)
				{
					Debug.LogError ("Invalid Drop Prefab in RiverGridController");
					// Exit the loop early
					yield break;
				}
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
		if (numDropsAvailable > minNumDropsAvailable)
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
		drop.GetComponent<RiverDrop> ().Enable ();
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
