using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerMovementScript : MonoBehaviour 
{
	[Header("Player Components")]
	public Transform m_PlayerTransform;
	public Rigidbody2D m_PlayerRigidbody2D;
	public CapsuleCollider2D m_PlayerCapsuleCollider2D;
	
	[Header("Scene Camera")]
	public Camera m_SceneCamera;

	[Header("Player Movement Parameters")]
	public float m_MovementSpeed;
	public float m_RunMultiplier;
	public float m_RayToGroundDistance;
	public float m_MaxSlopeAngle;



	// Player Movement Inputs
	private float m_HorizontalInput;

	// Player Grounded variables
	public bool m_PlayerGrounded;
	public bool m_PlayerSliding;
	RaycastHit2D[] m_RaycastHit2DArray;
	private Vector2 m_FloorNormal;


	void Start () 
	{
	}
	
	void Update () 
	{
		// Update player Inputs
		m_HorizontalInput = Input.GetAxisRaw("Horizontal");
	}

	void FixedUpdate()
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
			m_RaycastHit2DArray = Physics2D.CapsuleCastAll(new Vector2(m_PlayerTransform.position.x,m_PlayerTransform.position.y)+m_PlayerCapsuleCollider2D.offset,m_PlayerCapsuleCollider2D.size,m_PlayerCapsuleCollider2D.direction,m_PlayerTransform.eulerAngles.z,Vector2.right*m_HorizontalInput,m_MovementSpeed*Time.fixedDeltaTime, (1 << LayerMask.NameToLayer("Scenario")));
			
			foreach(RaycastHit2D hit in m_RaycastHit2DArray)
			{
				if (Vector2.Angle(Vector2.up, hit.normal)>m_MaxSlopeAngle)
				{
					m_HorizontalInput = 0;
				}
			}
		}
		


		// Move the player
		m_PlayerRigidbody2D.velocity = Vector2.right*m_MovementSpeed*m_HorizontalInput + Vector2.up*m_PlayerRigidbody2D.velocity.y;
		

		// Adapt player movement to the ground
		if (m_PlayerGrounded && m_FloorNormal != Vector2.up) {	m_PlayerRigidbody2D.velocity = Vector3.ProjectOnPlane(m_PlayerRigidbody2D.velocity,m_FloorNormal); }
		// Apply Sliding to the player
		if (m_PlayerSliding){ 	m_PlayerRigidbody2D.velocity = new Vector2(m_PlayerRigidbody2D.velocity.x, Mathf.Min(m_PlayerRigidbody2D.velocity.y,-2f*m_MovementSpeed)); 	}
		


		// Finally, add the gravity force
		m_PlayerRigidbody2D.AddForce(Physics2D.gravity);
	}





	//Depaluego
		// m_MousePositionInWorld = m_SceneCamera.ScreenToWorldPoint(Input.mousePosition+Vector3.forward*m_SceneCamera.nearClipPlane);
		// m_MousePositionInWorld.z = 0;

		// m_BallRigidbodies.MovePosition(Vector3.SmoothDamp(m_BallRigidbodies.position,m_MousePositionInWorld,ref m_BallVelocity,0.05f));
}
