using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraMovementScript : MonoBehaviour
 {
	[Header("Player and Camera Transforms")]
	public Transform m_CameraTransform;
	public Transform m_PlayerTransfrom;

	[Header("Camera Smooth Parameters")]
	public float m_HorizontalSmoothTime;
	public float m_VerticalSmoothTime;

	private float m_HorizontalCurrentVelocity;
	private float m_VerticalCurrentVelocity;


	// Use this for initialization
	void Start () 
	{
		m_CameraTransform.position = m_PlayerTransfrom.position;
	}
	
	// Update is called once per frame
	void Update ()
	{
		float x = Mathf.SmoothDamp(m_CameraTransform.position.x,m_PlayerTransfrom.position.x,ref m_HorizontalCurrentVelocity, m_HorizontalSmoothTime,100f);
		float y = Mathf.SmoothDamp(m_CameraTransform.position.y,m_PlayerTransfrom.position.y,ref m_VerticalCurrentVelocity, m_VerticalSmoothTime,100f);

		m_CameraTransform.position = new Vector3(x,y,-10f);
	}
}
