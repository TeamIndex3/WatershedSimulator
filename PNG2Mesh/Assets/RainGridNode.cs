using UnityEngine;
using System.Collections;

public class RainGridNode : ScriptableObject {

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

	public void CreateTree(RainGridNode parent, int numRows, int numCols)
	{
		this.parent = parent;
		numChildren = numRows;
		RainGridNode temp = null;
		children = new RainGridNode[numChildren];
		for (int i = 0; i < numChildren; i++){
			temp = (RainGridNode)ScriptableObject.CreateInstance ("RainGridNode");
			//temp.Init();
			temp.CreateTree(this,numCols,0);
			children[i] = temp;
		}
	}

	public void Init(Vector3 loc)
	{
		location = loc;
	}

	private int GetNextChild()
	{
		return 0;
	}

	public void NextChild()
	{
		if (numChildren == 0) {
			DropRain ();
		} else {
			children[GetNextChild()].NextChild();
		}
	}

	private void DropRain()
	{
		// Take a drop from the pool, 
		// give it this location, 
		// give it a yield return new waitforseconds that is random
		// reduce the availabledrop counter;
		// Then enable the drop.
		Debug.LogError ("Dropping drop at location " + location[0] + "," + location[1] + "," + location[2]);
	}
}
