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
	public RainGridController controller;
	public int ID;

	// Use this for initialization
	void Start () {
		//this.body = GetComponent<Rigidbody>();
		//this.r = GetComponent<MeshRenderer>();
		//grid = GetComponent<RainGridController> ();
		/*if (grid == null) {
			Debug.LogError ("NO GRID!");
		}*/
	}

	void Awake()
	{
		this.colliding = false;
		grid = GameObject.Find ("RainGrid");
		controller = grid.GetComponent<RainGridController> ();
		ID = 0;
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
		// Make sure we don't have any crazy bounces going on by limiting the rate at which drops can move
		bool doUpdate = false;
		float tempValue;
		velocityVector = body.velocity;
		tempValue = velocityVector.x;
		if (Mathf.Abs (tempValue) > maxVelocity) {
			tempValue = (tempValue > 0) ? maxVelocity:(-1)*maxVelocity;
			velocityVector.x = tempValue;
			doUpdate = true;
		}
		tempValue = velocityVector.y;
		if (tempValue > maxVelocity/2) {
			velocityVector.y = maxVelocity/2;
			doUpdate = true;
		}
		tempValue = velocityVector.z;
		if (Mathf.Abs (tempValue) > maxVelocity) {
			tempValue = (tempValue > 0) ? maxVelocity:(-1)*maxVelocity;
			velocityVector.z = tempValue;
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
		// Check to see if we are touching another drop.
		if (collision.collider is SphereCollider) {
			// We need their drop script
			var thing = collision.gameObject.GetComponent<Drop>();
			// Make sure they aren't busy too
			if (thing.colliding == true)
			{
				this.colliding = false;
				return;
			}
			// Don't process this if we can't combine in the first place
			if (body.mass >= maxMass)
			{
				this.colliding = false;
				return;
			}
			// Only now is it worth allocating the float that represents the other drop's mass, 
			// since we know at least this has a valid mass
			float m = collision.rigidbody.mass;
			// Check if they are valid for combining
			if (m >= maxMass)
			{
				this.colliding = false;
				return;
			}
			// Break ties first - we don't want to let both objects grow and not get deleted.
			if (m == this.body.mass)
			{
				// This is the easiest way to break ties since every clone of a prefab has a unique name
				// 
				if (thing.ID <= this.ID)
				{
					// Make sure the resulting drop isn't "too big"
					if (m + this.body.mass <= maxMass)
					{
						//Debug.LogError ("Drop Grew!");
						this.body.mass += m;
						this.gameObject.transform.localScale += collision.transform.localScale;
					}
					//else
					//{
					//	Debug.LogError ("This drop would be too big!");
					//}
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
				//else
				//{
				//	Debug.LogError ("This drop would be too big!");
				//}
			}
			else if (m + this.body.mass <= maxMass)
			{
				// The other sphere is bigger, just turn this drop off.

				//Debug.LogError("Drop disappeared!");
				Disable ();
			}
		}
		// Free up this object for the next collision
		this.colliding = false;
	}
}
