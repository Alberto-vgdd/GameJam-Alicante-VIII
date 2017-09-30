using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MoveBullet : MonoBehaviour {

	public float m_BulletSpeed;
	public Transform m_PlayerTransform;
	public string direction;
	// Use this for initialization
	void Awake()
	{
		m_PlayerTransform = GameObject.Find("Player").transform;
		if(m_PlayerTransform.position.x < transform.position.x){
			m_BulletSpeed = -m_BulletSpeed;
		}else{

		}
		GetComponent<Rigidbody2D>().velocity = Vector2.right * m_BulletSpeed;
	}

	
	void OnTriggerEnter2D(Collider2D coll){
		if(coll.gameObject.tag == "Ball" || coll.gameObject.tag == "Player"){
			Destroy(this.gameObject);
		}
	}
}
