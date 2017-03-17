﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ObjectPlacer : MonoBehaviour {

	public CountryLocation reader;
	public GameObject prefab;

	// Use this for initialization
	void Start () {
		Debug.Log ("LOOK WE ARE DOING THINGS");
		reader.NotANormalWord ();
		Debug.Log ("Size of our data: " + reader.countries.Count);
		foreach (KeyValuePair<string, List<float>> entry in reader.countries)
		{
			Debug.Log (entry.Key + ", lat: " + entry.Value[0] + ", long: " + entry.Value[1]);
			GameObject.Instantiate(prefab, Vector3.zero, Quaternion.Euler(new Vector3(0, entry.Value[1], entry.Value[0])));
		}
		Debug.Log ("THINGS HAVE BEEN DONE");
	}
	
	// Update is called once per frame
	void Update () {
		
	}
}
