using UnityEngine;
using System.Collections;

public class Seed {

	private int current;
	private int prev;
	private int maxValue = (int)Mathf.Floor(System.Int32.MaxValue/2);
	private float percentGranularity = 10000;
	private int temp;
	private bool busy;

	public Seed()
	{
		// Pick some random-ass prime numbers to start
		current = 11;
		prev = 17;
		while (current < 111 && current < maxValue) {
			IncrementSeed ();
		}
	}

	public Seed(int initialSeed)
	{
		busy = true;
		current = initialSeed;
		prev = (int)Mathf.Floor (initialSeed / 2);
		while (current < 111) {
			IncrementSeed ();
		}
		busy = false;
	}

	public int GetValueAsInt(int min, int max)
	{
		IncrementSeed ();
		return (int)Mathf.Floor (min + (current % (max - min)));
		busy = false;
	}

	public float GetValueAsFloat(float min, float max)
	{
		IncrementSeed ();
		return min + (current % (max - min));
	}

	public float GetValueAsPercent()
	{
		IncrementSeed ();
		return (float)(current % percentGranularity) / percentGranularity;
	}

	private void IncrementSeed()
	{
		while (busy);
		busy = true;
		temp = prev;
		prev = current;
		current = (temp + prev) % maxValue;
		busy = false;
	}

	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
	
	}
}
