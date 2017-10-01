using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerMovementScript : MonoBehaviour 
{
	[Header("Player Components")]
	public Transform m_PlayerTransform;
	public Rigidbody2D m_PlayerRigidbody2D;
	public CapsuleCollider2D m_PlayerCapsuleCollider2D;
	public SpriteRenderer m_PlayerSpriteRendered;

	[Header("Animation Components")]
	public Transform m_SpriteTransform;
	public Animator m_PlayerAnimatorController;

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
	public float m_ClickTime;

	[Header("Health Parameters")]
	public float m_RecoveryTime;
	public float m_TimeBeforeDeath;
	
	[Header("Game Over Canvas")]
	public GameObject m_GameOverCanvas;

	[Header("Player Audio")]
	public AudioSource m_PlayerAudioSource;
	public AudioSource m_BallAudioSource;
	public AudioClip m_PlayerJumpSound;
	public AudioClip m_PlayerRushAndDeathSound;
	public AudioClip m_PlayerPunchSound;
	public AudioClip m_PlayerHitSound;
	public AudioClip m_BallLevitateSound;
	public AudioClip m_PlayerWalkSound;
	public AudioClip m_PlayerBallReleaseSound;
	
	[Header("Player Attack")]
	public float m_AttackDuration;
	private float m_AttackTimer;
	private  bool m_PlayerAttacking;

    public ParticleSystem dustParticles;

	



	// Player Movement Inputs
	private float m_HorizontalInput;
	private bool m_JumpInput;
	private Vector3 m_MousePositionInWorld;
	private bool m_ChangeStateInput;
	private float m_OldHorizontalInput;
	
	

	// Click Inputs variables
	private Ray m_ScreenToWorldRay;
	private RaycastHit2D m_ScreenToWorldRaycastHitInfo;
	private bool m_BallInUse;
	private bool m_ClickInput;
	private float m_ClickTimer;
	private Vector3 m_ClickPosition;



	// Player Grounded variables
	private Vector3 m_PlayerCenter;
	private bool m_PlayerGrounded;
	private bool m_OldPlayerGrounded;
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
	private float m_DeathTimer;
	private bool m_PlayerDeath;




	void Start () 
	{
		m_PlayerOnly = false;
		m_PlayerBallLinked = false;
		m_PlayerBallTogether = true;
	}
	
	void Update () 
	{
		// Update player Inputs
		m_HorizontalInput = Input.GetAxisRaw("Horizontal");
		m_JumpInput = Input.GetButton("Jump");
		m_MousePositionInWorld = m_SceneCamera.ScreenToWorldPoint(Input.mousePosition+Vector3.forward*m_SceneCamera.nearClipPlane);
		m_ChangeStateInput = Input.GetMouseButtonDown(1);
		m_ClickInput = Input.GetMouseButtonDown(0);


		// Change Player and Ball States
		ChangeState();

		// Player Attack
		Click();




		// Timers
		if(m_PlayerOnly)
		{
			m_DeathTimer += Time.deltaTime;
		
			if (m_DeathTimer >= m_TimeBeforeDeath)
			{
				m_GameOverCanvas.SetActive(true);
				m_PlayerDeath = true;
			}
		}

		if (m_PlayerAttacking)
		{
			m_AttackTimer += Time.deltaTime;

			if (m_AttackTimer >= m_AttackDuration)
			{
				m_PlayerAttacking = false;
			}
		}





		// Update animations
		m_PlayerAnimatorController.SetBool("Together",m_PlayerBallTogether);

		// Flip sprites
		if (!m_PlayerDeath && !m_PlayerAttacking)
		{
			if (m_HorizontalInput > 0  && m_SpriteTransform.localScale.x < 0)
			{
				m_SpriteTransform.localScale = new Vector3(1,1,1);
				m_SpriteTransform.localPosition -= Vector3.right*m_PlayerCapsuleCollider2D.size.x;  
			}
			if (m_HorizontalInput < 0 && m_SpriteTransform.localScale.x > 0)
			{	
				m_SpriteTransform.localScale = new Vector3(-1,1,1); 
				m_SpriteTransform.localPosition += Vector3.right*m_PlayerCapsuleCollider2D.size.x;  
			}
		}

		// Set Walk Animation
		if ( m_OldHorizontalInput == 0 && m_HorizontalInput != 0 )
		{
			m_PlayerAnimatorController.SetBool("Walking",true);
		}
		else if (m_OldHorizontalInput != 0 && m_HorizontalInput == 0)
		{
			m_PlayerAnimatorController.SetBool("Walking",false);
		}

		// Set Grounded Animation
		m_PlayerAnimatorController.SetBool("Grounded",m_PlayerGrounded);

		// Set Death animation
		m_PlayerAnimatorController.SetBool("Death", m_PlayerDeath);
		
	
		m_OldHorizontalInput = m_HorizontalInput;
	}

	void FixedUpdate()
	{
		
		MovePlayer();
		MoveBall();
		
	}


	void MovePlayer()
	{
		m_PlayerCenter = m_PlayerTransform.position + new Vector3(m_PlayerCapsuleCollider2D.offset.x,m_PlayerCapsuleCollider2D.offset.y);
		m_RaycastHit2DArray = Physics2D.CapsuleCastAll(m_PlayerCenter,m_PlayerCapsuleCollider2D.size,m_PlayerCapsuleCollider2D.direction,m_PlayerTransform.eulerAngles.z,-Vector2.up,m_RayToGroundDistance, (1 << LayerMask.NameToLayer("Scenario")));
		
		// Check if the player is grounded (Assume the player is sliding by default).
		m_PlayerGrounded = m_PlayerSliding = (m_RaycastHit2DArray.Length > 0) ? true: false;

		if (!m_OldPlayerGrounded && m_PlayerGrounded)
		{
            dustParticles.transform.position = new Vector2(transform.position.x + 1.02f, transform.position.y - 0.2f);
            dustParticles.Play();
        }


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
			m_RaycastHit2DArray = Physics2D.CapsuleCastAll(m_PlayerCenter,m_PlayerCapsuleCollider2D.size,m_PlayerCapsuleCollider2D.direction,m_PlayerTransform.eulerAngles.z,Vector2.right*m_HorizontalInput,m_FastMovementpeed*Time.fixedDeltaTime, (1 << LayerMask.NameToLayer("Scenario")));

			foreach(RaycastHit2D hit in m_RaycastHit2DArray)
			{
				if (Vector2.Angle(Vector2.up, hit.normal)>m_MaxSlopeAngle)
				{
					m_HorizontalInput = 0;
				}
			}
		}
		
		// Set Walk Sound
		if ( m_PlayerRigidbody2D.velocity.x == 0 && m_HorizontalInput != 0 )
		{

			if (m_PlayerGrounded && !m_PlayerAttacking && !m_PlayerDeath) 
			{
				m_PlayerAudioSource.clip = m_PlayerWalkSound;
				m_PlayerAudioSource.loop = true;
				m_PlayerAudioSource.Play();
			}
			
		}
		else if (m_PlayerRigidbody2D.velocity.x != 0 && m_HorizontalInput == 0)
		{

			if (m_PlayerAudioSource.clip == m_PlayerWalkSound && !m_PlayerAttacking && !m_PlayerDeath )
			{
				m_PlayerAudioSource.Stop();
			}
		}

		
		// Move the player
		if (!m_PlayerDeath && !m_PlayerAttacking)
		{
			if (m_PlayerBallTogether)
			{
				m_PlayerRigidbody2D.velocity = Vector2.right*m_MovementSpeed*m_HorizontalInput + Vector2.up*m_PlayerRigidbody2D.velocity.y;
			}
			else
			{
				m_PlayerRigidbody2D.velocity = Vector2.right*m_FastMovementpeed*m_HorizontalInput + Vector2.up*m_PlayerRigidbody2D.velocity.y;
			}
		}
		
		
		

		// Adapt player movement to the ground
		if (m_PlayerGrounded && m_FloorNormal != Vector2.up) {	m_PlayerRigidbody2D.velocity = Vector3.ProjectOnPlane(m_PlayerRigidbody2D.velocity,m_FloorNormal); }
		// Apply Sliding to the player
		if (m_PlayerSliding){ 	m_PlayerRigidbody2D.velocity = new Vector2(m_PlayerRigidbody2D.velocity.x, Mathf.Min(m_PlayerRigidbody2D.velocity.y,-m_FastMovementpeed)); 	}


		//Manage the jump
		if (m_PlayerGrounded && m_JumpInput && (!m_PlayerDeath && !m_PlayerAttacking))
		{
			if (!m_PlayerSliding)
			{
				m_PlayerAudioSource.clip = m_PlayerJumpSound;
				m_PlayerAudioSource.loop = false;
				m_PlayerAudioSource.Play();

				m_PlayerRigidbody2D.velocity = Vector2.right*m_PlayerRigidbody2D.velocity.x+Vector2.up*m_JumpSpeed;

                dustParticles.transform.position = new Vector2(transform.position.x + 1.02f, transform.position.y - 0.2f);
                dustParticles.Play();
			}
		}
		
		// Apply movement in the opposite direction of the damage origin. Deny any horizontal input
		if (m_PlayerDamaged)
		{
			if (Mathf.Sign(m_HorizontalInput) != Mathf.Sign(m_DamageDirection.x))
			{
				m_PlayerRigidbody2D.velocity = m_MovementSpeed*m_DamageDirection + Vector2.up*m_PlayerRigidbody2D.velocity.y;
			}
				
			m_PlayerSpriteRendered.enabled = !m_PlayerSpriteRendered.enabled;

			m_RecoveryTimer += Time.fixedDeltaTime;
			if (m_RecoveryTimer >= m_RecoveryTime)
			{
				m_PlayerSpriteRendered.enabled = true;
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
				if (m_BallInUse)
				{
					m_TargetBallPosition = m_ClickPosition;
					m_ClickTimer += Time.fixedDeltaTime;

					if (m_ClickTimer >= m_ClickTime)
					{
						m_BallInUse = false;
					}
				}
				else
				{
					float mouseDistance = Mathf.Min( Vector3.Distance(m_MousePositionInWorld,m_PlayerCenter), m_MaximumBallDistance);
					Vector3 directionToMouse = (m_MousePositionInWorld-m_PlayerCenter).normalized;

					m_TargetBallPosition = m_PlayerCenter+directionToMouse*mouseDistance ;
				}
				
			}
			else if (m_PlayerBallTogether)
			{
				m_TargetBallPosition = m_PlayerCenter;

			}

			m_BallRigidbody2D.MovePosition(Vector2.SmoothDamp(m_BallRigidbody2D.position,m_TargetBallPosition,ref m_CurrentBallVelocity,m_BallSmooth,100f,Time.fixedDeltaTime));
			

			// LA GITANADA MOTHER
			if (m_PlayerBallTogether && Vector3.Distance(m_PlayerCenter,m_BallTransform.position) < 4)
			{
				m_BallTransform.gameObject.GetComponent<SpriteRenderer>().enabled = false;
			}
			else
			{
				m_BallTransform.gameObject.GetComponent<SpriteRenderer>().enabled = true;
			}
		}
		
		
		//This is for the Aurenigga
		m_OldPlayerGrounded = m_PlayerGrounded;
	}
	
	void ChangeState()
	{
		if (m_ChangeStateInput && !m_PlayerAttacking)
		{
			if (m_PlayerBallTogether)
			{
				m_PlayerBallTogether = false;
				m_PlayerBallLinked = true;



				if (m_BallAudioSource.clip == m_PlayerBallReleaseSound)
				{
					m_BallAudioSource.Stop();
				}

				m_BallAudioSource.clip = m_BallLevitateSound;
				m_BallAudioSource.loop = true;
				m_BallAudioSource.Play();
			}
			else if (m_PlayerBallLinked)
			{
				m_PlayerBallTogether = true;
				m_PlayerBallLinked = false;



				if (m_BallAudioSource.clip == m_BallLevitateSound)
				{
					m_BallAudioSource.Stop();
				}

				m_BallAudioSource.clip = m_PlayerBallReleaseSound;
				m_BallAudioSource.loop = false;
				m_BallAudioSource.Play();
				
			}
			
		}
	}

	void Click()
	{
		if (m_ClickInput)
		{
			if (m_PlayerBallLinked)
			{
				
				m_ScreenToWorldRay = m_SceneCamera.ScreenPointToRay(Input.mousePosition);
				m_ScreenToWorldRaycastHitInfo = Physics2D.Raycast(m_ScreenToWorldRay.origin,m_ScreenToWorldRay.direction,1000f,(1 << LayerMask.NameToLayer("Clickable")));

				if (m_ScreenToWorldRaycastHitInfo.transform != null)
				{
					m_BallInUse = true;
					m_ClickTimer = 0f;
					m_ClickPosition = m_ScreenToWorldRaycastHitInfo.transform.GetComponent<ClickPositionScript>().GetClickPosition();
				}
			}
			else if(m_PlayerBallTogether)
			{
				if (!m_PlayerAttacking && m_PlayerGrounded)
				{
					m_PlayerAttacking = true;
					m_AttackTimer = 0f;
					m_PlayerAnimatorController.SetTrigger("Attack");

					m_PlayerAudioSource.clip = m_PlayerPunchSound;
					m_PlayerAudioSource.loop = false;
					m_PlayerAudioSource.Play();
				}
			}
		}
	}

	void ApplyDamage()
	{
		m_RecoveryTimer = 0;
		m_PlayerDamaged = true;

		m_PlayerAudioSource.clip = m_PlayerHitSound;
		m_PlayerAudioSource.loop = false;
		m_PlayerAudioSource.Play();

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

		m_DeathTimer = 0;

		m_BallAudioSource.clip = m_PlayerRushAndDeathSound;
		m_BallAudioSource.loop = false;
		m_BallAudioSource.Play();
	}

	void RetrieveBall()
	{
		m_BallCollider2D.isTrigger = true;

		m_PlayerBallTogether = true;
		m_PlayerBallLinked = false;
		m_PlayerOnly = false;

		if (m_BallAudioSource.clip == m_PlayerRushAndDeathSound)
		{
			m_BallAudioSource.Stop();	
		}
	}





	void OnCollisionEnter2D (Collision2D collision)
	{
		if (collision.gameObject.CompareTag("Enemy") && !m_PlayerDamaged)
		{
			ApplyDamage();
			m_DamageDirection = (m_PlayerCenter.x>= collision.transform.position.x)? Vector2.right: -Vector2.right;
			
		}
		if (collision.gameObject.CompareTag("Ball"))
		{
			RetrieveBall();
		}
		

	}
}

	