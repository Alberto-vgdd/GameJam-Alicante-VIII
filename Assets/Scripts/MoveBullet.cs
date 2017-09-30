using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MoveBullet : MonoBehaviour {

	public float m_BulletSpeed;
	// Use this for initialization
	void Awake()
	{
		GetComponent<Rigidbody2D>().velocity = Vector2.right * m_BulletSpeed;
	}
	
	void OnTriggerEnter2D(Collider2D coll){
		if(coll.gameObject.tag == "Ball"){
			Destroy(this.gameObject);
		}
	}
}
