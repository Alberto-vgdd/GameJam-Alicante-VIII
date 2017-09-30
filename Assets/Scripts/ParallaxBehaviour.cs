using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ParallaxBehaviour : MonoBehaviour {

	GameObject player;

	public GameObject[] parallaxObjects;

	// Use this for initialization
	void Start () {
		player =  GameObject.FindGameObjectWithTag ("Player");
	}
	
	// Update is called once per frame
	void Update () {

		parallaxObjects [0].transform.position = new Vector3 (player.transform.position.x * 0.2f, 0f, 30f);
		parallaxObjects [1].transform.position = new Vector3 (player.transform.position.x * 0.5f, 0f, 30f);
		
	}
}
