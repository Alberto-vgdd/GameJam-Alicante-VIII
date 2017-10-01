using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ShootEnemyMovementScript : MonoBehaviour {

    public bool alive = true;

	Rigidbody2D m_Rigidbody2D;

	public GameObject m_BulletObject;
	public Transform m_PlayerTransform;
	public string direction;

	public float m_AggroDistance;
	public bool m_InRange;

	public Transform m_InstantiationPosition;
    public float m_MovementSpeed;

    private Animator anim;

    float frames;
	public float m_TimeBetweenShots;

	// Use this for initialization
	void Start () {
		
		m_Rigidbody2D = GetComponent<Rigidbody2D>();
        anim = GetComponent<Animator>();
	}

	void Update () 
	{
        if (alive)
        {
            CheckDistance();
            Shoot();
        }
		
	}

	void CheckDistance(){

		if(m_PlayerTransform.position.x > transform.position.x){
			direction = "right";
			transform.localScale = new Vector3(1, transform.localScale.y, transform.localScale.z);
		}else{
			direction = "left";
			transform.localScale = new Vector3(-1, transform.localScale.y, transform.localScale.z);
		}

		if(Vector3.Distance(transform.position, m_PlayerTransform.position) < m_AggroDistance)
        {
            m_InRange = true;
            anim.SetBool("Attack", true);
        }
        else
        {
            m_InRange = false;
            anim.SetBool("Attack", false);
        }
			
		
	}

	void Shoot()
	{
		if(m_InRange){

            if (Vector3.Distance(m_Rigidbody2D.transform.position, m_PlayerTransform.position) > 18) {
                if (m_Rigidbody2D.transform.position.x < m_PlayerTransform.position.x) {
                    m_Rigidbody2D.velocity = Vector2.right * m_MovementSpeed;
                    //Otherwise
                }
                else
                {
                    m_Rigidbody2D.velocity = Vector2.right * -m_MovementSpeed;
                }
            
            }
			frames++;
			if(frames * Time.deltaTime > m_TimeBetweenShots){
				GameObject m_NewBullet = Instantiate(m_BulletObject, m_InstantiationPosition.position, new Quaternion(0,0,0,0));
				m_NewBullet.GetComponent<MoveBullet>().direction = direction;
				frames = 0;
			}
		}

	}

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.tag == "Player Attack")
        {
            alive = false;
            anim.SetTrigger("Die");
        }
    }


}
