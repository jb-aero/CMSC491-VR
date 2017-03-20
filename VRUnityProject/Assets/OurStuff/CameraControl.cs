using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class CameraLimits
{
	public float upAngle, downAngle, upDownSpeed, leftRightSpeed, zoomSpeed, nearZoom, farZoom;
}

public class CameraControl : MonoBehaviour {

	public Transform cameraMount;
	public Transform camera;

	public CameraLimits limits;
	private float pitch, yaw, zoom;

	void Start() 
	{
		pitch = 0;
		yaw = 0;
		zoom = camera.position.z;
	}
	
	// Update is called once per frame
	void Update () {

		float turnUD = Input.GetAxis ("Vertical");
		float turnLR = Input.GetAxis ("Horizontal");

		yaw -= limits.leftRightSpeed * turnLR;
		pitch += limits.upDownSpeed * turnUD;

		pitch = Mathf.Clamp (pitch, -limits.upAngle, limits.downAngle);

		cameraMount.rotation = Quaternion.Euler (pitch, yaw, 0);

		if (Input.GetKey ("page up"))
		{
			zoom += limits.zoomSpeed;
		}
		else if (Input.GetKey ("page down"))
		{
			zoom -= limits.zoomSpeed;
		}

		zoom = Mathf.Clamp (zoom, -limits.farZoom, -limits.nearZoom);

		camera.localPosition = new Vector3 (0, 0, zoom);
	}
}
