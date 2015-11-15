using UnityEngine;
using System.Collections;

public class UIController : MonoBehaviour {


	public GameObject gui;
	private bool show = false;
	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
		// Allow escape key to toggle GUI rendering and interaction
		if (Input.GetKeyDown (KeyCode.Escape)) {
			//Debug.LogError ("Open GUI!");
			show = !show;
			gui.gameObject.SetActive (show);
		}
	}

	private void KillAllScenery()
	{
		// Currently depricated, this causes a visual artifact of loading when used.
		// Need to turn off camera until scene fully loads

		// This makes sure that our allocated objects are freed from memory.
		// I'm pretty sure that Unity does this for us with Application.LoadLevel(),
		// However we are dealing with a lot of objects, so I want to be sure it happens.
		var scene = GameObject.Find ("Scene");
		if (scene != null) {
			Destroy (scene);
		}
	}

	public void LoadRainScene()
	{
		//KillAllScenery ();
		Application.LoadLevel ("RainScene");
	}

	public void LoadRiverScene()
	{
		//KillAllScenery ();
		Application.LoadLevel ("RiverScene");
	}

	public void LoadMainScene()
	{
		//KillAllScenery ();
		Application.LoadLevel ("BaseScene");
	}

	public void Exit()
	{
		Application.Quit ();
	}
}
