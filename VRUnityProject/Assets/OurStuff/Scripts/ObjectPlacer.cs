using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ObjectPlacer : MonoBehaviour {

	public CountryLocation reader;
	public GameObject prefab;
    public GameObject lightPrefab;
    public GameObject cubePrefab;
    public GameObject rulerPrefab;
    public GameObject fogPrefab;
    public GameObject rulerLight;
	public Text yearText;
	public int yearIndex;
    public bool barGraph_view;
    public int varToShow;
	private Dictionary<string, List<GameObject> > markers;//This is to hold the tree markers. Each country maps to a list of trees.
    private int oldYear;
    private int oldVar;
    private List<string> years;

	// Use this for initialization
	void Start () {
        //This is the variable that controls which view we are looking at
        barGraph_view = true;
        varToShow = 1;//0 means trees, 1 is electricity, 2 is CO2
        yearIndex = 0;
        oldYear = 0;
        markers = new Dictionary<string, List<GameObject>>();

        Debug.Log ("LOOK WE ARE DOING THINGS");
		reader.NotANormalWord ();
		reader.dictionaryInception();
		years = reader.headerList;

        Debug.Log ("Size of our data: " + reader.countries.Count);
        if(barGraph_view == false)
        {
            foreach(KeyValuePair<string, Dictionary<string, List<float>>> entry in reader.countriesPollution)
            {
                string countryName = entry.Key; 
                float latitude = reader.countries[countryName][1];
                float longitude = reader.countries[countryName][2];

                float radDeg = reader.countries[countryName][7];

                if(varToShow == 0)//This means we are looking at trees right now
                {

                    int numTrees = (int)(entry.Value[years[yearIndex]][0]);
                    List<GameObject> listOfTrees = new List<GameObject>();
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
                            listOfTrees.Add(marker);
                        }
                        markers.Add(countryName,listOfTrees);
                    }
                }
            }
            reDrawElecAndCO2();
            
        }
        else{
            
            //Create four marker rectangles of 1.25, 1.5, 1.75, 2.0 heights
            DrawRecs();
        }

        Debug.Log ("THINGS HAVE BEEN DONE");
	}
	
	// Update is called once per frame
	void Update () {

		if (Input.GetButtonDown ("LeftGrip"))
		{
			barGraph_view = !barGraph_view;
		}

		if (Input.GetButtonDown ("RightGrip"))
		{
			++varToShow;
		}

		if (Input.GetButtonDown ("RightDButton"))
		{
			++yearIndex;
		}
		else if (Input.GetButtonDown ("LeftDButton"))
		{
			--yearIndex;
		}

		varToShow %= 3;
		yearIndex %= 24;
		yearText.text = years [yearIndex];

        if(yearIndex != oldYear)
        {
    		if(barGraph_view == false)
            {
                if(varToShow == 0)
                {
                    foreach(KeyValuePair<string, Dictionary<string, List<float>>> entry in reader.countriesPollution)
                    {
                        //Get num trees in new year. 
                        int amountTreesOld = (int)(entry.Value[years[oldYear]][0]);
                        int amountTrees = (int)(entry.Value[years[yearIndex]][0]);
                        //If it is less than old, remove some trees from the scene and from the dictionary
                        if(amountTrees < amountTreesOld)
                        {
                            int numToRemove = amountTreesOld - amountTrees;
                            //Get the list of trees from the dictionary
                            List<GameObject> tempList = new List<GameObject>();
                            if (markers.TryGetValue(entry.Key, out tempList))
                            {
                                for(int i = 0; i < numToRemove; i++)
                                {
                                    GameObject aTree = tempList[0];
                                    tempList.RemoveAt(0);
                                    Destroy(aTree);
                                }
                            }
                        }
                        //If it is more, make some new trees and add them to the dictionary and scene
                        if(amountTrees > amountTreesOld)
                        {
                            int numToAdd = amountTrees - amountTreesOld;
                            List<GameObject> tempList = new List<GameObject>();
                            if (markers.TryGetValue(entry.Key, out tempList))
                            {
                                string countryName = entry.Key;
                                float latitude = reader.countries[countryName][1];
                                float longitude = reader.countries[countryName][2];
                                float radDeg = reader.countries[countryName][7];
                                for(int i = 0; i < numToAdd; i++)
                                {
                                    float newLat = Random.Range(latitude - radDeg, latitude + radDeg);
                                    float newLong = Random.Range(longitude - radDeg, longitude + radDeg);
                                    GameObject marker = GameObject.Instantiate(prefab, Vector3.zero, Quaternion.Euler(new Vector3(0, -newLong, newLat)));
                                    marker.name = countryName + (i+amountTreesOld).ToString();
                                    tempList.Add(marker);
                                }
                            }
                        }
                    }
                }
                //Don't need to do anything fancy just redraw them
                reDrawElecAndCO2();
                oldYear = yearIndex;
            }
            else
            {
                oldYear = yearIndex;
                DrawRecs();
            }
            
        }
	}



    void reDrawElecAndCO2()
    {
        foreach(KeyValuePair<string, Dictionary<string, List<float>>> entry in reader.countriesPollution)
            {
                string countryName = entry.Key;
                
                
                
                float latitude = reader.countries[countryName][1];
                float longitude = reader.countries[countryName][2];

                float radDeg = reader.countries[countryName][7];

                if(varToShow == 1)
                {
                    float amountLight = (float)(entry.Value[years[yearIndex]][2]);///reader.maxElectList[0]);
                    if(amountLight > 0F)
                    {
                        List<float> reds = new List<float>{255F,255F,255F,153F,51F,51F,51F,51F,51F,153F};
                        List<float> greens = new List<float>{51F,153F,255F,255F,255F,255F,255F,153F,51F,51F};
                        List<float> blues = new List<float>{51F,51F,51F,51F,51F,153F,255F,255F,255F,255F};
                        int power = 0;
                        while(amountLight >= 10F)
                        {
                            power++;
                            amountLight = amountLight/10F;
                        }

                        Debug.Log (power);

                        GameObject lightMarker = GameObject.Instantiate(lightPrefab, Vector3.zero, Quaternion.Euler(new Vector3(0, -longitude, latitude)));
                        Light countryLight = lightMarker.GetComponentInChildren(typeof(Light)) as Light;
                        
                        Color prettyColor = new Color(reds[power]/255F,greens[power]/255F,blues[power]/255F);
                        countryLight.color = prettyColor;
                        lightMarker.name = countryName+"_light";
                    }
                }

                if(varToShow == 2)
                {
                    float amountCO2 = (float)(entry.Value[years[yearIndex]][1]);
                    if(amountCO2 > 0F)
                    {
                        float power = 0F;
                        while(amountCO2 >= 10F)
                        {
                            power++;
                            amountCO2 = amountCO2/10F;
                        }

                        Debug.Log (power);

                        GameObject fogMarker = GameObject.Instantiate(fogPrefab, Vector3.zero, Quaternion.Euler(new Vector3(0, -longitude, latitude)));
                        Renderer rend = fogMarker.GetComponentInChildren<Renderer>();
                        rend.material.SetFloat("_Pow", (power/10F)*4F);
                        ParticleSystem aPart = fogMarker.GetComponentInChildren<ParticleSystem>();
                        aPart.Play();
                        fogMarker.name = countryName+"_CO2";
                    }
                }
                
            }
    }



    //This function draws the rectangles. It's moved here so it can be called when year changes
    void DrawRecs()
    {
        /*This is for implementing the bar chart ones*/
            foreach(KeyValuePair<string, Dictionary<string, List<float>>> entry in reader.countriesPollution)
            {
                string countryName = entry.Key;
                
                int numTrees = (int)(entry.Value[years[yearIndex]][0]);
                float latitude = reader.countries[countryName][1];
                float longitude = reader.countries[countryName][2];

                /*So the idea here is to create a Legend for the cubes to compare to*/
                /*We probably want this to have markings of some kind, or else change colors every 1 or 2 marks*/

                GameObject markerCube;
                if(varToShow != 1)
                { 
                    markerCube= GameObject.Instantiate(rulerPrefab, Vector3.zero, Quaternion.Euler(new Vector3(0, -longitude-1F, latitude+1F)));
                }
                else
                {
                    markerCube= GameObject.Instantiate(rulerLight, Vector3.zero, Quaternion.Euler(new Vector3(0, -longitude-1F, latitude+1F)));
                }
                markerCube.name = countryName+"Legend";
                Transform tOChild = markerCube.transform.GetChild(0); 
                //tOChild.localScale += new Vector3(10F,0.1F,0.1F);
                //Debug.Log(countryName+":"+(float)(entry.Value[years[yearIndex]][0])/10+" "+(float)(entry.Value["1990"][1])*10F+" "+(float)(entry.Value["1990"][2])*10F);
                /*Now below we create three bars, one for each value scaled to be between 0 and 10*/
                float amountLight = (float)(entry.Value[years[yearIndex]][1]);
                //Debug.Log(amountLight);
                float amountTrees = (float)(entry.Value[years[yearIndex]][0])/10F;
                float amountPoll = (float)(entry.Value[years[yearIndex]][2]);

                if(varToShow == 1)
                {
                    if((float)(entry.Value[years[yearIndex]][1]) != -1F)
                    {
                        int power = 0;
                        while(amountLight >= 10F)
                        {
                            power++;
                            amountLight = amountLight/10F;
                        }

                        GameObject lightCubePow = GameObject.Instantiate(cubePrefab, Vector3.zero, Quaternion.Euler(new Vector3(0, -longitude+1F, latitude+1F)));
                        lightCubePow.name = countryName+"ElectricityPow";
                        Transform tChild = lightCubePow.transform.GetChild(0); 
                        tChild.localScale += new Vector3(power,0.1F,0.1F);
                        tChild.Translate(power/2F,0F,0F);
                        Renderer rend = lightCubePow.GetComponentInChildren<Renderer>();
                        rend.material.color = new Color(1.0F,0.5F,0F);
                
                    }
                }
                else if (varToShow == 0)
                {
                    if((float)(entry.Value[years[yearIndex]][0]) != -1F)
                    {
                        GameObject treeCube = GameObject.Instantiate(cubePrefab, Vector3.zero, Quaternion.Euler(new Vector3(0, -longitude-1F, latitude)));
                        treeCube.name = countryName+"Trees";
                        Transform tChild = treeCube.transform.GetChild(0); 
                        tChild.localScale += new Vector3(amountTrees,0.1F,0.1F);
                        tChild.Translate(amountTrees/2F,0F,0F);
                        Renderer rend = treeCube.GetComponentInChildren<Renderer>();
                        //Renderer rend = treeCube.GetComponent<Renderer>();
                        rend.material.color = new Color(0F,1.0F,0F);
                    }
                }
                else
                {
                    if((float)(entry.Value[years[yearIndex]][2])!= -1F)
                    {

                        int power = 0;
                        while(amountPoll >= 10F)
                        {
                            power++;
                            amountPoll = amountPoll/10F;
                        }

                        GameObject pollCubePow = GameObject.Instantiate(cubePrefab, Vector3.zero, Quaternion.Euler(new Vector3(0, -longitude+1F, latitude)));
                        pollCubePow.name = countryName+"CO2Pow";
                        Transform tChild = pollCubePow.transform.GetChild(0); 
                        tChild.localScale += new Vector3(power,0.1F,0.1F);
                        tChild.Translate(power/2F,0F,0F);
                        Renderer rend = pollCubePow.GetComponentInChildren<Renderer>();
                        rend.material.color = new Color(0F,0.0F,0.5F);

                    }
                }
            }
    }
}
