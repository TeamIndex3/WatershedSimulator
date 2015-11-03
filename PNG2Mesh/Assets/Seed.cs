using UnityEngine;
using System.Collections;

public class Seed {

	private int current;
	private int prev;
	private int maxValue = (int)Mathf.Floor(System.Int32.MaxValue/2);
	private int percentGranularity = 10000;
	private int temp;

	public Seed()
	{
		// Pick some random-ass prime numbers to start
		current = 11;
		prev = 17;
		while (current < 111) {
			IncrementSeed ();
		}
	}

	public Seed(int initialSeed)
	{
		current = initialSeed;
		prev = (int)Mathf.Floor (initialSeed / 2);
		while (current < 111) {
			IncrementSeed ();
		}
	}

	public int GetValueAsInt(int min, int max)
	{
		IncrementSeed ();
		return (int)Mathf.Floor (min + (current % (max - min)));
	}

	public float GetValueAsFloat(float min, float max)
	{
		IncrementSeed ();
		return min + (current % (max - min));
	}

	public float GetValueAsPercent()
	{
		IncrementSeed ();
		return (current % percentGranularity) / percentGranularity;
	}

	private void IncrementSeed()
	{
		temp = prev;
		prev = current;
		current = (temp + prev) % maxValue;
	}

	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
	
	}
}
