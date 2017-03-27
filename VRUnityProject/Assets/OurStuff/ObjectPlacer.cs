using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ObjectPlacer : MonoBehaviour {

	public CountryLocation reader;
	public GameObject prefab;
    public GameObject lightPrefab;
	
	private Dictionary<string, GameObject> markers;
    private Dictionary<string, GameObject> lightMarkers;

	// Use this for initialization
	void Start () {
        markers = new Dictionary<string, GameObject>();

        Debug.Log ("LOOK WE ARE DOING THINGS");
		reader.NotANormalWord ();
        reader.dictionaryInception();

        Debug.Log ("Size of our data: " + reader.countries.Count);

        foreach(KeyValuePair<string, Dictionary<string, List<float>>> entry in reader.countriesPollution)
        {
            string countryName = entry.Key;
            
            int numTrees = (int)(entry.Value["1990"][0]);

            //Debug.Log(countryName);
            //Debug.Log(reader.countries[countryName]);
            
            float latitude = reader.countries[countryName][1];
            float longitude = reader.countries[countryName][2];

            float radDeg = reader.countries[countryName][7];
            if(radDeg > 5)
            {
                radDeg = 5;
            }
            if (numTrees != -1)
            {
                for (int i = 0; i < numTrees; i++)
                {
                    float newLat = Random.Range(latitude - radDeg, latitude + radDeg);
                    float newLong = Random.Range(longitude - radDeg, longitude + radDeg);
					GameObject marker = GameObject.Instantiate(prefab, Vector3.zero, Quaternion.Euler(new Vector3(0, -newLong, newLat)));
                    marker.name = countryName + i.ToString();
                }
            }

            float amountLight = (float)(entry.Value["1990"][2]);
            if(amountLight != -1)
            {
               
                GameObject lightMarker = GameObject.Instantiate(lightPrefab, Vector3.zero, Quaternion.Euler(new Vector3(0, -longitude, latitude)));
                Light countryLight = lightMarker.AddComponent<Light>();
                countryLight.intensity = 2F;//amountLight * 8.0f;
                Color prettyColor = new Color(1F,1F-(amountLight/100.0F),0F);
                countryLight.color = prettyColor;//.Lerp(255, (255 * amountLight), 255);
                lightMarker.name = countryName;
            }
           
            
        }

        /*foreach (KeyValuePair<string, List<float>> entry in reader.countries)
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
            
        }*/
        Debug.Log ("THINGS HAVE BEEN DONE");
	}
	
	// Update is called once per frame
	void Update () {
		
	}
}
