using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameOver : MonoBehaviour {

	Animation m_Animation;
	float timer;
	float maxtimer;
	// Use this for initialization
	void Awake () 
	{
		m_Animation = GetComponent<Animation>();
		m_Animation.Play();
		timer = 0f;
		maxtimer = 4f;
	}
	
	 void Update()
	{
		timer += Time.deltaTime;
		if (timer > maxtimer)
		{
			if(Input.anyKey) UnityEngine.SceneManagement.SceneManager.LoadScene(0);
		}

	}

}