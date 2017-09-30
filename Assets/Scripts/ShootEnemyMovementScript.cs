using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ShootEnemyMovementScript : MonoBehaviour {

	Rigidbody2D m_Rigidbody2D;

	public GameObject m_BulletObject;
	public Transform m_PlayerTransform;
	public string direction;

	public float m_AggroDistance;
	public bool m_InRange;

	public Transform m_InstantiationPosition;


	float frames;
	public float m_TimeBetweenShots;

	// Use this for initialization
	void Start () {
		
		m_Rigidbody2D = GetComponent<Rigidbody2D>();
	}

	void Update () 
	{
		CheckDistance();
		Shoot();
	}

	void CheckDistance(){

		if(m_PlayerTransform.position.x > transform.position.x){
			direction = "right";
			transform.localScale = new Vector3(1, transform.localScale.y, transform.localScale.z);
		}else{
			direction = "left";
			transform.localScale = new Vector3(-1, transform.localScale.y, transform.localScale.z);
		}

		if(Vector3.Distance(transform.position, m_PlayerTransform.position) < m_AggroDistance)
			m_InRange = true;
		else
			m_InRange = false;
		
	}

	void Shoot()
	{
		if(m_InRange){
			frames++;
			if(frames * Time.deltaTime > m_TimeBetweenShots){
				GameObject m_NewBullet = Instantiate(m_BulletObject, m_InstantiationPosition.position, new Quaternion(0,0,0,0));
				m_NewBullet.GetComponent<MoveBullet>().direction = direction;
				frames = 0;
			}
		}

	}

	

}
