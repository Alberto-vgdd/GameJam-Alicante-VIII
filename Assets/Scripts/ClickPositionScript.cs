using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ClickPositionScript : MonoBehaviour 
{
	[Header("Ball Holder Transform")]
	public Transform m_BallHolderTransform;
	

	private bool m_Clicked;
	void Start()
	{
		m_Clicked = false;
	}

	public Vector3 GetClickPosition()
	{
		m_Clicked = true;
		return m_BallHolderTransform.position;
	}

	public bool HasBeenClicked()
	{
		return m_Clicked;
	}
}
