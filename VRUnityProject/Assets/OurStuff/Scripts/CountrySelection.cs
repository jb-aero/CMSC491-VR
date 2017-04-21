using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class CountrySelection : MonoBehaviour {

	private SteamVR_TrackedObject trackedObject;
	private SteamVR_Controller.Device device;
	private RaycastHit raycastHit;
	// private GameObject selected = null;
	private string sname;
	private Text country, valt;
	private ObjectPlacer op;

	// Use this for initialization
	void Start ()
	{
		trackedObject = GetComponent<SteamVR_TrackedObject> ();
		device = SteamVR_Controller.Input ((int) trackedObject.index);
		op = GameObject.Find ("PuppetMaster").GetComponent<ObjectPlacer> ();
		country = GameObject.Find ("CountryName").GetComponent<Text> ();
		valt = GameObject.Find ("SelectedValue").GetComponent<Text> ();
	}
	
	// Update is called once per frame
	void Update ()
	{
		if (device.GetPressDown (SteamVR_Controller.ButtonMask.Trigger))
		{
			if(Physics.Raycast(transform.position, transform.forward, out raycastHit))
			{
				// selected = raycastHit.collider.gameObject;
				// Debug.Log ("Selected: " + selected.transform.root.name);

				sname = raycastHit.collider.gameObject.transform.root.name;

				if (sname != "Earth")
				{
					if (op.barGraph_view)
					{
						if (sname[sname.Length - 1] == 'd')
						{
							country.text = sname.Substring (0, sname.Length - "Legend".Length);
						}
						else
						{
							switch (op.varToShow)
							{
							case 0:
								country.text = sname.Substring (0, sname.Length - "Trees".Length);
								break;
							case 1:
								country.text = sname.Substring (0, sname.Length - "ElectricityPow".Length);
								break;
							case 2:
								country.text = sname.Substring (0, sname.Length - "CO2".Length);
								break;
							}
						}
					}
					else
					{
						switch (op.varToShow)
						{
						case 0:
							country.text = sname.Substring (0, sname.Length - 2);
							break;
						case 1:
							country.text = sname.Substring (0, sname.Length - "_light".Length);
							break;
						case 2:
							country.text = sname.Substring (0, sname.Length - "_CO2".Length);
							break;
						}
						
					}

					float val = op.reader.countriesPollution[country.text][op.years[op.yearIndex]][op.varToShow];
					if (op.varToShow == 0)
					{
						valt.text = val.ToString ("2f");
					}
					else
					{
						int power = 0;
						while(val >= 10F)
						{
							power++;
							val = val/10F;
						}
						valt.text = val.ToString ("4F") + "e" + power.ToString ();
					}

					// Debug.Log ("Selected: " + country.text);
				}
			}
		}
	}
}
