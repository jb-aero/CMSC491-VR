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
    public Text varText;
	public int yearIndex;
    public bool barGraph_view;
    public int varToShow;

    private List<string> variables = new List<string>{"% Forest Cover","Electricity Usage","CO2 Emissions"};

    //This is to hold the tree markers. Each country maps to a list of trees.
	private Dictionary<string, List<GameObject> > markers;
    //These hold the lights and the fog objects
    private List<GameObject> listOfLights = new List<GameObject>();
    private List<GameObject> listOfFog = new List<GameObject>();
    //These hold the bars when they have been connected
    private List<GameObject> listOfTreeBars = new List<GameObject>();
    private List<GameObject> listOfElecBars = new List<GameObject>();
    private List<GameObject> listOfCO2Bars = new List<GameObject>();
    private List<GameObject> listOfLegendBars = new List<GameObject>();

    //To detect a change
    private int oldYear;
    private int oldVar;
    private bool wasBar;

    private List<string> years;

    //Colors for electricity
    private List<float> elecR =  new List<float>{128F,140F,153F,179F,191F,204F,217F,230F,242F,255F};
    private List<float> elecG =  new List<float>{128F,140F,153F,179F,191F,204F,217F,230F,242F,255F};
    private List<float> elecB =  new List<float>{128F,115F,102F,89F,77F,64F,51F,38F,25F,13F,0F};
    //Colors for CO2 scales
    private List<float> CO2R =  new List<float>{0F,31F,61F,92F,122F,153F,173F,194F,214F,235F,255F};
    private List<float> CO2G =  new List<float>{0F,31F,61F,92F,122F,153F,173F,194F,214F,235F,255F};
    private List<float> CO2B =  new List<float>{0F,20F,41F,61F,82F,102F,133F,163F,194F,224F,255F};

    //Colors for tree Scales
    private List<float> treeR =  new List<float>{0F,0F,0F,0F,0F,26F,77F,128F,179F,230F};
    private List<float> treeG =  new List<float>{26F,77F,128F,179F,230F,255F,255F,255F,255F,255F};
    private List<float> treeB =  new List<float>{0F,0F,0F,0F,0F,26F,77F,128F,179F,230F};
	// Use this for initialization
	void Start () {
        //This is the variable that controls which view we are looking at
        barGraph_view = true;
        varToShow = 1;//0 means trees, 1 is electricity, 2 is CO2
        yearIndex = 0;
        oldYear = 0;
        oldVar = varToShow;
        wasBar = barGraph_view;
        markers = new Dictionary<string, List<GameObject>>();

        Debug.Log ("LOOK WE ARE DOING THINGS");
		reader.NotANormalWord ();
		reader.dictionaryInception();
		years = reader.headerList;

        Debug.Log ("Size of our data: " + reader.countries.Count);
        if(barGraph_view == false)
        {

            if(varToShow == 0)//This means we are looking at trees right now for the first time
            {
                drawTreesFromScratch();
            }
            else
            {
                reDrawElecAndCO2();
            } 
            
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
			Debug.Log ("Hit left grip.");
			barGraph_view = !barGraph_view;
		}

		if (Input.GetButtonDown ("RightGrip"))
		{
			Debug.Log ("Hit right grip.");
			++varToShow;
		}

		if (Input.GetButtonDown ("RightMenuButton"))
		{
			++yearIndex;
		}
		else if (Input.GetButtonDown ("LeftMenuButton"))
		{
			--yearIndex;
		}

        if(yearIndex < 0)
        {
            yearIndex += 24;
        }

		varToShow %= 3;
		yearIndex %= 24;
		yearText.text = years [yearIndex];
        varText.text = variables[varToShow];
        //Check if the view, variable, and year have changed in that order
        if(wasBar != barGraph_view)
        {
            clearHouse();
            if(barGraph_view == true)
            {
                DrawRecs();
            }
            else{
                if(varToShow == 0)
                {
                    drawTreesFromScratch();
                }
                else
                {
                    reDrawElecAndCO2();
                }
            }
            wasBar = barGraph_view;
        }
        else if(oldVar != varToShow && barGraph_view == false)
        {
            if(varToShow == 0)
            {
                //Delete the CO2 and light values, and draw the trees
                clearHouse();
                drawTreesFromScratch();
            }
            else if(oldVar == 0)
            {
                //Remove the trees, then call redraw
                clearHouse();
                reDrawElecAndCO2();
            }
            else{
                clearHouse();
                reDrawElecAndCO2();
            }
            oldVar = varToShow;
        }
        else if(oldVar != varToShow && barGraph_view == true)
        {
            clearHouse();
            DrawRecs();
            oldVar = varToShow;
        }
        else if(yearIndex != oldYear)
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
                else if(varToShow == 1)
                {
                    //Don't need to do anything fancy just redraw them
                    clearHouse();
                    reDrawElecAndCO2();
                    oldYear = yearIndex;
                }
                else{
                    //Update the smog values but don't redraw them
                    //For each item in the list of fog
                    for(int i = 0; i < listOfFog.Count;i++)
                    {
                            //Get the name of the country from the name of the marker
                            GameObject temp = listOfFog[i];
                            string countryName = temp.name.Substring(0,temp.name.Length - 4);
                            //Get the new value for that country depending on the new year
                            float amountCO2 = (float)(reader.countriesPollution[countryName][years[yearIndex]][1]);
                            //Change the variables in the fog marker to reflect that of the new amount
                            int power = 0;
                            while(amountCO2 >= 10F)
                            {
                                power++;
                                amountCO2 = amountCO2/10F;
                            }

                        Renderer rend = temp.GetComponentInChildren<Renderer>();
                        rend.material.SetFloat("_Pow", (power/10F)*4F);
                        ParticleSystem aPart = temp.GetComponentInChildren<ParticleSystem>();
                        
                    }
                    
                    
                    
                    oldYear = yearIndex;
                }
            }
            else
            {
                oldYear = yearIndex;
                clearHouse();
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
                        //List<float> reds = new List<float>{255F,255F,255F,153F,51F,51F,51F,51F,51F,153F};
                        //List<float> greens = new List<float>{51F,153F,255F,255F,255F,255F,255F,153F,51F,51F};
                        //List<float> blues = new List<float>{51F,51F,51F,51F,51F,153F,255F,255F,255F,255F};

                        List<float> reds =  new List<float>{0F,0F,128F,255F,255F,255F,255F};
                        List<float> greens =  new List<float>{255F,255F,255F,255F,128F,64F,0F};
                        List<float> blues =  new List<float>{128F,0F,0F,0F,0F,0F,0F};
                        int power = 0;
                        while(amountLight >= 10F)
                        {
                            power++;
                            amountLight = amountLight/10F;
                        }

                        GameObject lightMarker = GameObject.Instantiate(lightPrefab, Vector3.zero, Quaternion.Euler(new Vector3(0, -longitude, latitude)));
                        listOfLights.Add(lightMarker);
                        Light countryLight = lightMarker.GetComponentInChildren(typeof(Light)) as Light;
                        
                        Color prettyColor = new Color(reds[power]/255F,greens[power]/255F,blues[power]/255F);
                        countryLight.color = prettyColor;
                        float aRange = 2F*(radDeg/3.6F);
                        if(aRange > 4F)
                        {
                            aRange = 4;
                        }
                        countryLight.range = aRange;//Get the size as proportional to the country size
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

                        //Debug.Log (power);

                        GameObject fogMarker = GameObject.Instantiate(fogPrefab, Vector3.zero, Quaternion.Euler(new Vector3(0, -longitude, latitude)));
                        listOfFog.Add(fogMarker);
                        Renderer rend = fogMarker.GetComponentInChildren<Renderer>();
                        rend.material.SetFloat("_Pow", (power/10F)*4F);
                        rend.material.SetFloat("_Alpha", (radDeg/7.2F));
                        ParticleSystem aPart = fogMarker.GetComponentInChildren<ParticleSystem>();
                        Transform tChild = fogMarker.transform.GetChild(0); 
                        tChild.localScale = new Vector3((radDeg/7.2F),(radDeg/7.2F),(radDeg/7.2F));
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
                if(varToShow == 0)
                { 
                    markerCube= GameObject.Instantiate(rulerPrefab, Vector3.zero, Quaternion.Euler(new Vector3(0, -longitude-1F, latitude+1F)));
                }
                else
                {
                    markerCube= GameObject.Instantiate(rulerLight, Vector3.zero, Quaternion.Euler(new Vector3(0, -longitude-1F, latitude+1F)));
                }
                markerCube.name = countryName+"Legend";
                listOfLegendBars.Add(markerCube);
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
                        listOfElecBars.Add(lightCubePow);
                        Transform tChild = lightCubePow.transform.GetChild(0); 
                        tChild.localScale += new Vector3(power,0.1F,0.1F);
                        tChild.Translate(power/2F,0F,0F);
                        Renderer rend = lightCubePow.GetComponentInChildren<Renderer>();
                        rend.material.color = new Color(elecR[power]/255F,elecG[power]/255F,elecB[power]/255F);
                
                    }
                }
                else if (varToShow == 0)
                {

                    if((float)(entry.Value[years[yearIndex]][0]) != -1F)
                    {
                        GameObject treeCube = GameObject.Instantiate(cubePrefab, Vector3.zero, Quaternion.Euler(new Vector3(0, -longitude-1F, latitude)));
                        treeCube.name = countryName+"Trees";
                        listOfTreeBars.Add(treeCube);
                        Transform tChild = treeCube.transform.GetChild(0); 
                        tChild.localScale += new Vector3(amountTrees,0.1F,0.1F);
                        tChild.Translate(amountTrees/2F,0F,0F);
                        Renderer rend = treeCube.GetComponentInChildren<Renderer>();
                        //Renderer rend = treeCube.GetComponent<Renderer>();
                        rend.material.color = new Color(treeR[(int)amountTrees]/255F,treeG[(int)amountTrees]/255F,treeB[(int)amountTrees]/255F);
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
                        pollCubePow.name = countryName+"CO2";
                        listOfCO2Bars.Add(pollCubePow);
                        Transform tChild = pollCubePow.transform.GetChild(0); 
                        tChild.localScale += new Vector3(power,0.1F,0.1F);
                        tChild.Translate(power/2F,0F,0F);
                        Renderer rend = pollCubePow.GetComponentInChildren<Renderer>();
                        rend.material.color = new Color(CO2R[power]/255F,CO2G[power]/255F,CO2B[power]/255F);

                    }
                }
            }
    }


    void drawTreesFromScratch()
    {
        foreach(KeyValuePair<string, Dictionary<string, List<float>>> entry in reader.countriesPollution)
            {
                string countryName = entry.Key; 
                float latitude = reader.countries[countryName][1];
                float longitude = reader.countries[countryName][2];

                float radDeg = reader.countries[countryName][7];

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

    //This function removes all game objects for all countries
    void clearHouse()
    {
        while(listOfTreeBars.Count > 0)
        {
            GameObject aTree = listOfTreeBars[0];
            listOfTreeBars.RemoveAt(0);
            Destroy(aTree);
        }
        while(listOfCO2Bars.Count > 0)
        {
            GameObject aFog = listOfCO2Bars[0];
            listOfCO2Bars.RemoveAt(0);
            Destroy(aFog);
        }
        while(listOfElecBars.Count > 0)
        {
            GameObject aSpark = listOfElecBars[0];
            listOfElecBars.RemoveAt(0);
            Destroy(aSpark);
        }
        while(listOfLegendBars.Count > 0)
        {
            GameObject aLeg = listOfLegendBars[0];
            listOfLegendBars.RemoveAt(0);
            Destroy(aLeg);
        }
        while(listOfLights.Count > 0)
        {
            GameObject aLight = listOfLights[0];
            listOfLights.RemoveAt(0);
            Destroy(aLight);
        }
        while(listOfFog.Count > 0)
        {
            GameObject aFog = listOfFog[0];
            listOfFog.RemoveAt(0);
            Destroy(aFog);
        }
          foreach(KeyValuePair<string, List<GameObject>> entry in markers)
                    {
                        List<GameObject> tempList = new List<GameObject>();
                        if (markers.TryGetValue(entry.Key, out tempList))
                            {
                                while(tempList.Count > 0)
                                {
                                    GameObject aTree = tempList[0];
                                    tempList.RemoveAt(0);
                                    Destroy(aTree);
                                }
                            }
                    }

    }
}
