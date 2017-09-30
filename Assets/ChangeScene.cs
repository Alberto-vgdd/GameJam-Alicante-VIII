using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ChangeScene : MonoBehaviour {

	// Update is called once per frame
	bool m_CanChange;

	void Start()
	{

		Invoke("CanChange", 2f);
	}



	void CanChange()
	{
		m_CanChange = true;
	}
	

	void Update () 
	{
		if(m_CanChange)
			if(Input.anyKey)
				UnityEngine.SceneManagement.SceneManager.LoadScene(1);		
	
	}

}
