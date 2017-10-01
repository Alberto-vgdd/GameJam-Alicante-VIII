using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraMovementScript : MonoBehaviour
 {
	[Header("Player and Camera Transforms")]
	public Transform m_CameraTransform;
	public Transform m_PlayerTransfrom;
	public CapsuleCollider2D m_PlayerCapsuleCollider2D;

	[Header("Camera Smooth Parameters")]
	public float m_HorizontalSmoothTime;
	public float m_VerticalSmoothTime;
	public float m_VerticalOffset;

	private float m_HorizontalCurrentVelocity;
	private float m_VerticalCurrentVelocity;

	private Vector3 m_TargetPosition;
	


	// Use this for initialization
	void Start () 
	{
		m_CameraTransform.position = m_PlayerTransfrom.position+new Vector3(m_PlayerCapsuleCollider2D.offset.x,m_PlayerCapsuleCollider2D.offset.y);
	}
	
	// Update is called once per frame
	void Update ()
	{
		m_TargetPosition = m_PlayerTransfrom.position+new Vector3(m_PlayerCapsuleCollider2D.offset.x,m_PlayerCapsuleCollider2D.offset.y+m_VerticalOffset);
		float x = Mathf.SmoothDamp(m_CameraTransform.position.x,m_TargetPosition.x,ref m_HorizontalCurrentVelocity, m_HorizontalSmoothTime,100f);
		float y = Mathf.SmoothDamp(m_CameraTransform.position.y,m_TargetPosition.y,ref m_VerticalCurrentVelocity, m_VerticalSmoothTime,100f);

		m_CameraTransform.position = new Vector3(x,y,-10f);
	}
}
