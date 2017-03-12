using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Net;
using UnityEngine;

public class CountryLocation : MonoBehaviour {
    float PI = 3.1415F;
	public Dictionary<string, List<float>> countries = new Dictionary<string, List<float>>();

    public void NotANormalWord () {
		Debug.Log("I AM DOING THINGS TOO");
        using (var fs = File.OpenRead("../Data/Lat and Long of Capitals of Countries.csv"))
        using (var reader = new StreamReader(fs))
        {
            //First read in the file with the lat and long
            var line = reader.ReadLine();
            var values = line.Split(',');
            if(values[0] != "Country Name")
            {
                string key = values[0];
                List<float> grooveList = new List<float>();
                grooveList.Add(float.Parse(values[1]));
                grooveList.Add(float.Parse(values[2]));
                countries.Add(key, grooveList);
            }

        }

        using (var fs = File.OpenRead("../Data/CountriesLandAreafromWikipedia.csv"))
        using (var reader = new StreamReader(fs))
        {
            List<float> tempList = new List<float>();
            var line = reader.ReadLine();
			var values = line.Split(',');
			Debug.Log ("Values is null? " + (values == null));
			Debug.Log ("templist is null? " + (tempList == null));
            if(countries.TryGetValue(values[0], out tempList))
            {
                //The country was in the dictionary, so now we add the size to the list
                float size = float.Parse(values[1]);
                tempList.Add(size);
                //Take 2*sqrt(SA/PI) to get diameter of country (assuming a circle)
                float diameter = 2*Mathf.Sqrt((size / PI));
                //Take this / 40075 km which is diameter / circumference of the earth
                float cRange = diameter / 40075.0F;
                // that * 360 gets us the number of degrees the range should be
                float degRange = (cRange * 360.0F)/2F;
                //Take the lat and long for each country, and add/subtract 0.5*num degrees to get the lat/long range for each country
                float minLat = tempList[0] - degRange;
                if(minLat < -90)
                {
                    minLat = -90;
                }
                float maxLat = tempList[0] + degRange;
                if(maxLat > 90)
                {
                    maxLat = 90;
                }
                float minLong = tempList[1] - degRange;
                if(minLong < -180)
                {
                    minLong = -180;
                }
                float maxLong = tempList[1] + degRange;
                if(maxLong > 180)
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

    // Update is called once per frame
    void Update () {
		
	}
}
