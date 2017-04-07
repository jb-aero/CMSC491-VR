using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class CameraLimits
{
	public float upAngle, downAngle, upDownSpeed, leftRightSpeed, zoomSpeed, nearZoom, farZoom;
}

public class CameraControl : MonoBehaviour {

	public Transform viewer;
	public Transform cameraMount;

	public CameraLimits limits;
	private float pitch, yaw, zoom;

	void Start() 
	{
		pitch = 0;
		yaw = 0;
		zoom = cameraMount.position.x;
	}
	
	// Update is called once per frame
	void Update () {

		float turnUD = Input.GetAxis ("Vertical");
		float turnLR = Input.GetAxis ("Horizontal");
		float zoomIO = Input.GetAxis ("Zoom");

		yaw -= limits.leftRightSpeed * turnLR;
		pitch += limits.upDownSpeed * turnUD;
		zoom -= limits.zoomSpeed * zoomIO;

		pitch = Mathf.Clamp (pitch, -limits.upAngle, limits.downAngle);
		zoom = Mathf.Clamp (zoom, limits.nearZoom, limits.farZoom);

		viewer.rotation = Quaternion.Euler (0, yaw, pitch);
		cameraMount.localPosition = new Vector3 (zoom, 0, 0);
	}
}
