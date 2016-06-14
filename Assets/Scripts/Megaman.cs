using UnityEngine;
using System.Collections;


[RequireComponent (typeof (Animator))]
public class Megaman : MonoBehaviour {

    public float walkingSpeed;
    public float jumpSpeed;

    private Animator anim;
    private Rigidbody2D body;
	// Use this for initialization
	void Start () {
        anim = gameObject.GetComponentInChildren<Animator>();
        body = gameObject.GetComponent<Rigidbody2D>();
	}
	
	// Update is called once per frame
	void Update () {
     
        anim.SetBool("isRunning", false);

        if (Input.GetKey(KeyCode.RightArrow)) {
            //Only trigger run animation if not jumping
            if (!anim.GetBool("isJumping")) {
                anim.SetBool("isRunning", true);
            }
            // Move to the right and make spirte face right
            transform.Translate(Vector3.right * walkingSpeed * Time.deltaTime);
            gameObject.transform.localScale = new Vector3(1,1,1);
        } else if (Input.GetKey(KeyCode.LeftArrow)) {
            //Only trigger run animation if not jumping
            if (!anim.GetBool("isJumping")) {
                anim.SetBool("isRunning", true);
            }
            // Move to the left and make sprite face left
            transform.Translate(Vector3.left * walkingSpeed * Time.deltaTime);
            gameObject.transform.localScale = new Vector3(-1, 1, 1);
        }

        if (Input.GetKeyDown(KeyCode.Space)) {
            anim.SetBool("isRunning", false);
            anim.SetBool("isJumping", true);

            body.AddForce(Vector2.up * jumpSpeed);
        }

	}

    void OnCollisionEnter2D(Collision2D coll) {

        foreach (ContactPoint2D hit in coll.contacts) {
            Debug.Log("Megaman hitpoint: " + hit.point);
        }

        anim.SetBool("isJumping", false);

        if (Physics2D.Raycast(transform.position, Vector2.down, 10000, 1 << 8).distance > 1.7f) {

        }
    }

    private bool checkground() {

        Debug.Log("Raycast result: " + Physics2D.Raycast(transform.position, Vector2.down,10000,1 << 8).distance);
        return true;
    }

    public void Shoot() {
        Debug.Log("Shoot");
        anim.SetTrigger("shot");
    }
}
