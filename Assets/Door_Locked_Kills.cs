using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Door_Locked_Kills : MonoBehaviour {

    public MeleeEnemyMovementScript enemy1;
    public MeleeEnemyMovementScript enemy2;

    // [Header("Doors Transform")]
    public Transform m_UpperDoor;
    public Transform m_LowerDoor;

    [Header("Door Parameters")]
    public float m_DoorAngle;
    public float m_DoorHeight;
    public float m_DoorSmoothTime;


    private bool m_DoorOpening;

    private Vector3 m_LowerDoorCurrentVelocity;
    private Vector3 m_UpperDoorCurrentVelocity;

    private Vector3 m_UpperDoorOpenedPosition;
    private Vector3 m_LowerDoorOpenedPosition;

    void Start()
    {
        m_UpperDoorOpenedPosition = m_UpperDoor.position + Quaternion.AngleAxis(m_DoorAngle, Vector3.forward) * Vector3.up * m_DoorHeight;
        m_LowerDoorOpenedPosition = m_LowerDoor.position + Quaternion.AngleAxis(m_DoorAngle, Vector3.forward) * Vector3.up * -m_DoorHeight;

    }

    void Update()
    {
        if (enemy1.alive == false && enemy2.alive == false)
        {
            m_DoorOpening = true;
        }
        if (m_DoorOpening)
        {
            m_UpperDoor.position = Vector3.SmoothDamp(m_UpperDoor.position, m_UpperDoorOpenedPosition, ref m_UpperDoorCurrentVelocity, m_DoorSmoothTime, 100f);
            m_LowerDoor.position = Vector3.SmoothDamp(m_LowerDoor.position, m_LowerDoorOpenedPosition, ref m_LowerDoorCurrentVelocity, m_DoorSmoothTime, 100f);
        }
    }

}
