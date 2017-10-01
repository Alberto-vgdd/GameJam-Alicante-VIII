using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DoorSwitchScript : MonoBehaviour 
{

	[Header("Doors Transform")]
	public Transform m_UpperDoor;
	public Transform m_LowerDoor;

	[Header("Door Parameters")]
	public float m_DoorAngle;
	public float m_DoorHeight;
	public float m_DoorSmoothTime;

	[Header("Click Position Script")]
	public ClickPositionScript m_ClickPositionScript;

	[Header("Door Audio Source")]
	public AudioSource m_DoorAudioSource;


	private bool m_DoorOpening;

	private Vector3 m_LowerDoorCurrentVelocity;
	private Vector3 m_UpperDoorCurrentVelocity;

	private Vector3 m_UpperDoorOpenedPosition;
	private Vector3 m_LowerDoorOpenedPosition;

	void Start()
	{
		m_UpperDoorOpenedPosition = m_UpperDoor.position + Quaternion.AngleAxis(m_DoorAngle,Vector3.forward)*Vector3.up*m_DoorHeight;
		m_LowerDoorOpenedPosition = m_LowerDoor.position + Quaternion.AngleAxis(m_DoorAngle,Vector3.forward)* Vector3.up*-m_DoorHeight;

	}

	void Update () 
	{
		if (m_ClickPositionScript.HasBeenClicked() && !m_DoorOpening)
		{
			m_DoorAudioSource.Play();
			m_DoorOpening = true;
		}

		if (m_DoorOpening)
		{
			m_UpperDoor.position = Vector3.SmoothDamp(m_UpperDoor.position,m_UpperDoorOpenedPosition,ref m_UpperDoorCurrentVelocity,m_DoorSmoothTime,100f);
			m_LowerDoor.position = Vector3.SmoothDamp(m_LowerDoor.position,m_LowerDoorOpenedPosition,ref m_LowerDoorCurrentVelocity,m_DoorSmoothTime,100f);
		}	
	}



}
