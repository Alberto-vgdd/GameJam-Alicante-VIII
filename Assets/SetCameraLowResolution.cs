using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SetCameraLowResolution : MonoBehaviour {

	float m_CameraWidth = 960f;
	float m_CameraHeight = 540f;
	Camera camera;
	// Use this for initialization
	void Start () {
		camera = GetComponent<Camera>();
		// Camera has fixed width and height on every screen solution


		Debug.Log(camera.pixelWidth + "," + camera.pixelHeight);
	}
	
	// Update is called once per frame
	void Update () {
		
	}
}
