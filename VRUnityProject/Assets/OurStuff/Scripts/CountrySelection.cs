using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CountrySelection : MonoBehaviour {

	private SteamVR_TrackedObject trackedObject;
	private SteamVR_Controller.Device device;
	private RaycastHit raycastHit;
	private GameObject gameObject = null;

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
			if(Physics.Raycast(transform.position, transform.forward, out raycastHit))
			{
				gameObject = raycastHit.collider.gameObject;

				Debug.Log ("Selected: " + gameObject.transform.root.name);
			}
		}
	}
}
