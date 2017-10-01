using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ToxicShot : MonoBehaviour {

    public float speed;

    GameObject player;
    Vector3 moveDir;


	// Use this for initialization
	void Start () {
        player = GameObject.FindGameObjectWithTag("Player");
        Vector3 playerCenter = new Vector3(player.transform.position.x + 1.2f, player.transform.position.y + 1.6f);

        moveDir = playerCenter - transform.position;
        moveDir = moveDir.normalized;
    }

    private void Update()
    {
        gameObject.transform.position += moveDir * Time.deltaTime * speed;
    }

    void OnTriggerEnter2D(Collider2D coll)
    {
        if (coll.gameObject.tag == "Ball" || coll.gameObject.tag == "Player" || coll.gameObject.layer == LayerMask.NameToLayer("Scenario"))
        {
            Destroy(this.gameObject);
        }
    }
}
