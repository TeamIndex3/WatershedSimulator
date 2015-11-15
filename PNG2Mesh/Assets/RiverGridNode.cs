using UnityEngine;
using System.Collections;

public class RiverGridNode : ScriptableObject {
	
	public RiverGridController grid;
	public RiverGridNode parent;
	public RiverGridNode[] children;
	public int numChildren;
	public Vector3 location;
	
	public RiverGridNode()
	{
		this.location = new Vector3 (0, 0, 0);
	}
	
	public RiverGridNode(Vector3 loc)
	{
		this.location = loc;
	}
	
	// Use this for initialization
	void Start () {
		
		
	}
	
	// Update is called once per frame
	void Update () {
		
	}
	
	public void CreateTree(RiverGridController grid, RiverGridNode parent, int numRows, int numCols)
	{
		this.grid = grid;
		this.parent = parent;
		numChildren = numRows;
		RiverGridNode temp = null;
		children = new RiverGridNode[numChildren];
		float tempZFloat = grid.startZ;
		float tempXFloat = grid.startX;
		Vector3 tempLocation;
		for (int i = 0; i < numChildren; i++){
			temp = (RiverGridNode)ScriptableObject.CreateInstance ("RiverGridNode");
			if (this.parent == null)
			{
				//tempZFloat += grid.zStep;
				// Assume this is a direct child of the root. Update z steps
				tempLocation = new Vector3(location[0],location[1],tempZFloat + i*grid.xStep);
				temp.Init (tempLocation);
			}
			else if (this.parent.parent == null)
			{
				//tempXFloat += grid.xStep;
				// Assume this is a direct child of the root. Update z steps
				tempLocation = new Vector3(tempXFloat + i*grid.xStep,location[1],location[2]);
				temp.Init (tempLocation);
			}
			else
			{
				Debug.LogError ("Impossible case, this RiverGridNode has no parent pointer AND no parent.parent pointer");
			}
			//temp.Init (location);
			temp.CreateTree(grid,this,numCols,0);
			children[i] = temp;
		}
	}
	
	public void Dispense()
	{
		// Called on the root node, this recursive finds a random leaf node,
		// tells that node to create a drop at a random (x,y,z) coordinate,
		// after a random delay time.
		if (numChildren == 0) {
			DropRiver ();
		} else {
			children[GetNextChild()].Dispense();
		}
	}
	
	public void Init(Vector3 loc)
	{
		// Initialization method, since constructors are ignored on ScriptableObjects
		this.location = loc;
	}
	
	private int GetNextChild()
	{
		// Return the index of a random child node
		return grid.seed.GetValueAsInt(0,numChildren);
	}
	
	private void DropRiver()
	{
		// Take a drop from the pool, 
		// give it this location, 
		// give it a yield return new waitforseconds that is random
		// reduce the availabledrop counter;
		// Then enable the drop.
		// Ensure there is a drop object available in the queue.
		if (grid.numDropsAvailable > grid.minNumDropsAvailable) {
			// Now it is worth the memory allocation
			float percent, tempx, tempz;
			// Find a random percent to multiply the x offset from this node's location by
			percent = grid.seed.GetValueAsPercent();
			tempx = (location[0] - grid.xStep/2) + (grid.xStep * percent);
			// Do the same thing with the z offset
			percent = grid.seed.GetValueAsPercent();
			tempz = (location[2] - grid.zStep/2) + (grid.zStep * percent);
			// Tell the grid to dequeue a drop at the given x,z location with a constant y
			grid.RemoveDropFromQueue(new Vector3(tempx,location[1],tempz));
			//Debug.LogError ("Dropping drop at location " + tempx + "," + location[1] + "," + tempz);
		}
	}
}
