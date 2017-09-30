using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MeleeEnemyMovementScript : MonoBehaviour {

	Rigidbody2D m_Rigidbody2D;

	[Header("Aggro")]
	public float m_AggroDistance;
	public float m_MovementSpeed;
	public Transform m_PlayerTransform;
	public bool m_ChasingPlayer;

	[Header("Patrolling behavior")]
	public bool m_Patrolling;
	public bool m_ReturningToInitialPosition;
	public Vector3 m_InitialPosition;
	public Vector3 m_LeftPatrolLimit;
	public Vector3 m_RightPatrolLimit;
	public float m_PatrolDistance;

	public bool m_PatrollingLeft;
	public bool m_PatrollingRight;


	// Use this for initialization
	void Start () 
	{
		m_Rigidbody2D = GetComponent<Rigidbody2D>();

		// To get the patrol range
		m_InitialPosition = transform.position;
		m_LeftPatrolLimit = m_InitialPosition - new Vector3(m_PatrolDistance, 0, 0);
		m_RightPatrolLimit = m_InitialPosition + new Vector3(m_PatrolDistance, 0, 0);

		//Move it to the left by default
		m_PatrollingLeft = true;
	}

	
	// Update is called once per frame
	void Update () 
	{
		CheckAggro();

	}



	void CheckAggro()
	{
		//Inside the aggro range
		if(Vector3.Distance(transform.position, m_PlayerTransform.position) < m_AggroDistance){
			m_Patrolling = false;
 			m_ChasingPlayer = true;
		//If outside the aggro range
		}else{
			//If we were chasing the player before
			if(m_ChasingPlayer){
				//We return to the initial position and cease the chase
				m_ReturningToInitialPosition = true;
				m_ChasingPlayer = false;
			}
		}

	}



	void FixedUpdate()
	{
		Move();
	}


	void Move()
	{
		//State: chasing the player
		if(m_ChasingPlayer)
		{
			//Move towards its direction
			m_Rigidbody2D.velocity = Vector2.right * m_MovementSpeed * Mathf.Sign((m_PlayerTransform.position - transform.position).x);
		}
		else if(m_ReturningToInitialPosition)
		{
			//If we haven't reached the initial position yet
			if(!Mathf.Approximately(Mathf.Round(transform.position.x), Mathf.Round(m_InitialPosition.x)))
			{
				//If it's on the right of the actual position
				if(transform.position.x < m_InitialPosition.x)
				{
					m_Rigidbody2D.velocity = Vector2.right * m_MovementSpeed;
				//Otherwise
				}else{
					m_Rigidbody2D.velocity = Vector2.right * -m_MovementSpeed;
				}
			}
			//Otherwise we have reached it
			else
			{
				Debug.Log("Original position");
				m_ReturningToInitialPosition = false;
				m_Patrolling = true;
				m_Rigidbody2D.velocity = Vector2.zero;
			}
			
		}

		// == Patrolling
		else
		{
			//Move to the left
			if(m_PatrollingLeft){
				m_Rigidbody2D.velocity = Vector2.right * -m_MovementSpeed;
			//Move to the right
			}else{
				m_Rigidbody2D.velocity = Vector2.right * m_MovementSpeed;
			}

			//If we have moved beyond the patrol limit, reverse the movement direction
			if(transform.position.x < m_LeftPatrolLimit.x){
				m_PatrollingLeft = false;
				m_PatrollingRight = true;
			}
			//Same here
			if(transform.position.x > m_RightPatrolLimit.x){
				m_PatrollingRight = false;
				m_PatrollingLeft = true;
			}
			
		}

	
		}

}

