using UnityEngine;
using System.Collections;
using UnityStandardAssets.Characters.FirstPerson;
using UnityEngine.UI;

public class UIController : MonoBehaviour {


	public GameObject gui;
	public GameObject controller;
	public GameObject confirmationPanel;
	public GameObject surface0;
	public GameObject surface1;
	public GameObject surface2;
	public GameObject surface3;
	public GameObject surface4;
	public GameObject surface5;
	private bool showUI = false;
	private bool allowFPS = false;
	private bool exitConf = false;
	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
		// Allow escape key to toggle GUI rendering and interaction
		if (Input.GetKeyDown (KeyCode.Escape)) {
			//Debug.LogError ("Open GUI!");
			showUI = !showUI;
			gui.gameObject.SetActive (showUI);
		}
	}

	public void ToggleFPSView()
	{
		allowFPS = !allowFPS;
		//Debug.LogError (controller.name + ": " + allowFPS);
		controller.SetActive (allowFPS);
		RiverGridController gridController = GameObject.FindObjectOfType<RiverGridController> ();
		if (gridController != null) {
			controller.transform.position = gridController.GetSurfaceTopCenter();
		}
		var label = GameObject.Find("FPS Camera Button");
		var canvas = GameObject.FindObjectOfType<Canvas> ();
		if (label != null) {
			var labelText = label.GetComponentInChildren<Text>();
			if (labelText != null)
			{
				if (labelText.text=="Enter the Matrix...")
				{
					labelText.text = "EXIT THE MATRIX!";
					canvas.worldCamera = controller.GetComponent<Camera>();
				}
				else{
					canvas.worldCamera = GameObject.Find("Main Camera").GetComponent<Camera>();
					labelText.text = "Enter the Matrix...";
				}
			}
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

	public void Pause()
	{
		Time.timeScale = 0.0f;
	}

	public void Play()
	{
		Time.timeScale = 1.0f;
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

	public void ExitConfirmation()
	{
		exitConf = !exitConf;

		if (confirmationPanel != null)
		{
			confirmationPanel.SetActive(exitConf);
		}
	}

	public void LoadSurface0()
	{
		RiverGridController grid = GameObject.Find ("RiverGrid").GetComponent<RiverGridController> ();
		if (grid == null) {
			Debug.LogError ("River grid controller is uninitialized");
			return;
		}
		GameObject oldSurface = grid.surface;
		oldSurface.SetActive (false);
		surface0.SetActive (true);
		grid.surface = surface0;
		grid.GetDimensions ();
		grid.UpdateRiverTree ();
	}
	public void LoadSurface1()
	{
		RiverGridController grid = GameObject.Find ("RiverGrid").GetComponent<RiverGridController> ();
		if (grid == null) {
			Debug.LogError ("River grid controller is uninitialized");
			return;
		}
		GameObject oldSurface = grid.surface;
		oldSurface.SetActive (false);
		surface1.SetActive (true);
		grid.surface = surface1;
		grid.GetDimensions ();
		grid.UpdateRiverTree ();
	}
	public void LoadSurface2()
	{
		RiverGridController grid = GameObject.Find ("RiverGrid").GetComponent<RiverGridController> ();
		if (grid == null) {
			Debug.LogError ("River grid controller is uninitialized");
			return;
		}
		GameObject oldSurface = grid.surface;
		oldSurface.SetActive (false);
		surface2.SetActive (true);
		grid.surface = surface2;
		grid.GetDimensions ();
		grid.UpdateRiverTree ();
	}
	public void LoadSurface3()
	{
		RiverGridController grid = GameObject.Find ("RiverGrid").GetComponent<RiverGridController> ();
		if (grid == null) {
			Debug.LogError ("River grid controller is uninitialized");
			return;
		}
		GameObject oldSurface = grid.surface;
		oldSurface.SetActive (false);
		surface3.SetActive (true);
		grid.surface = surface3;
		grid.GetDimensions ();
		grid.UpdateRiverTree ();
	}
	public void LoadSurface4()
	{
		RiverGridController grid = GameObject.Find ("RiverGrid").GetComponent<RiverGridController> ();
		if (grid == null) {
			Debug.LogError ("River grid controller is uninitialized");
			return;
		}
		GameObject oldSurface = grid.surface;
		oldSurface.SetActive (false);
		surface4.SetActive (true);
		grid.surface = surface4;
		grid.GetDimensions ();
		grid.UpdateRiverTree ();
	}
	public void LoadSurface5()
	{
		RiverGridController grid = GameObject.Find ("RiverGrid").GetComponent<RiverGridController> ();
		if (grid == null) {
			Debug.LogError ("River grid controller is uninitialized");
			return;
		}
		GameObject oldSurface = grid.surface;
		oldSurface.SetActive (false);
		surface5.SetActive (true);
		grid.surface = surface5;
		grid.GetDimensions ();
		grid.UpdateRiverTree ();
	}

	public void Exit()
	{
		Application.Quit ();
	}
}
