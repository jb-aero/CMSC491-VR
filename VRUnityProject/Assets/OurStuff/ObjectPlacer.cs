using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ObjectPlacer : MonoBehaviour {

	public CountryLocation reader;

	// Use this for initialization
	void Start () {
		Debug.Log ("LOOK WE ARE DOING THINGS");
		reader.NotANormalWord ();
		Debug.Log ("Size of our data: " + reader.countries.Count);
		foreach (KeyValuePair<string, List<float>> entry in reader.countries)
		{
            int numItems = entry.Value.Count;
            if(numItems == 1)
            {
                Debug.Log(entry.Key + ", num items: " + entry.Value.Count);
            }
			
		}
		Debug.Log ("THINGS HAVE BEEN DONE");
	}
	
	// Update is called once per frame
	void Update () {
		
	}
}
