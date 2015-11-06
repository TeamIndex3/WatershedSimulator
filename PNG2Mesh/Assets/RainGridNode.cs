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
				temp.Init (location);
			}
			else if (this.parent.parent == null)
			{
				//tempZFloat += grid.zStep;
				// Assume this is a direct child of the root. Update z steps
				tempLocation = new Vector3(location[0],location[1],tempZFloat + i*grid.xStep);
				temp.Init (tempLocation);
			}
			else
			{
				//tempXFloat += grid.xStep;
				// Assume this is a direct child of the root. Update z steps
				tempLocation = new Vector3(tempXFloat + i*grid.xStep,location[1],location[2]);
				temp.Init (tempLocation);
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

	public void NextChild()
	{

	}

	private void DropRain()
	{
		// Take a drop from the pool, 
		// give it this location, 
		// give it a yield return new waitforseconds that is random
		// reduce the availabledrop counter;
		// Then enable the drop.
		if (grid.numDropsAvailable > 0) {
			float tempx = (location[0] - grid.xStep) + (grid.xStep * grid.seed.GetValueAsPercent());
			float tempz = (location[2] - grid.zStep) + (grid.zStep * grid.seed.GetValueAsPercent());
			grid.RemoveDropFromQueue(new Vector3(tempx,location[1],tempz));
		}
		Debug.LogError ("Dropping drop at location " + location[0] + "," + location[1] + "," + location[2]);
	}
}
