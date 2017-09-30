using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ShootEnemyMovementScript : MonoBehaviour {

	Rigidbody2D m_Rigidbody2D;

	public GameObject m_BulletObject;
	public Transform m_PlayerTransform;


	public float m_AggroDistance;
	public bool m_InRange;

	public Vector3 m_InstantiationPosition;


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
					Instantiate(m_BulletObject, transform.position + m_InstantiationPosition, new Quaternion(0,0,0,0));
				frames = 0;
			}
		}

	}

	

}
