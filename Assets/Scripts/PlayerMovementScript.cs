using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerMovementScript : MonoBehaviour 
{
	[Header("Player Components")]
	public Transform m_PlayerTransform;
	public Rigidbody2D m_PlayerRigidbody2D;
	public CapsuleCollider2D m_PlayerCapsuleCollider2D;

	[Header("Ball Components")]
	public Transform m_BallTransform;
	public Rigidbody2D m_BallRigidbody2D;
	public Collider2D m_BallCollider2D;
	
	[Header("Scene Camera")]
	public Camera m_SceneCamera;

	[Header("Player Movement Parameters")]
	public float m_MovementSpeed;
	public float m_FastMovementpeed;
	public float m_JumpSpeed;
	public float m_RayToGroundDistance;
	public float m_MaxSlopeAngle;
	public float m_GravityScaleJetpack;
	public float m_GravityScaleJump;

	[Header("Ball Movement Parameters")]
	public float m_BallSmooth;
	public float m_MaximumBallDistance;

	[Header("Health Parameters")]
	public float m_RecoveryTime;

	



	// Player Movement Inputs
	private float m_HorizontalInput;
	private bool m_JumpInput;
	private Vector3 m_MousePositionInWorld;
	private bool m_ChangeState;

	// Player Grounded variables
	private bool m_PlayerGrounded;
	private bool m_PlayerSliding;
	RaycastHit2D[] m_RaycastHit2DArray;
	private Vector2 m_FloorNormal;


	// Ball Movement Variables
	private Vector2 m_TargetBallPosition;
	private Vector2 m_CurrentBallVelocity;

	// Player & Ball States
	private bool m_PlayerBallTogether;
	private bool m_PlayerBallLinked;
	private bool m_PlayerOnly;

	// Player Heatlh variables
	private bool m_PlayerDamaged;
	private float m_RecoveryTimer;
	private Vector2 m_DamageDirection;

	void Start () 
	{
		m_PlayerOnly = false;
		m_PlayerBallLinked = true;
		m_PlayerBallTogether = false;
	}
	
	void Update () 
	{
		// Update player Inputs
		m_HorizontalInput = Input.GetAxisRaw("Horizontal");
		m_JumpInput = Input.GetButton("Jump");
		m_MousePositionInWorld = m_SceneCamera.ScreenToWorldPoint(Input.mousePosition+Vector3.forward*m_SceneCamera.nearClipPlane);
		m_ChangeState = Input.GetMouseButtonDown(1);


		// Change Player and Ball States
		ChangeState();
	}

	void FixedUpdate()
	{
		MovePlayer();
		MoveBall();
	}


	void MovePlayer()
	{
		// Cast a capsule below the player
		m_RaycastHit2DArray = Physics2D.CapsuleCastAll(new Vector2(m_PlayerTransform.position.x,m_PlayerTransform.position.y)+m_PlayerCapsuleCollider2D.offset,m_PlayerCapsuleCollider2D.size,m_PlayerCapsuleCollider2D.direction,m_PlayerTransform.eulerAngles.z,-Vector2.up,m_RayToGroundDistance, (1 << LayerMask.NameToLayer("Scenario")));
		
		// Check if the player is grounded (Assume the player is sliding by default).
		m_PlayerGrounded = m_PlayerSliding = (m_RaycastHit2DArray.Length > 0) ? true: false;

		// Check if the player is sliding.
		foreach (RaycastHit2D hit in m_RaycastHit2DArray)
		{
			m_FloorNormal = hit.normal;

			if (Vector2.Angle(Vector2.up, hit.normal)<m_MaxSlopeAngle)
			{
				m_PlayerSliding = false;
				break;
			}
		}

		if (m_HorizontalInput != 0)
		{
			// Check if the player is trying to walk into a wall/ramp, and avoid the movement
			m_RaycastHit2DArray = Physics2D.CapsuleCastAll(new Vector2(m_PlayerTransform.position.x,m_PlayerTransform.position.y)+m_PlayerCapsuleCollider2D.offset,m_PlayerCapsuleCollider2D.size,m_PlayerCapsuleCollider2D.direction,m_PlayerTransform.eulerAngles.z,Vector2.right*m_HorizontalInput,m_FastMovementpeed*Time.fixedDeltaTime, (1 << LayerMask.NameToLayer("Scenario")));
			
			foreach(RaycastHit2D hit in m_RaycastHit2DArray)
			{
				if (Vector2.Angle(Vector2.up, hit.normal)>m_MaxSlopeAngle)
				{
					m_HorizontalInput = 0;
				}
			}
		}
		
		
		// Move the player
		if (m_PlayerBallTogether)
		{
			m_PlayerRigidbody2D.velocity = Vector2.right*m_MovementSpeed*m_HorizontalInput + Vector2.up*m_PlayerRigidbody2D.velocity.y;
		}
		else
		{
			m_PlayerRigidbody2D.velocity = Vector2.right*m_FastMovementpeed*m_HorizontalInput + Vector2.up*m_PlayerRigidbody2D.velocity.y;
		}
		
		

		// Adapt player movement to the ground
		if (m_PlayerGrounded && m_FloorNormal != Vector2.up) {	m_PlayerRigidbody2D.velocity = Vector3.ProjectOnPlane(m_PlayerRigidbody2D.velocity,m_FloorNormal); }
		// Apply Sliding to the player
		if (m_PlayerSliding){ 	m_PlayerRigidbody2D.velocity = new Vector2(m_PlayerRigidbody2D.velocity.x, Mathf.Min(m_PlayerRigidbody2D.velocity.y,-m_FastMovementpeed)); 	}


		//Manage the jump
		if (m_PlayerGrounded && m_JumpInput)
		{
			if (!m_PlayerSliding)
			{
				m_PlayerRigidbody2D.velocity = Vector2.right*m_PlayerRigidbody2D.velocity.x+Vector2.up*m_JumpSpeed;
			}
		}
		
		// Apply movement in the opposite direction of the damage origin. Deny any horizontal input
		if (m_PlayerDamaged)
		{
			if (Mathf.Sign(m_HorizontalInput) != Mathf.Sign(m_DamageDirection.x))
			{
				m_PlayerRigidbody2D.velocity = m_MovementSpeed*m_DamageDirection + Vector2.up*m_PlayerRigidbody2D.velocity.y;
			}
				

			m_RecoveryTimer += Time.fixedDeltaTime;
			if (m_RecoveryTimer >= m_RecoveryTime)
			{
				m_PlayerDamaged = false;
			}
	
		}
		

		// Finally, add the gravity force
		if (m_PlayerBallTogether)
		{
			m_PlayerRigidbody2D.AddForce(Physics2D.gravity*m_GravityScaleJetpack);
		}
		else
		{
			m_PlayerRigidbody2D.AddForce(Physics2D.gravity*m_GravityScaleJump);
		}
		
	}

	void MoveBall()
	{
		if (!m_PlayerOnly)
		{
			if (m_PlayerBallLinked)
			{
				float mouseDistance = Mathf.Min( Vector3.Distance(m_MousePositionInWorld,m_PlayerTransform.position), m_MaximumBallDistance);
				Vector3 directionToMouse = (m_MousePositionInWorld-m_PlayerTransform.position).normalized;

				m_TargetBallPosition = m_PlayerTransform.position+directionToMouse*mouseDistance;
			}
			else if (m_PlayerBallTogether)
			{
				m_TargetBallPosition = m_PlayerTransform.position;
			}

			m_BallRigidbody2D.MovePosition(Vector2.SmoothDamp(m_BallRigidbody2D.position,m_TargetBallPosition,ref m_CurrentBallVelocity,m_BallSmooth,100f,Time.fixedDeltaTime));
			
		}
		
		
		
	}
	
	void ChangeState()
	{
		if (m_ChangeState)
		{
			if (m_PlayerBallTogether)
			{
				m_PlayerBallTogether = false;
				m_PlayerBallLinked = true;
			}
			else if (m_PlayerBallLinked)
			{
				m_PlayerBallTogether = true;
				m_PlayerBallLinked = false;
			}
			
		}
	}

	void ApplyDamage()
	{
		m_RecoveryTimer = 0;
		m_PlayerDamaged = true;


		if (m_PlayerBallTogether)
		{
			m_PlayerBallTogether = false;
			m_PlayerBallLinked = true;
		}
		else if (m_PlayerBallLinked)
		{
			ReleaseBall();
		}

		
	}




	void ReleaseBall()
	{
		m_BallCollider2D.isTrigger = false;

		m_PlayerBallTogether = false;
		m_PlayerBallLinked = false;
		m_PlayerOnly = true;
	}

	void RetrieveBall()
	{
		m_BallCollider2D.isTrigger = true;

		m_PlayerBallTogether = true;
		m_PlayerBallLinked = false;
		m_PlayerOnly = false;
	}





	void OnCollisionEnter2D (Collision2D collision)
	{
		if (collision.gameObject.CompareTag("Enemy") && !m_PlayerDamaged)
		{
			ApplyDamage();
			m_DamageDirection = (m_PlayerTransform.position.x >= collision.transform.position.x)? Vector2.right: -Vector2.right;
			
		}
		if (collision.gameObject.CompareTag("Ball"))
		{
			RetrieveBall();
		}
		

	}
}

	
