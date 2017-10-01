using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GoToFinalScene : MonoBehaviour {

	public GameObject m_Canvas;
	// Use this for initialization
	void Start () {
		
	}

	void Load(){
		UnityEngine.SceneManagement.SceneManager.LoadScene(2);
	}
	
	void OnTriggerEnter2D(Collider2D coll){

		if(coll.gameObject.tag == "Player"){
			m_Canvas.GetComponent<Animation>().Play();
			Invoke("Load", 2f);	
		}
		
	}
}
