using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HelixRotateScript : MonoBehaviour {


    float rotationZ = 90f;
    public Transform helix;

    [Header("Click Position Script")]
    public ClickPositionScript m_ClickPositionScript;


    private bool m_HelixActive;
    [Header("Parametres Helix")]

    public float zspeedRotation;



    void Start()
    {

    }

    void Update()
    {
        
        if (m_ClickPositionScript.HasBeenClicked())
        {

            m_HelixActive = true;
            
           
        }

        if (m_HelixActive)
        {
            rotationZ += zspeedRotation*Time.deltaTime;
            helix.localEulerAngles = new Vector3(0,0,rotationZ);

        }
    }
}