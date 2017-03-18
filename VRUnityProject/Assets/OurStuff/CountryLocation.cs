using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Net;
using UnityEngine;

public class CountryLocation : MonoBehaviour {
    float PI = 3.1415F;
    // For each item in dict <name of country> -> [size in km^2, lat, long,  min lat, max lat, min long, max long]
    public Dictionary<string, List<float>> countries = new Dictionary<string, List<float>>();
    public Dictionary<string, List<float>> countriesTrees = new Dictionary<string, List<float>>();
    public Dictionary<string, List<float>> countriesCO2 = new Dictionary<string, List<float>>();
    public Dictionary<string, List<float>> countriesElectricity = new Dictionary<string, List<float>>();
    public void whyWontYouWork()
    {
        using (var fs = File.OpenRead("../Data/reduced_compiled_data.csv"))
        using (var reader = new StreamReader(fs))
        {
            string line = "";
            while ((line = reader.ReadLine()) != null)
            {
                var values = line.Split(',');
                Debug.Log(values[0]);
                var key = values[0];
                if(values[0] != "Country Name")
                {
                    
                    if(values[1] == "Forest area (% of land area)")
                    {
                        List<float> grooveList = new List<float>();
                        for (int i = 2; i < 25; i++)
                        {
                            //Read in the forest area by year
                            grooveList.Add(float.Parse(values[i]));
                        }
                        countriesTrees.Add(key, grooveList);

                    }
                    else if (values[1] == "CO2 emissions")
                    {
                        List<float> grooveList = new List<float>();
                        for (int i = 2; i < 25; i++)
                        {
                            //Read in the forest area by year
                            grooveList.Add(float.Parse(values[i]));
                        }
                        countriesCO2.Add(key, grooveList);

                    }
                    else
                    {
                        List<float> grooveList = new List<float>();
                        for (int i = 2; i < 25; i++)
                        {
                            //Read in the forest area by year
                            grooveList.Add(float.Parse(values[i]));
                        }
                        countriesElectricity.Add(key, grooveList);
                    }
                }

            }
        }
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
                }

            }
        }

    }

    
        


    // Update is called once per frame
    void Update () {
		
	}
}
