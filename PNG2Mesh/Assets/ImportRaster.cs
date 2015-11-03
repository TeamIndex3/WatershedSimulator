/*
Watershed Simulator Raster File Parsing
Authors, in alphabetic order: Max Kohl and Garrett Morrison
Contributors in alphabetic order by last name:
        Abdulmajeed Kadi, Hannah Smith, Joshua Yang
*/


// Imports
using UnityEngine;
using System.Collections;

// Everything in C# is a class, and anything that we want to use in Unity should probably drive from MonoBehaviour.
// MonoBehaviour gives us a whole bunch of awesome utility. 
// Feel free to cehck out the MonoBehavior class reference: http://docs.unity3d.com/ScriptReference/MonoBehaviour.html
public class ImportRaster : MonoBehaviour {

	// Declare data members of this class
	public Material mat;
	public string meshName;
	public UnityEngine.Vector3 color;
	public Vector3[] vertices;
	public int[] triangles;
	public int numVertices;
	public int numTriangles;
	private int tempInt;
	private int tempStringArray;
	private System.IO.StreamReader file;
	
	// The start function is called when a gameobject is loaded into the game.
	// This function is called as the game initializes and the objects in the Hierarchy screen are parsed.
	void Start() {
		string fn = "example.png";
		// Set up our file handle and get our data in memory
		ParseFile (fn);
		// Create a surface from the parsed data
		Mesh surface = MeshSetup();
		// Did you remember to null check? I didn't :[
		// By the way, you can return from any function type, even void.
		if (surface == null) {
			return;
		}
		// Add necessary components so that this surface is interactive and interesting.
		// First add a Mesh Filter so this surface is standardized in Unity
		gameObject.AddComponent<MeshFilter> ().mesh = surface;
		// Now we can give it a collider that is uniform with the surface
		gameObject.AddComponent<MeshCollider> ();
		// And a renderer which allows the camera to see it
		gameObject.AddComponent<MeshRenderer> ();
		// Lastly, give it a physics component.
		gameObject.AddComponent<Rigidbody> ();
		
		// Set the properties of these components. 
		// isKinematic == true implies that this RigidBody is not moved by collisions
		gameObject.GetComponent<Rigidbody> ().isKinematic = true;
		// useGravity == false implies that this object will not have a constand force of -9.8 units / second applied to it along the Y axis
		gameObject.GetComponent<Rigidbody> ().useGravity = false;
		// Let the renderer know what material prefab to draw this surface with
		gameObject.GetComponent<Renderer>().material = mat;
	}
	
	// The Awake function is called as a gameObject is added to the Scene. It will only be called once (as far as I remember). 
	// In this case, since this script is attached to the ProceduralSurface prefab, 
	void Awake()
	{
		// No need to do anything here since the Start function handles everything we need.
	}
	
	// This is called every frame for a rendered object
	void Update()
	{
		// No need to do anything in here at this time. 
		// Usually we don't want to touch update functions because they can prevent Unity from optimizing for us.
	}

	public void ParseFile(string fileName)
	{
		// Begin parsing file
		string line;
		// Read the file and display it line by line.
		file = new System.IO.StreamReader (fileName);
		// Read a single line of the file, then perform a null check to prevent a NRE from occurring.
		// For more sweet file stream documentation, check out https://msdn.microsoft.com/en-us/library/system.io.streamreader(v=vs.110).aspx
		line = file.ReadLine ();
		// Null check the line before you mess with it
		if (line == null || !(line is string)) {
			// No touchey touchey!
			file.Close ();
			return;
		}

		// TODO: Do cool stuff here
		
		// Always close a file when you're done with it
		file.Close ();

		
		// Below is example code that I thought you might find useful.
		// It contains a few of the operations that I did with full annotations.
		// The following code comes from CreateMesh.cs, where I read from example.tin 


		/*
		// Uncomment this block for syntax highlighting - not all of the following code will compile, it needs your personal touch.

		// Please observe CreateMesh.cs for the actual implementation this code was stripped from.


		// Example of splitting a string on a character
		string[] tempStringArray = line.Split (' ');

		// Create some local variables to populate the Vector3 of vertices
		float tempx, tempy, tempz;
		// Expecting an integer value here
		int.TryParse (line, out tempInt);
		// Check to see if this is null
		if (tempInt == null) {
			return null;
		}
		// Update a local variable
		numVertices = tempInt;
		// Create an array of Vector3 objects of size numVertices
		vertices = new Vector3[numVertices];
		// Store a temp vector for loop optimization
		Vector3 tempVector;
		// Iterate over vertices
		for (int i = 0; i < numVertices; i++) {
			// Each vertex is delimited by a newline character
			line = file.ReadLine ();
			// Alwyas null check
			if (line == null) {
				file.Close ();
				return null;
			}
			// Get the x,y,z values
			tempStringArray = line.Split (' ');
			// Convert from string to float
			float.TryParse (tempStringArray [0], out tempx);
			float.TryParse (tempStringArray [1], out tempy);
			float.TryParse (tempStringArray [2], out tempz);
			// NULLLLLL CHECKKKKK SONNNNNNN
			if (tempx == null || tempy == null || tempz == null) {
				return null;
			}
			// Create a Vector3 instance with this point's x,y,z values
			tempVector = new Vector3 (tempx, tempy, tempz);
			// Add it to the Vertices array
			vertices [i] = tempVector;
		}
		// Same idea as before, but this time for populating the triangle vertices
		for (int i = 0; i < numTriangles; i++) {
			line = file.ReadLine ();
			if (line == null) {
				file.Close ();
				return null;
			}
			tempStringArray = line.Split (' ');
			int.TryParse (tempStringArray [0], out tempXInt);
			int.TryParse (tempStringArray [1], out tempYInt);
			int.TryParse (tempStringArray [2], out tempZInt);
			triangles [i * 3] = tempXInt - 1;
			triangles [i * 3 + 1] = tempYInt - 1;
			triangles [i * 3 + 2] = tempZInt - 1;
		}
		*/
	}

	public Mesh MeshSetup()
	{
		// Create the mesh from the data included in the file.

		// Start with an empty Mesh
		Mesh mesh = new Mesh();
		// Set vertices and triangles
		mesh.vertices = vertices;
		mesh.triangles = triangles;

		// Apply UV texture mapping 
		// Ideally this should come during the Triangulation algorithm, or afterwards at the added cost of an O(n) triangle traversal
		// We are using a 1d texture (solid color), so it should be possible to set a uv as Mathf.floor(y*256), or something like that.
		// This could ideally happen as soon we discover that a triangle meets the Delaunay criteria and we add the triangle to the triangle array.
		// UV mapping is implemented as an array of Vector2's, each with a color value between 0 and 256.
		// Vector2[] UV = new Vector2[] {new Vector2(0,256),new Vector2(256,256),new Vector2(256,0),new Vector2(0,0)};
		// Then apply the UV mappings to the mesh
		// mesh.uv = UV;

		// Recalculate normals so the renderer makes things pretty
		mesh.RecalculateNormals ();
		return mesh;
	}
}