using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameOver : MonoBehaviour {

	Animation m_Animation;
	// Use this for initialization
	void Start () {
		m_Animation = GetComponent<Animation>();
	}
	
	public void TriggerGameOver(){

		m_Animation.Play();
		if(Input.anyKey) UnityEngine.SceneManagement.SceneManager.LoadScene(0);

	}

}