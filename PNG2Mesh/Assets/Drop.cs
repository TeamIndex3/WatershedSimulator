using UnityEngine;
using System.Collections;

public class Drop : MonoBehaviour {
	
	public Rigidbody body;
	public bool colliding;
	public MeshRenderer r;
	public GameObject grid;
	public float maxMass;
	public Vector3 velocityVector;
	public float maxVelocity = 19.6f;

	// Use this for initialization
	void Start () {
		this.colliding = false;
		//this.body = GetComponent<Rigidbody>();
		//this.r = GetComponent<MeshRenderer>();
		//grid = GetComponent<RainGridController> ();
		/*if (grid == null) {
			Debug.LogError ("NO GRID!");
		}*/
	}

	void Awake()
	{
		grid = GameObject.Find ("RainGrid");
	}



	public void Enable()
	{
		// Turn all physics and rendering on for this drop.
		/*this.body.isKinematic = false;
		this.body.detectCollisions = true;
		this.body.useGravity = true;
		this.r.GetComponentInParent<Renderer>().enabled = true;*/
		// Instead, try 
		this.gameObject.SetActive(true);
	}

	void LateUpdate()
	{
		bool doUpdate = false;
		if (Mathf.Abs (body.velocity [0]) > maxVelocity) {
			velocityVector = new Vector3 (maxVelocity, body.velocity[1], body.velocity[2]);
			doUpdate = true;
		}
		if (body.velocity [1] > maxVelocity/2) {
			velocityVector = new Vector3 (body.velocity [0],maxVelocity/3, body.velocity[2]);
			doUpdate = true;
		}
		if (Mathf.Abs (body.velocity [2]) > maxVelocity) {
			velocityVector = new Vector3 (body.velocity [0], body.velocity[1], maxVelocity);
			doUpdate = true;
		}
		if (doUpdate) {
			body.velocity = velocityVector;
		}
	}

	void Update()
	{
		
		//velocityVector = new Vector3 (body.velocity [0], 0, body.velocity [2]);
		//body.velocity = velocityVector;
	}

	public void Disable()
	{
		// Turn off all physics and rendering for this drop and add it to the available drop queue.
		/*this.body.isKinematic = true;
		this.body.detectCollisions = false;
		this.body.useGravity = false;
		this.r.GetComponentInParent<Renderer> ().enabled = false;*/
		// Instead, try 
		this.gameObject.SetActive(false);
		if (grid == null) {
			Debug.LogError ("Null grid, try it again!");
			grid = GameObject.Find("RainGrid");
		}
		var controller = grid.GetComponent<RainGridController> ();
		controller.AddToQueue (this.gameObject);
	}

	void OnCollisionEnter(Collision collision)
	{
		// Multiple collisions can occur at once - make sure this is an atomic operation!
		if (this.colliding == true) {
			return;
		}
		// Don't allow any other collisions to be processed on this object while we are working on it
		this.colliding = true;

		// Most likely it is touching a surface, check that first
		if (collision.collider is MeshCollider)
		{
			if (collision.gameObject.name == "BasePlane") 
			{
				// This drop has fallen off of the terrain, 
				// Time to remove it from the scene and re-queue it as a new rain drop.

				//Debug.LogError ("Drop has fallen off terrain!");
				Disable ();
			}
			else
			{
				// It is just touching a normal surface - continue as usual and resume collision detection.
				this.colliding = false;
				return;
			}
		}
		// Check to see if instead, we are touching another drop.
		else if (collision.collider is SphereCollider) {
			var thing = collision.gameObject.GetComponent<Drop>();
			if (thing.colliding == true)
			{
				this.colliding = false;
				return;
			}
			float m = collision.rigidbody.mass;
			// Break ties first - we don't want to let both objects grow and not get deleted.
			if (m == this.body.mass)
			{
				// This is the easiest way to break ties since every clone of a prefab has a unique name
				if (string.Compare (collision.gameObject.name,this.gameObject.name) >= 0)
				{
					// Make sure the resulting drop isn't "too big"
					if (m + this.body.mass <= maxMass)
					{
						//Debug.LogError ("Drop Grew!");
						this.body.mass += m;
						this.gameObject.transform.localScale += collision.transform.localScale;
					}
					/*else
					{
						Debug.LogError ("This drop would be too big!");
					}*/
				}
				else if (m + this.body.mass <= maxMass)
				{
					Disable();
				}
			}
			// If this drop is bigger than the one it is touching, absorb it.
			else if (m < this.body.mass)
			{
				// Make sure the resulting drop isn't "too big"
				if (m + this.body.mass <= maxMass)
				{
					//Debug.LogError ("Drop Grew!");
					this.body.mass += m;
					this.gameObject.transform.localScale += collision.transform.localScale;
				}
				/*else
				{
					Debug.LogError ("This drop would be too big!");
				}*/
			}
			else if (m + this.body.mass <= maxMass)
			{
				// The other sphere is bigger, just turn this drop off.

				//Debug.LogError("Drop disappeared!");
				Disable ();
			}
			
			//velocityVector = new Vector3 (body.velocity [0], 0, body.velocity [2]);
			//velocityVector = new Vector3 (0, 0,0);
			//body.velocity = velocityVector;
		}
		// Free up this object for the next collision

		//body.velocity [1] = 0.0;
		this.colliding = false;
	}
}
