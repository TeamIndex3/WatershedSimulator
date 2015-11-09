using UnityEngine;
using System.Collections;

public class RainGridNode : ScriptableObject {

	public RainGridController grid;
	public RainGridNode parent;
	public RainGridNode[] children;
	public int numChildren;
	public Vector3 location;

	public RainGridNode()
	{
		this.location = new Vector3 (0, 0, 0);
	}

	public RainGridNode(Vector3 loc)
	{
		this.location = loc;
	}

	// Use this for initialization
	void Start () {
	

	}
	
	// Update is called once per frame
	void Update () {
	
	}

	public void CreateTree(RainGridController grid, RainGridNode parent, int numRows, int numCols)
	{
		this.grid = grid;
		this.parent = parent;
		numChildren = numRows;
		RainGridNode temp = null;
		children = new RainGridNode[numChildren];
		float tempZFloat = grid.startZ;
		float tempXFloat = grid.startX;
		Vector3 tempLocation;
		for (int i = 0; i < numChildren; i++){
			temp = (RainGridNode)ScriptableObject.CreateInstance ("RainGridNode");
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
				Debug.LogError ("Impossible case, this RainGridNode has no parent pointer AND no parent.parent pointer");
			}
			//temp.Init (location);
			temp.CreateTree(grid,this,numCols,0);
			children[i] = temp;
		}
	}

	public void Dispense()
	{
		if (numChildren == 0) {
			DropRain ();
		} else {
			children[GetNextChild()].Dispense();
		}
	}

	public void Init(Vector3 loc)
	{
		this.location = loc;
	}

	private int GetNextChild()
	{
		return grid.seed.GetValueAsInt(0,numChildren);
	}

	private void DropRain()
	{
		// Take a drop from the pool, 
		// give it this location, 
		// give it a yield return new waitforseconds that is random
		// reduce the availabledrop counter;
		// Then enable the drop.
		float percent, tempx, tempz;
		if (grid.numDropsAvailable > 0) {
			percent = grid.seed.GetValueAsPercent();
			tempx = (location[0] - grid.xStep/2) + (grid.xStep * percent);
			percent = grid.seed.GetValueAsPercent();
			tempz = (location[2] - grid.zStep/2) + (grid.zStep * percent);
			grid.RemoveDropFromQueue(new Vector3(tempx,location[1],tempz));
			//Debug.LogError ("Dropping drop at location " + tempx + "," + location[1] + "," + tempz);
		}
	}
}
