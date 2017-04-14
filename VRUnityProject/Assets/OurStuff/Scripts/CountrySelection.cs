using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CountrySelection : MonoBehaviour {

	private SteamVR_TrackedObject trackedObject;
	private SteamVR_Controller.Device device;

	// Use this for initialization
	void Start ()
	{
		trackedObject = GetComponent<SteamVR_TrackedObject> ();
		device = SteamVR_Controller.Input ((int) trackedObject.index);
	}
	
	// Update is called once per frame
	void Update ()
	{
		if (device.GetPressDown (SteamVR_Controller.ButtonMask.Trigger))
		{
			Debug.Log ("Number " + trackedObject.index + " is alive!");
			device.TriggerHapticPulse ();
		}
	}
}
