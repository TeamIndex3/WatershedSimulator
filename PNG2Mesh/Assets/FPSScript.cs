/*
 * Author: Dave Hampson
 * This code is posted on the Unity community Wiki available at:
 * http://wiki.unity3d.com/wiki/index.php?title=FramesPerSecond
*/ 
using UnityEngine;
using System.Collections;

public class FPSScript : MonoBehaviour
{
	float deltaTime = 0.0f;
	
	void Update()
	{
		deltaTime += (Time.deltaTime - deltaTime) * 0.1f;
	}
	
	void OnGUI()
	{
		int w = Screen.width, h = Screen.height;
		
		GUIStyle style = new GUIStyle();
		
		Rect rect = new Rect(0, 0, w, h * 2 / 100);
		style.alignment = TextAnchor.UpperCenter;
		style.fontSize = h * 2 / 100;
		float msec = deltaTime * 1000.0f;
		float fps = 1.0f / deltaTime;
		if (fps >= 60.0f) {
			style.normal.textColor = new Color (0.0f, 1.0f, 0.0f, 1.0f);
		} else if (fps >= 20) {
			style.normal.textColor = new Color (1.0f, 1.0f, 0.0f, 1.0f);
		} else {
			style.normal.textColor = new Color (1.0f, 0.0f, 0.0f, 1.0f);
		}
		string text = string.Format("{0:0.0} ms ({1:0.} fps)", msec, fps);
		GUI.Label(rect, text, style);
	}
}