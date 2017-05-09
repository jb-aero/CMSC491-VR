using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System.Text.RegularExpressions;

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
	public float secPerChange;

    private List<string> variables = new List<string>{"% Forest Cover","Electricity Usage(KWa per capita)","CO2 Emissions (kton)"};

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

    //To detect a change, we keep aprivate variables with the previous value
    private int oldYear;
    private int lastTreeYear;
    private int oldVar;
	private bool wasBar;
	private float lastVarChange;

    public List<string> years;

    //Colors for electricity

    private List<float> elecR=  new List<float>{227F,223F,219F,215F,211F,207F,203F,199F,195F,191F};
    private List<float> elecG=  new List<float>{229F,203F,178F,152F,127F,101F,76F,50F,25F,0F};
    private List<float> elecB=  new List<float>{0F,0F,0F,0F,0F,0F,0F,0F,0F,0F};
    
    //Colors for CO2 scales
    private List<float> CO2R =  new List<float>{44F,57F,70F,83F,96F,109F,122F,135F,148F,162F};
    private List<float> CO2G =  new List<float>{237F,213F,189F,165F,141F,117F,93F,69F,45F,21F};
    private List<float> CO2B =  new List<float>{246F,230F,215F,200F,184F,169F,154F,138F,123F,108F};

    //Colors for tree Scales
    private List<float> treeR =  new List<float>{28F,28F,28F,28F,28F,28F,28F,28F,28F,28F};//{0F,0F,0F,0F,0F,26F,77F,128F,179F,230F};
    private List<float> treeG =  new List<float>{130F,130F,130F,130F,130F,130F,130F,130F,130F,130F};//{26F,77F,128F,179F,230F,255F,255F,255F,255F,255F};
    private List<float> treeB =  new List<float>{43F,43F,43F,43F,43F,43F,43F,43F,43F,43F};//{0F,0F,0F,0F,0F,26F,77F,128F,179F,230F};

    private List<GameObject> legendList = new List<GameObject>();
    private GameObject canvas;
    private Text description;

	// Use this for initialization
    //This function runs when the game starts
	void Start () {

        //Initialize all the variables
        barGraph_view = true;
        varToShow = 1;//0 means trees, 1 is electricity, 2 is CO2
        yearIndex = 0;
        oldYear = 0;
        lastTreeYear = 0;
        oldVar = varToShow;
        wasBar = barGraph_view;
		lastVarChange = 0;
        markers = new Dictionary<string, List<GameObject>>();

        Debug.Log ("LOOK WE ARE DOING THINGS");
        //Build the dictionary
		reader.NotANormalWord ();
		reader.dictionaryInception();
		years = reader.headerList;

        //Build the list of legend-objects so we can color our legend later
        GameObject canvas = GameObject.Find("Canvas");
        foreach(Transform child in canvas.transform)
        {
            Debug.Log(child.name);
            //Debug.Log(counter);
            
            if(child.name.StartsWith("Legend"))
            {
                string num = Regex.Match(child.name,@"\d+").Value;
                child.name = num;
                legendList.Add(child.gameObject);
                //counter += 1;
            }
            if(child.name == "Desc")
            {
                description = child.gameObject.GetComponentInChildren<Text>();
            }
        }

        //Draw our initial view
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

        //Get button presses
		if (Input.GetKeyDown ("m"))
		{
			Debug.Log ("Hit left menu.");
			barGraph_view = !barGraph_view;
		}

		if (Input.GetButtonDown ("LeftMenuButton"))
		{
			Debug.Log ("Hit right menu so hard.");
			++varToShow;
		}

		if (lastVarChange < Time.time)
		{
			if (Input.GetAxis ("Years") > 0)
			{
				++yearIndex;
				lastVarChange = Time.time + secPerChange;
			}
			else if (Input.GetAxis ("Years") < 0)
			{
				--yearIndex;
				lastVarChange = Time.time + secPerChange;
			}
		}

        if(yearIndex < 0)
        {
            yearIndex += 24;
        }

		varToShow %= 3;
		yearIndex %= 24;
		yearText.text = years [yearIndex];
        varText.text = variables[varToShow];

        //Check if the view, variable, and year have changed (in that order)
        if(wasBar != barGraph_view)
        {
            //If we have switched views (not allowed for user trials, update the view)
            clearHouse();
            if(barGraph_view == true)
            {
                DrawRecs();
            }
            else{
                if(varToShow == 0)
                {
                    if(markers.Keys.Count == 0)
                    {
                        drawTreesFromScratch();
                    }
                    else{
                        updateTrees();
                    }
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
            //If we switched variables in the natural view, draw the appropriate stuff
            if(varToShow == 0)
            {
                //Delete the CO2 and light values, and draw the trees
                clearHouse();

                if(markers.Keys.Count == 0)
                    {
                        drawTreesFromScratch();
                    }
                    else{
                        updateTrees();
                    }
            }
            else
            {
                //We are switching to something not trees
                clearHouse();
                reDrawElecAndCO2();
            }

            oldVar = varToShow;
        }
        else if(oldVar != varToShow && barGraph_view == true)
        {
            //Switching between variables in bar view
            clearHouse();
            DrawRecs();
            oldVar = varToShow;
        }
        else if(yearIndex != oldYear)
        {
            //The year has been changed
    		if(barGraph_view == false)
            {
                if(varToShow == 0)
                {
                    //We don't want to redraw all of the trees, so just update
                    updateTrees();
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


    //This function is for drawing electricity and fog in the natural view
    void reDrawElecAndCO2()
    {


        //Color lists for the light and fog
        List<float> reds =  new List<float>{0F,24F,0F,57F,128F,91F,255F,125F,255F,159F,255F,193F,255F,227F};
        List<float> greens =  new List<float>{255F,226F,255F,190F,255F,155F,255F,119F,128F,84F,64F,48F,0F,13F};
        List<float> blues =  new List<float>{128F,224F,0F,209F,0F,194F,0F,180F,0F,165F,0F,150F,0F,136F};

        List<float> fogR =  new List<float>{255F,98F,235F,104F,214F,110F,194F,116F,153F,122F,122F,128F,61F,134F,0F,141F};
        List<float> fogG =  new List<float>{255F,228F,235F,204F,214F,180F,194F,157F,153F,133F,122F,110F,61F,86F,0F,63F};
        List<float> fogB =  new List<float>{255F,243F,224F,225F,194F,208F,163F,190F,102F,173F,82F,155F,41F,138F,0F,121F};



        if(varToShow == 1)
        {
            description.text = "Value = Color";
        //Now let's update the legend
                    foreach(GameObject legendPiece in legendList)
                    {
                        int theNum = int.Parse(legendPiece.transform.name.ToString());

                        if(theNum < blues.Count)
                        {

                            legendPiece.GetComponentInChildren<Image>().color = new Color(reds[theNum]/255F,greens[theNum]/255F,blues[theNum]/255F);
                            legendPiece.transform.GetComponentInChildren<Text>().color = new Color(1F,1F,1F,1F);
                            if(theNum%2 == 0)
                            {
                                legendPiece.transform.GetComponentInChildren<Text>().text = "1x10^"+((theNum/2));
                            }
                            else
                            {
                                legendPiece.transform.GetComponentInChildren<Text>().text = "5x10^"+((theNum-1)/2);
                            }
                        }
                        else
                        {
                            legendPiece.GetComponentInChildren<Image>().color = new Color(0F,0F,0F,0F);
                            legendPiece.transform.GetComponentInChildren<Text>().color = new Color(0F,0F,0F,0F);
                        }
                    }
        }
        else{
            description.text = "Value = Color";
            //Now let's update the legend
                    foreach(GameObject legendPiece in legendList)
                    {
                        int theNum = int.Parse(legendPiece.transform.name.ToString());
                        if(theNum < blues.Count)
                        {
                            legendPiece.GetComponentInChildren<Image>().color = new Color(fogR[theNum]/255F,fogG[theNum]/255F,fogB[theNum]/255F);
                            legendPiece.transform.GetComponentInChildren<Text>().color = new Color(1F,1F,1F,1F);
                            if(theNum%2 == 0)
                            {
                                legendPiece.transform.GetComponentInChildren<Text>().text = "1x10^"+((theNum/2));
                            }
                            else
                            {
                                legendPiece.transform.GetComponentInChildren<Text>().text = "5x10^"+((theNum-1)/2);
                            }
                        }
                        else
                        {
                            legendPiece.GetComponentInChildren<Image>().color = new Color(0F,0F,0F,0F);
                            legendPiece.transform.GetComponentInChildren<Text>().color = new Color(0F,0F,0F,0F);
                        }
                    }
        }

        //Now loop over the countries, and update what needs to be done
        foreach(KeyValuePair<string, Dictionary<string, List<float>>> entry in reader.countriesPollution)
            {
                string countryName = entry.Key;
                float latitude = reader.countries[countryName][1];
                float longitude = reader.countries[countryName][2];
                float radDeg = reader.countries[countryName][7];

                //If we want lights, draw the lights!
                if(varToShow == 1)
                {
                    float amountLight = (float)(entry.Value[years[yearIndex]][2]);///reader.maxElectList[0]);
                    if(amountLight > 0F)
                    {

                        //Get the exponent and value
                        int power = 0;
                        while(amountLight >= 10F)
                        {
                            power++;
                            amountLight = amountLight/10F;
                        }

                        //Odd indices represent 5x10^# so we double our stuff
                        power = power * 2;
                        if(amountLight >= 5F)
                        {
                            power = power + 1;
                        }

                        //Draw the lights
                        GameObject lightMarker = GameObject.Instantiate(lightPrefab, Vector3.zero, Quaternion.Euler(new Vector3(0, -longitude, latitude)));
                        listOfLights.Add(lightMarker);
                        Light countryLight = lightMarker.GetComponentInChildren(typeof(Light)) as Light;

                        //Set the color and size of the lights
                        Color prettyColor = new Color(reds[power]/255F,greens[power]/255F,blues[power]/255F);
                        countryLight.color = prettyColor;
                        //countryLight.intensity = 1F + (amountLight/10F)*7F;
                        float aRange = 2F*(radDeg/3.6F);
                        if(aRange > 4F)
                        {
                            aRange = 4;
                        }
                        countryLight.range = aRange;//Get the size as proportional to the country size
                        lightMarker.name = countryName+"_light";
                    }


                }

                //If we want fog, draw the fog!
                if(varToShow == 2)
                {

                    float amountCO2 = (float)(entry.Value[years[yearIndex]][1]);
                    if(amountCO2 != -1F)
                    {
                    //Get the exponent and value
                        if(amountCO2 > 0F)
                        {
                            float power = 0F;
                            while(amountCO2 >= 10F)
                            {
                                power++;
                                amountCO2 = amountCO2/10F;
                            }


                            //Odd indices represent 5x10^# so we double our stuff
                            power = power * 2F;
                            if(amountCO2 >= 5F)
                            {
                                power = power + 1;
                            }

                            //Draw the fog
                            GameObject fogMarker = GameObject.Instantiate(fogPrefab, Vector3.zero, Quaternion.Euler(new Vector3(0, -longitude, latitude)));
                            listOfFog.Add(fogMarker);

                            //Set the fog variables to appropriate values
                            Renderer rend = fogMarker.GetComponentInChildren<Renderer>();
                            //rend.material.SetFloat("_Pow", (amountCO2/10F)*4F);
                            rend.material.SetFloat("_Alpha", power/20F);
                            rend.material.SetColor("_Color", new Color(fogR[(int)power]/255F,fogG[(int)power]/255F,fogB[(int)power]/255F));
                            ParticleSystem aPart = fogMarker.GetComponentInChildren<ParticleSystem>();
                            Transform tChild = fogMarker.transform.GetChild(0);
                            float newRad;
                            if(radDeg/10F > 1F)
                            {
                                newRad = 1F;
                            }
                            else if(radDeg/15F < 0.1F)
                            {
                                newRad = 0.1F;
                            }
                            else
                            {
                                newRad = radDeg/15F;
                            }
                            tChild.localScale = new Vector3((newRad),(newRad),(newRad));
                            aPart.Play();
                            fogMarker.name = countryName+"_CO2";
                        }
                    }
                }

            }
    }



    //This function draws the bar graph
    void DrawRecs()
    {
        //Update legend
        if(varToShow != 0)
        {
        description.text = "Value = Color x 10^height";
        }
        else
        {
            description.text = "Value = Height (10% step sizes)";
        }
        if(varToShow == 1)
        {
        //Now let's update the legend for electricity
                    foreach(GameObject legendPiece in legendList)
                    {
                        int theNum = int.Parse(legendPiece.transform.name.ToString());
                        if(theNum %2 == 0)
                        {
                            theNum = theNum / 2;
                            legendPiece.GetComponentInChildren<Image>().color = new Color(elecR[theNum]/255F,elecG[theNum]/255F,elecB[theNum]/255F);
                            legendPiece.transform.GetComponentInChildren<Text>().color = new Color(1F,1F,1F,1F);
							legendPiece.transform.GetComponentInChildren<Text>().text = theNum.ToString();
                        }
                        else{
                            legendPiece.GetComponentInChildren<Image>().color = new Color(0F,0F,0F,0F);
                            legendPiece.transform.GetComponentInChildren<Text>().color = new Color(0F,0F,0F,0F);
                        }
                    }
        }
        else if(varToShow == 2)
        {

        //Now let's update the legend for CO2
                    foreach(GameObject legendPiece in legendList)
                    {
                        int theNum = int.Parse(legendPiece.transform.name.ToString());
                        if(theNum % 2 == 0)
                        {
                            theNum = theNum / 2;
                            legendPiece.GetComponentInChildren<Image>().color = new Color(CO2R[theNum]/255F,CO2G[theNum]/255F,CO2B[theNum]/255F);
							legendPiece.transform.GetComponentInChildren<Text>().color = new Color(1F,1F,1F,1F);
							legendPiece.transform.GetComponentInChildren<Text>().text = theNum.ToString();
                        }
                        else
                        {
                            legendPiece.GetComponentInChildren<Image>().color = new Color(0F,0F,0F,0F);
                            legendPiece.transform.GetComponentInChildren<Text>().color = new Color(0F,0F,0F,0F);
                        }
                    }

        }
        else{
            foreach(GameObject legendPiece in legendList)
                    {
                        //Treeeees
                        int theNum = int.Parse(legendPiece.transform.name.ToString());
                        if(theNum % 2 == 0)
                        {
                            theNum = theNum / 2;
                            legendPiece.GetComponentInChildren<Image>().color = new Color(treeR[theNum]/255F,treeG[theNum]/255F,treeB[theNum]/255F);
							legendPiece.transform.GetComponentInChildren<Text>().color = new Color(1F,1F,1F,1F);
							legendPiece.transform.GetComponentInChildren<Text>().text = theNum.ToString();
                        }
                        else
                        {
                            legendPiece.GetComponentInChildren<Image>().color = new Color(0F,0F,0F,0F);
                            legendPiece.transform.GetComponentInChildren<Text>().color = new Color(0F,0F,0F,0F);
                        }
                    }
        }
            //Loop over the countries
            foreach(KeyValuePair<string, Dictionary<string, List<float>>> entry in reader.countriesPollution)
            {
                string countryName = entry.Key;

                int numTrees = (int)(entry.Value[years[yearIndex]][0]);
                float latitude = reader.countries[countryName][1];
                float longitude = reader.countries[countryName][2];

                /*So the idea here is to create a ruler for the cubes to compare to*/
                //Generate a ruler. Height depends on if it is trees or not, since elec and CO2 don't go all the way to 10
                GameObject markerCube;
                if(varToShow == 0)//Trees
                {
                    markerCube= GameObject.Instantiate(rulerPrefab, Vector3.zero, Quaternion.Euler(new Vector3(0, -longitude, latitude)));//-longitude-1F, latitude+1F)));
                }
                else
                {
                    markerCube= GameObject.Instantiate(rulerLight, Vector3.zero, Quaternion.Euler(new Vector3(0, -longitude,latitude)));//-1F, latitude+1F)));
                }

                markerCube.name = countryName+"Legend";
                listOfLegendBars.Add(markerCube);
                Transform tOChild = markerCube.transform.GetChild(0);

                float amountLight = (float)(entry.Value[years[yearIndex]][2]);
                //Debug.Log(amountLight);
                float amountTrees = (float)(entry.Value[years[yearIndex]][0])/10F;
                float amountPoll = (float)(entry.Value[years[yearIndex]][1]);

                if(varToShow == 1)//Elec
                {
                    if((float)(entry.Value[years[yearIndex]][2]) != -1F)
                    {
                        int power = 0;
                        while(amountLight >= 10F)
                        {
                            power++;
                            amountLight = amountLight/10F;
                        }

                        //Draw the bar and set the height and color
                        GameObject lightCubePow = GameObject.Instantiate(cubePrefab, Vector3.zero, Quaternion.Euler(new Vector3(0, -longitude,latitude)));//+1F, latitude+1F)));
                        lightCubePow.name = countryName+"ElectricityPow";
                        listOfElecBars.Add(lightCubePow);
                        Transform tChild = lightCubePow.transform.GetChild(0);
                        //Since rectangles grow symmetrically, need to move it out of the earth a bit
                        tChild.localScale += new Vector3(power,0.1F,0.1F);
                        tChild.Translate(power/2F,0F,0F);
                        Renderer rend = lightCubePow.GetComponentInChildren<Renderer>();
                        rend.material.color = new Color(elecR[(int)amountLight]/255F,elecG[(int)amountLight]/255F,elecB[(int)amountLight]/255F);

                    }
                }
                else if (varToShow == 0)//Treeeees
                {

                    if((float)(entry.Value[years[yearIndex]][0]) != -1F)
                    {
                        //Draw the bar and set the height and color, bot of which are just %/10
                        GameObject treeCube = GameObject.Instantiate(cubePrefab, Vector3.zero, Quaternion.Euler(new Vector3(0, -longitude,latitude)));//-1F, latitude)));
                        treeCube.name = countryName+"Trees";
                        listOfTreeBars.Add(treeCube);
                        Transform tChild = treeCube.transform.GetChild(0);
                        tChild.localScale += new Vector3(amountTrees,0.1F,0.1F);
                        tChild.Translate(amountTrees/2F,0F,0F);
                        Renderer rend = treeCube.GetComponentInChildren<Renderer>();
                        rend.material.color = new Color(treeR[(int)amountTrees]/255F,treeG[(int)amountTrees]/255F,treeB[(int)amountTrees]/255F);
                    }
                }
                else//CO2
                {
                    if((float)(entry.Value[years[yearIndex]][1])!= -1F)
                    {

                        int power = 0;
                        while(amountPoll >= 10F)
                        {
                            power++;
                            amountPoll = amountPoll/10F;
                        }
                        //Draw the bar and set the height and color
                        GameObject pollCubePow = GameObject.Instantiate(cubePrefab, Vector3.zero, Quaternion.Euler(new Vector3(0, -longitude,latitude)));//+1F, latitude)));
                        pollCubePow.name = countryName+"CO2";
                        listOfCO2Bars.Add(pollCubePow);
                        Transform tChild = pollCubePow.transform.GetChild(0);
                        tChild.localScale += new Vector3(power,0.1F,0.1F);
                        //Adjust height
                        tChild.Translate(power/2F,0F,0F);
                        Renderer rend = pollCubePow.GetComponentInChildren<Renderer>();
                        rend.material.color = new Color(CO2R[(int)(amountPoll)]/255F,CO2G[(int)(amountPoll)]/255F,CO2B[(int)amountPoll]/255F);

                    }
                }
            }
    }


    //This function is called the very first time trees are drawn in the scene. It builds the country->tree dictionary from scratch
    void drawTreesFromScratch()
    {
        description.text = "Each tree = 1% Forest cover";
        //No legend necessary
        foreach(GameObject legendPiece in legendList)
                    {
                        legendPiece.GetComponentInChildren<Image>().color = new Color(0F,0F,0F,0F);
                        legendPiece.transform.GetComponentInChildren<Text>().color = new Color(0F,0F,0F,0F);
                    }

        //loop over countries
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
                        //For each percent, make a new tree with a random lat and long within the country's borders
                        float newLat = Random.Range(latitude - radDeg, latitude + radDeg);
                        float newLong = Random.Range(longitude - radDeg, longitude + radDeg);
                        GameObject marker = GameObject.Instantiate(prefab, Vector3.zero, Quaternion.Euler(new Vector3(0, -newLong, newLat)));
						marker.name = countryName + (i < 10 ? "0" : "") + i.ToString();
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
        //For the trees, we don't destroy them (since redrawing them is a pain), so they just get de-activated
    	foreach(KeyValuePair<string, List<GameObject>> entry in markers)
        {
            List<GameObject> tempList = new List<GameObject>();
            if (markers.TryGetValue(entry.Key, out tempList))
            {
                for(int t = 0; t < tempList.Count; t++)
                {
                    GameObject aTree = tempList[t];
					aTree.SetActive (false);
                }
            }
        }

    }

    //This is called whenever something changes with trees (or changes to trees). Only called if trees have been displpayed before.
    void updateTrees()
    {
        description.text = "Each Tree = 1% Forest Cover";
        foreach(GameObject legendPiece in legendList)
                    {
                        legendPiece.GetComponentInChildren<Image>().color = new Color(0F,0F,0F,0F);
                        legendPiece.transform.GetComponentInChildren<Text>().color = new Color(0F,0F,0F,0F);
                    }


        //Make all existing trees visible
        foreach(KeyValuePair<string, List<GameObject>> entry in markers)
        {
            List<GameObject> tempList = new List<GameObject>();
            if (markers.TryGetValue(entry.Key, out tempList))
            {
                for(int t = 0; t < tempList.Count; t++)
                {
                    GameObject aTree = tempList[t];
					aTree.SetActive (true);
                }
            }
        }
        //Loop over countries
        foreach(KeyValuePair<string, Dictionary<string, List<float>>> entry in reader.countriesPollution)
                    {
                        //Get num trees in new year.
                        int amountTreesOld = (int)(entry.Value[years[lastTreeYear]][0]);
                        int amountTrees = (int)(entry.Value[years[yearIndex]][0]);
                        if(amountTreesOld != -1F && amountTrees != -1F)
                        {
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
                                        if(tempList.Count > 0)
                                        {
                                            GameObject aTree = tempList[0];
                                            tempList.RemoveAt(0);
                                            Destroy(aTree);
                                        }
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
        lastTreeYear = yearIndex;
    }
}
