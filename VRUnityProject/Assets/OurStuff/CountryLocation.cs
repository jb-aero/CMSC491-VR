using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Net;
using UnityEngine;

public class CountryLocation : MonoBehaviour {
    float PI = 3.1415F;
    // For each item in dict <name of country> -> [size in km^2, lat, long,  min lat, max lat, min long, max long, radius in degrees]
    public Dictionary<string, List<float>> countries = new Dictionary<string, List<float>>();
    //This dictionary holds the countries as keys. For each country, it has a dictionary. In that dictionary, the keys are years and the values are a list
    //The pollution numbers are, in this order, [forest area %, CO2 emmisions, Electricity] A -1 indicates a missing value.
    public Dictionary<string, Dictionary<string, List<float>>> countriesPollution = new Dictionary<string, Dictionary<string, List<float>>>();
    public void dictionaryInception()
    {

        float maxEmmission = 0.0F;
        float maxElectricity = 0.0F;
        //Read in the file and get the maximum CO2 emmision and electricity amount
        using (var fs = File.OpenRead("../Data/reduced_compiled_data.csv"))
        using (var reader = new StreamReader(fs))
        {
            string line = "";
            List<string> headerList = new List<string>() { "1990", "1991", "1992", "1993", "1994", "1995", "1996", "1997", "1998", "1999", "2000", "2001", "2002", "2003", "2004", "2005", "2006", "2007", "2008", "2009", "2010", "2011", "2012", "2013" };
            while ((line = reader.ReadLine()) != null)
            {
                var values = line.Split(',');
                if(values[1] == "CO2 emissions")
                {
                    for(int i = 2; i < 25; i++)
                    {
                        float theNum = float.Parse(values[i]);
                        if (theNum > maxEmmission)
                        {
                            maxEmmission = theNum;
                        }
                    }
                }
                if(values[1] == "Electric power consumption (kWh per capita)")
                {
                    for (int i = 2; i < 25; i++)
                    {
                        float theNum = float.Parse(values[i]);
                        if (theNum > maxElectricity)
                        {
                            maxElectricity = theNum;
                        }
                    }
                }
            }
        }

        Debug.Log("Max emission: "+maxEmmission);
        Debug.Log("Max electricity: " + maxElectricity);


        using (var fs = File.OpenRead("../Data/reduced_compiled_data.csv"))
        using (var reader = new StreamReader(fs))
        {
            string line = "";
            List<string> headerList = new List<string>() { "1990", "1991", "1992", "1993", "1994", "1995", "1996", "1997", "1998", "1999", "2000", "2001", "2002", "2003", "2004", "2005", "2006", "2007", "2008", "2009", "2010", "2011", "2012", "2013" };
            while ((line = reader.ReadLine()) != null)
            {
                var values = line.Split(',');
                //Debug.Log(values[0]);
                var key = values[0];
                Dictionary<string, List<float>> tempDict;
                if (values[0] != "Country Name")
                {
                    tempDict = new Dictionary<string, List<float>>();
                    //If we have already read in some values for this country
                    if (countriesPollution.TryGetValue(key, out tempDict))
                    {
                        //If the country is already in the dictionary, get the dictionary of year->data
                        for (int index = 0; index < headerList.Count; index++)
                        {
                            float theValue = float.Parse(values[index + 2]);

                            //Debug.Log(values[1]);
                            
                            if(values[1] == "CO2 emissions" && theValue != -1F)
                            {
                                theValue = theValue / (float)maxEmmission;
                                if(theValue > 1F)
                                {
                                    Debug.Log(values[0]+" "+theValue);
                                }
                            }
                            if(values[1] == "Electric power consumption (kWh per capita)"  && theValue != -1F)
                            {
                                theValue = theValue / (float)maxElectricity;
                            }
                            //Debug.Log(theValue);
                            List<float> tempList = new List<float>();
                            if (tempDict.TryGetValue(headerList[index], out tempList))  
                            {
                                //If this year already is in the dictionary for this country, add to this list
                                tempList.Add(theValue);
                            }
                            else
                            {
                                tempList.Add(theValue);
                                tempDict.Add(headerList[index], tempList);
                            }
                        }
                        
                    }
                    else
                    {
                        tempDict = new Dictionary<string, List<float>>();
                        //If the country is not already in the dictionary, make the dictionary of year->data
                        

                        for (int index = 0; index < headerList.Count; index++)
                        {
                            List<float> tempList2 = new List<float>();
                            string year = headerList[index];
                            //Debug.Log(key+" "+year+" "+values[1]);
                            float theValue = float.Parse(values[index + 2]);
                           // Debug.Log(values[1]);
                            //Debug.Log(theValue);
                            tempList2.Add(theValue);
                            tempDict.Add(year, tempList2);
                        }
                        countriesPollution.Add(key, tempDict);
                    }
                }
                
            }
        }

        //Debug.Log(countriesPollution);

    }

    

    public void NotANormalWord() {
        Debug.Log("I AM DOING THINGS TOO");
        using (var fs = File.OpenRead("../Data/CountriesLandAreafromWikipedia.csv"))
        using (var reader = new StreamReader(fs))
        {

            string line = "";

            while ((line = reader.ReadLine()) != null)
            {
                var values = line.Split(',');
                string key = values[0];
                if (key != "Country" && key != "Total")
                {
                    List<float> grooveList = new List<float>();

                    //Debug.Log("Values is null? " + (values == null));
                    //Debug.Log("templist is null? " + (tempList == null));
                    grooveList.Add(float.Parse(values[1]));
                    countries.Add(key, grooveList);
                    //Add the size to the dictionary

                }
            }
        }
        using (var fs = File.OpenRead("../Data/Lat and Long of Capitals of Countries.csv"))
        using (var reader = new StreamReader(fs))
        {
            List<float> tempList = new List<float>();
            string line = "";
            while ((line = reader.ReadLine()) != null)
            {
                var values = line.Split(',');
                if (countries.TryGetValue(values[0], out tempList))
                {
                    tempList.Add(float.Parse(values[1]));
                    tempList.Add(float.Parse(values[2]));
                    float size = tempList[0];
                    //Take 2*sqrt(SA/PI) to get diameter of country (assuming a circle)
                    float diameter = 2 * Mathf.Sqrt((size / PI));
                    //Take this / 40075 km which is diameter / circumference of the earth
                    float cRange = diameter / 40075.0F;
                    // that * 360 gets us the number of degrees the range should be
                    float degRange = (cRange * 360.0F) / 2F;
                    //Take the lat and long for each country, and add/subtract 0.5*num degrees to get the lat/long range for each country
                    float minLat = tempList[0] - degRange;
                    if (minLat < -90)
                    {
                        minLat = -90;
                    }
                    float maxLat = tempList[0] + degRange;
                    if (maxLat > 90)
                    {
                        maxLat = 90;
                    }
                    float minLong = tempList[1] - degRange;
                    if (minLong < -180)
                    {
                        minLong = -180;
                    }
                    float maxLong = tempList[1] + degRange;
                    if (maxLong > 180)
                    {
                        maxLong = 180;
                    }
                    tempList.Add(minLat);
                    tempList.Add(maxLat);
                    tempList.Add(minLong);
                    tempList.Add(maxLong);
                    tempList.Add(degRange);
                  
                    //Debug.Log(values[0] + " " + tempList[1] + " " + tempList[2]);
                }

            }
        }

    }

    
        


    // Update is called once per frame
    void Update () {
		
	}
}
