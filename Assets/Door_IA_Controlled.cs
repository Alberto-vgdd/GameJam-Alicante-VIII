using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Door_IA_Controlled : MonoBehaviour {

    [Header("Doors Transform")]
    public Transform m_UpperDoor;
    public Transform m_LowerDoor;

    [Header("Door Parameters")]
    public float m_DoorAngle;
    public float m_DoorHeight;
    public float m_DoorSmoothTime;

    private bool m_DoorOpening;
    private bool m_DoorClosing;

    private Vector3 m_LowerDoorCurrentVelocity;
    private Vector3 m_UpperDoorCurrentVelocity;

    private Vector3 m_UpperDoorOpenedPosition;
    private Vector3 m_LowerDoorOpenedPosition;

    private Vector3 m_UpperDoorClosedPosition;
    private Vector3 m_LowerDoorClosedPosition;

    void Start()
    {
        m_UpperDoorOpenedPosition = m_UpperDoor.position + Quaternion.AngleAxis(m_DoorAngle, Vector3.forward) * Vector3.up * m_DoorHeight;
        m_LowerDoorOpenedPosition = m_LowerDoor.position + Quaternion.AngleAxis(m_DoorAngle, Vector3.forward) * Vector3.up * -m_DoorHeight;

        m_UpperDoorClosedPosition = m_UpperDoor.position;
        m_LowerDoorClosedPosition = m_LowerDoor.position;
    }

    void Update()
    {
        if (m_DoorOpening)
        {
            m_UpperDoor.position = Vector3.SmoothDamp(m_UpperDoor.position, m_UpperDoorOpenedPosition, ref m_UpperDoorCurrentVelocity, m_DoorSmoothTime, 100f);
            m_LowerDoor.position = Vector3.SmoothDamp(m_LowerDoor.position, m_LowerDoorOpenedPosition, ref m_LowerDoorCurrentVelocity, m_DoorSmoothTime, 100f);
            if (m_UpperDoor.position == m_UpperDoorOpenedPosition && m_LowerDoor.position == m_LowerDoorOpenedPosition) m_DoorOpening = false;
        }
        if (m_DoorClosing)
        {
            m_UpperDoor.position = Vector3.SmoothDamp(m_UpperDoor.position, m_UpperDoorClosedPosition, ref m_UpperDoorCurrentVelocity, m_DoorSmoothTime, 100f);
            m_LowerDoor.position = Vector3.SmoothDamp(m_LowerDoor.position, m_LowerDoorClosedPosition, ref m_LowerDoorCurrentVelocity, m_DoorSmoothTime, 100f);
            if (m_UpperDoor.position == m_UpperDoorClosedPosition && m_LowerDoor.position == m_LowerDoorClosedPosition) m_DoorClosing = false;
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if(collision.gameObject.tag == "enemy")
        {
            m_DoorOpening = true;
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.gameObject.tag == "enemy")
        {
            m_DoorClosing = true;
        }
    }
}
