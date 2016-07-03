using UnityEngine;
using System.Collections;

public class Ruby : Bullet {

    public GameObject player;
    public GameObject explosion;
    public float moveSpeed;
    private Vector3 target;

	// Use this for initialization
	public override void Start () {
        base.Start();

        target = player.transform.position;
	}
	
	// Update is called once per frame
	public override void Update () {
        transform.position = Vector3.MoveTowards(transform.position, target, moveSpeed * Time.deltaTime);

        if(transform.position == target) {
            BlowUp();
        }
	}

    public override void OnTriggerEnter2D(Collider2D col) {
        if (col.tag == player.tag) {
            Player playerScript = col.GetComponent<Player>();
            if (playerScript.isHurt == false) {
                playerScript.isHurt = true;
                playerScript.hp -= damage;
                playerScript.damageDir = -1;
                BlowUp(); 
            }
        }
    }

    void BlowUp() {
        Instantiate(explosion, transform.position, Quaternion.identity);
        Destroy(gameObject);
    }
}
