using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlantShooting : MonoBehaviour {

    public float shootingInterval;
    public GameObject toxicOrb;
    public Transform shootingPos;

    Animation shootingAnimation;
    float secondsToShoot;
    
	// Use this for initialization
	void Start () {
        secondsToShoot = shootingInterval;
	}
	
	// Update is called once per frame
	void Update () {
        if (secondsToShoot > 0) secondsToShoot -= Time.deltaTime;
        else
        {
            Instantiate(toxicOrb, shootingPos.position,shootingPos.rotation);
            secondsToShoot = shootingInterval;
        }
	}
}
