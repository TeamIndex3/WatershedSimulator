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
		for (int i = 0; i < numChildren; i++){
			temp = (RainGridNode)ScriptableObject.CreateInstance ("RainGridNode");
			temp.Init (location);
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
			grid.RemoveDropFromQueue(location);
		}
		Debug.LogError ("Dropping drop at location " + location[0] + "," + location[1] + "," + location[2]);
	}
}
