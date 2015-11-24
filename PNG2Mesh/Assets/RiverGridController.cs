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
	public GameObject heightSlider;
	public GameObject XSlider;
	public GameObject ZSlider;
	public GameObject ZScaleSlider;
	public GameObject velocitySlider;
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
	public int heightScalar;
	public float velocityScalar;
	public Vector3 InitialVelocity;
	public Vector3 DropConstantForce;
	public Vector3 origin;
	
	// Private data members - Not available to the Inspector
	private Vector3 location;
	private GameObject[] drops;
	private GameObject currentDrop;
	private float dropCreationDelaySeconds = 1;
	private int numDropsPerCreationCycle = 100;
	private Quaternion identity;
	private GameObject dropPointer;
	private Toggle on;
	private Slider discharge;
	private Slider height;
	private Slider xSliderValue;
	private Slider zSliderValue;
	private Slider zScaleValue;
	private Slider velocitySliderValue;
	private Bounds encapsulatedBounds;
	private IEnumerator dropping;
	
	// Every MonoBehavior has a Start function which is called upon instantiation into the Scene / Hierarchy
	void Start () {
		// Set class specific constants
		origin = new Vector3 (0, 0, 0);
		identity = Quaternion.identity;
		InitialVelocity = origin;
		
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
		dropping = StartDropping ();
		StartCoroutine (dropping);
		// Implied: Gravity and constant force is included in the drop prefab - River will begin with the instantion of this script.
	}
	
	void Awake()
	{
		on = onSwitch.GetComponent<Toggle> ();
		discharge = DischargeSlider.GetComponent<Slider> ();
		height = heightSlider.GetComponent<Slider> ();
		xSliderValue = XSlider.GetComponent<Slider> ();
		zSliderValue = ZSlider.GetComponent<Slider> ();
		zScaleValue = ZScaleSlider.GetComponent<Slider> ();
		velocitySliderValue = velocitySlider.GetComponent < Slider> ();
		scaleZ = zScaleValue.normalizedValue;
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
		//startY = centerY;
		startY = encapsulatedBounds.min.y + (height.normalizedValue * (encapsulatedBounds.max.y*2));
		startX = encapsulatedBounds.min.x + (xSliderValue.normalizedValue * encapsulatedBounds.max.x);
		//startX = encapsulatedBounds.min.x;
		//startX = centerX - (lengthX / 2);
		float endX = centerX + (lengthX / 2);
		startZ = encapsulatedBounds.min.z + (zSliderValue.normalizedValue * (encapsulatedBounds.max.z - encapsulatedBounds.min.z)) - lengthZ;
		float endZ = centerZ + (lengthZ / 2);
		yStep = heightScalar*scaleY;
		zStep = lengthZ / numZSteps;
		HandleVelocitySlider ();
		Camera mainCamera = GameObject.Find("Main Camera").GetComponent<Camera>();
		if (mainCamera != null) {
			mainCamera.transform.position = new Vector3(encapsulatedBounds.max.x,centerY,encapsulatedBounds.min.z);
			Vector3 target = new Vector3(encapsulatedBounds.min.x,encapsulatedBounds.min.y,encapsulatedBounds.max.z);
			mainCamera.transform.LookAt(target);
		}
	}

	public void UpdateRiverTree()
	{
		bool wasOn = on.isOn;root.Kill ();
		root = null;
		if (wasOn == true) {
			StopCoroutine (dropping);
			on.isOn = false;
		}
		// Create the root node of the showerhead tree
		root = (RiverGridNode)ScriptableObject.CreateInstance("RiverGridNode");
		// Give it a base location Vector3 (the center point of the river grid)
		location = new Vector3 (startX, centerY, startZ);
		//Debug.LogError ("Creating riverGridNodes at center: " + location [0] + "," + location [1] + "," + location [2]);
		root.Init (location);
		// Recursively create the showerhead tree with a pointer to the following objects:
		// This class, a null parent, and the dimensions of the tree.
		root.CreateTree (this, null, numZSteps, numYSteps);
		if (wasOn == true) {
			on.isOn = true;
		}
	}

	public void HandleVelocitySlider()
	{
		if (velocitySliderValue == null) {
			return;
		}
		float tempVelocity = 1.0f-(0.5f - velocitySliderValue.normalizedValue) * velocityScalar;
		InitialVelocity = new Vector3(tempVelocity,0,0);
		DropConstantForce = new Vector3 (tempVelocity / 10.0f, -9.8f, 0);
	}

	public void HandleZSlider()
	{
		if (zSliderValue == null) {
			return;
		}
		//startZ = encapsulatedBounds.min.z + zScaleValue.normalizedValue *(encapsulatedBounds.max.z - lengthZ);
		//startZ = encapsulatedBounds.center.z + lengthZ*zSliderValue.normalizedValue;
		startZ = encapsulatedBounds.min.z + (zSliderValue.normalizedValue * (encapsulatedBounds.max.z - encapsulatedBounds.min.z)) - lengthZ;
		//startZ = encapsulatedBounds.min.X
		UpdateRiverTree ();
	}

	public void HandleZScale()
	{
		if (zScaleValue == null) {
			return;
		}
		scaleZ = zScaleValue.normalizedValue;
		lengthZ = scaleZ*(encapsulatedBounds.max.z - encapsulatedBounds.min.z);
		zStep = lengthZ / numZSteps;
		UpdateRiverTree ();
	}
	public void HandleXSlider()
	{
		if (xSliderValue == null) {
			return;
		}
		startX = encapsulatedBounds.min.x + (xSliderValue.normalizedValue * encapsulatedBounds.max.x);
		UpdateRiverTree ();
	}

	public void HandleHeightSlider()
	{
		if (height == null) {
			return;
		}
		startY = encapsulatedBounds.min.y + (height.normalizedValue * (encapsulatedBounds.max.y*2));

		UpdateRiverTree();
	}
	
	// GUI input handler functions
	public void HandleSwitch()
	{
		if (on == null) {
			return;
		}
		// All we care about is if the switch turns on
		if (on.isOn) {
			//Debug.LogError("Turning River on");
			StopCoroutine (dropping);
			StartCoroutine(dropping);
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
		RiverDrop theDrop = drop.GetComponent<RiverDrop> ();
		if (theDrop == null)
		{
			yield break;
		}
		theDrop.Enable ();
		theDrop.body.velocity = InitialVelocity;
		drop.GetComponent<ConstantForce>().force = DropConstantForce;
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
