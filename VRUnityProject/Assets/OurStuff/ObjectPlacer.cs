using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ObjectPlacer : MonoBehaviour {

	public CountryLocation reader;
	public GameObject prefab;
    public GameObject lightPrefab;
    public GameObject cubePrefab;
	
	private Dictionary<string, GameObject> markers;
    private Dictionary<string, GameObject> lightMarkers;

	// Use this for initialization
	void Start () {
        //This is the variable that controls which view we are looking at
        bool rectangles = true;
        markers = new Dictionary<string, GameObject>();

        Debug.Log ("LOOK WE ARE DOING THINGS");
		reader.NotANormalWord ();
        reader.dictionaryInception();

        Debug.Log ("Size of our data: " + reader.countries.Count);
        if(rectangles == false)
        {
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
    				Light countryLight = lightMarker.GetComponentInChildren(typeof(Light)) as Light;
                    countryLight.intensity = 2F;//amountLight * 8.0f;
                    Color prettyColor = new Color(1F,1F-(amountLight/100.0F),0F);
                    countryLight.color = prettyColor;//.Lerp(255, (255 * amountLight), 255);
                    countryLight.range = 1000F * (radDeg/360F);
                    lightMarker.name = countryName;
                }
               
                
            }
        }
        else{
            /*This is for implementing the bar chart ones*/
            foreach(KeyValuePair<string, Dictionary<string, List<float>>> entry in reader.countriesPollution)
            {
                string countryName = entry.Key;
                
                int numTrees = (int)(entry.Value["1990"][0]);
                float latitude = reader.countries[countryName][1];
                float longitude = reader.countries[countryName][2];

                /*So the idea here is to create a Legend for the cubes to compare to*/
                /*We probably want this to have markings of some kind, or else change colors every 1 or 2 marks*/

                GameObject markerCube = GameObject.Instantiate(cubePrefab, Vector3.zero, Quaternion.Euler(new Vector3(0, -longitude-1F, latitude+1F)));

                markerCube.name = countryName+"Legend";
                Transform tOChild = markerCube.transform.GetChild(0); 
                tOChild.localScale += new Vector3(10F,0.25F,0.25F);
                //Debug.Log(countryName+":"+(float)(entry.Value["1990"][0])/10+" "+(float)(entry.Value["1990"][1])*100F+" "+(float)(entry.Value["1990"][2])*100F);
                /*Now below we create three bars, one for each value scaled to be between 0 and 10*/
                float amountLight = (float)(entry.Value["1990"][1])*100.0F;
                //Debug.Log(amountLight);
                float amountTrees = (float)(entry.Value["1990"][0])/10F;
                float amountPoll = (float)(entry.Value["1990"][2])*100F;

                if((float)(entry.Value["1990"][1]) != -1F)
                {

                    GameObject lightCube = GameObject.Instantiate(cubePrefab, Vector3.zero, Quaternion.Euler(new Vector3(0, -longitude+1F, latitude+1F)));
                    lightCube.name = countryName+"Electricity";
                    Transform tChild = lightCube.transform.GetChild(0); 
                    tChild.localScale += new Vector3(amountLight,0.25F,0.25F);
                    Renderer rend = lightCube.GetComponentInChildren<Renderer>();
                    //Renderer rend = lightCube.GetComponent<Renderer>();
                    rend.material.color = new Color(1.0F,1.0F,0F);
                }

                if((float)(entry.Value["1990"][0]) != -1F)
                {
                    GameObject treeCube = GameObject.Instantiate(cubePrefab, Vector3.zero, Quaternion.Euler(new Vector3(0, -longitude-1F, latitude-1F)));
                    treeCube.name = countryName+"Trees";
                    Transform tChild = treeCube.transform.GetChild(0); 
                    tChild.localScale += new Vector3(amountTrees,0.25F,0.25F);
                    Renderer rend = treeCube.GetComponentInChildren<Renderer>();
                    //Renderer rend = treeCube.GetComponent<Renderer>();
                    rend.material.color = new Color(0F,1.0F,0F);
                }

                if((float)(entry.Value["1990"][2])!= -1F)
                {
                    GameObject pollCube = GameObject.Instantiate(cubePrefab, Vector3.zero, Quaternion.Euler(new Vector3(0, -longitude+1F, latitude-1F)));
                    pollCube.name = countryName+"CO2";
                    Transform tChild = pollCube.transform.GetChild(0); 
                    tChild.localScale += new Vector3(amountPoll,0.25F,0.25F);
                    Renderer rend = pollCube.GetComponentInChildren<Renderer>();
                    //Renderer rend = pollCube.GetComponent<Renderer>();
                    rend.material.color = new Color(1F,0.0F,1F);
                }
                
            }
            //Create four marker rectangles of 1.25, 1.5, 1.75, 2.0 heights
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
