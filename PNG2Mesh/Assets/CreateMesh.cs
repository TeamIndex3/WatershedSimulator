using UnityEngine;
using System.Collections;

public class CreateMesh : MonoBehaviour {
	
	public Material mat;
	public string meshName;
	public UnityEngine.Vector3 color;
	public Vector3[] vertices;
	public int[] triangles;
	public int numVertices;
	public int numTriangles;

	public Mesh MeshSetup(string fileName)
	{
		// Begin parsing TIN file
		string line;
		// Read the file and display it line by line.
		System.IO.StreamReader file = new System.IO.StreamReader(fileName);
		
		line = file.ReadLine();
		if (line == null)
		{
			file.Close();
			return null;
		}
		if (line != "TIN")
		{
			file.Close();
			return null;
		}
		line = file.ReadLine();
		if (line == null)
		{
			file.Close();
			return null;
		}
		if (line != "BEGT")
		{
			file.Close();
			return null;
		}
		line = file.ReadLine();
		if (line == null)
		{
			file.Close();
			return null;
		}
		if (!line.Contains("TNAM"))
		{
			file.Close();
			return null;
		}
		int length = line.Length - 5;
		meshName = line.Substring(5,length);
		line = file.ReadLine();
		if (line == null)
		{
			file.Close();
			return null;
		}
		if (!line.Contains("TCOL"))
		{
			file.Close();
			return null;
		}
		length = line.Length - 5;
		line = line.Substring(5, length);
		string[] tempStringArray = line.Split(' ');
		int tempInt;
		for (int i = 0; i < tempStringArray.Length; i++) {
			int.TryParse(tempStringArray[i], out tempInt);
			color[i] = tempInt;
		}
		line = file.ReadLine();
		if (line == null)
		{
			file.Close();
			return null;
		}
		if (!line.Contains("VERT"))
		{
			file.Close();
			return null;
		}
		
		float tempx, tempy, tempz;
		length = line.Length - 5;
		line = line.Substring(5,length);
		int.TryParse(line, out tempInt);
		numVertices = tempInt;
		vertices = new Vector3[numVertices];
		Vector3 tempVector;
		
		for (int i = 0; i < numVertices; i++) {
			line = file.ReadLine();
			if (line == null)
			{
				file.Close();
				return null;
			}
			tempStringArray = line.Split (' ');
			float.TryParse(tempStringArray[0],out tempx);
			float.TryParse(tempStringArray[1],out tempy);
			float.TryParse(tempStringArray[2],out tempz);
			tempVector = new Vector3(tempx,tempy,tempz);
			vertices[i] = tempVector;
		}
		line = file.ReadLine();
		if (line == null)
		{
			file.Close();
			return null;
		}
		if (!line.Contains("TRI"))
		{
			file.Close();
			return null;
		}
		length = line.Length - 4;
		line = line.Substring(4,length);
		int.TryParse(line, out tempInt);
		numTriangles = tempInt;
		int tempXInt, tempYInt, tempZInt;
		triangles = new int[numTriangles*3];
		for (int i = 0; i < numTriangles; i++) {
			line = file.ReadLine();
			if (line == null)
			{
				file.Close();
				return null;
			}
			tempStringArray = line.Split(' ');
			int.TryParse(tempStringArray[0],out tempXInt);
			int.TryParse(tempStringArray[1],out tempYInt);
			int.TryParse(tempStringArray[2],out tempZInt);
			triangles[i*3] = tempXInt-1;
			triangles[i*3 + 1] = tempYInt-1;
			triangles[i*3 + 2] = tempZInt-1;
		}
		// Done parsing TIN file
		file.Close(); 

		// Create the mesh from the data included in TIN file.
		// Create empty Mesh
		Mesh mesh = new Mesh();
		// Set vertices and triangles
		mesh.vertices = vertices;
		mesh.triangles = triangles;
		// Recalculate normals so the renderer makes things pretty
		mesh.RecalculateNormals();
		// Apply UV texture mapping 
		//Vector2[] UV = new Vector2[] {new Vector2(0,256),new Vector2(256,256),new Vector2(256,0),new Vector2(0,0)};
		//mesh.uv = UV;
		return mesh;
	}

	void Start() {
		Mesh stuff = MeshSetup("example.tin");
		gameObject.AddComponent<MeshFilter>().mesh = stuff;
		gameObject.AddComponent<MeshRenderer>();
		gameObject.AddComponent<Rigidbody>();
		gameObject.GetComponent<Rigidbody>().isKinematic = true;
		gameObject.GetComponent<Rigidbody>().useGravity = false;
		gameObject.GetComponent<Renderer>().material = mat;
		gameObject.AddComponent<MeshCollider>();
		gameObject.GetComponent<MeshCollider>().convex = true;
	}
}
