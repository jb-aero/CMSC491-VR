using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ObjectPlacer : MonoBehaviour {

	public CountryLocation reader;
	public GameObject prefab;
	
	private Dictionary<string, GameObject> markers;

	// Use this for initialization
	void Start () {
		Debug.Log ("LOOK WE ARE DOING THINGS");
		reader.NotANormalWord ();
        reader.whyWontYouWork();

        Debug.Log ("Size of our data: " + reader.countries.Count);
		foreach (KeyValuePair<string, List<float>> entry in reader.countries)
		{
            int numItems = entry.Value.Count;
            if(numItems == 1)
            {
                Debug.Log(entry.Key + ", num items: " + entry.Value.Count);
            }
			Debug.Log (entry.Key + ", lat: " + entry.Value[1] + ", long: " + entry.Value[2]);
			GameObject marker = GameObject.Instantiate(prefab, Vector3.zero, Quaternion.Euler(new Vector3(0, -entry.Value[2], entry.Value[1])));
            
            marker.name = entry.Key;
            
			markers.Add(entry.Key, marker);
            
        }
		Debug.Log ("THINGS HAVE BEEN DONE");
	}
	
	// Update is called once per frame
	void Update () {
		
	}
}
