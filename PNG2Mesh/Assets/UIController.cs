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

	public void LoadRainScene()
	{
		Application.LoadLevel ("RainScene");
	}

	public void LoadRiverScene()
	{
		Application.LoadLevel ("RiverScene");
	}

	public void LoadMainScene()
	{
		Application.LoadLevel ("BaseScene");
	}

	public void Exit()
	{
		Application.Quit ();
	}
}
