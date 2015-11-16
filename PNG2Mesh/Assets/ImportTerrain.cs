using UnityEngine;
using System.Collections;

public class ImportTerrain : MonoBehaviour {

	public GameObject emptyPrefabWithMeshRenderer;
	public GameObject surface;
	public string filepath;
	public Material mat;
		
	// Use this for initialization
	void Start () {
		Mesh importedMesh = GetComponent<ObjImporter>().ImportFile(filepath);
		//surface = Instantiate(emptyPrefabWithMeshRenderer,transform.position,transform.rotation);
		//surface.GetComponent(MeshFilter).mesh = importedMesh;
		surface.AddComponent<MeshFilter>();
		surface.GetComponent<MeshFilter>().mesh = importedMesh;
		surface.AddComponent<MeshRenderer>().material = mat;
	}
		
	// Update is called once per frame
	void Update () {
			
	}
}