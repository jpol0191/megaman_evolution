using UnityEngine;
using System.Collections;

public class Duck : Enemy {

    Vector3 velocity;

    public int damage;

    private float velocityXSmoothing;
    private float accelerationTimeAirborne = 0.1f;
    private float accelerationTimeGrounded = 0.0f;
    private float input;

    // Move pattern variables
    public float timeToChangeDir;
    private float timeInThisDir;
    private Vector2 fallVelocity;

    //Sounds 
    public AudioClip duckHuntTune;
    public AudioClip blastSound;
    public AudioClip quackSound;

    // Use this for initialization
    public override void Start () {
        base.Start();

        input = -1;
        isAlive = true;
        animator.SetBool("isAlive", true);
        
	}
	
	// Update is called once per frame
	void Update () {

        if (hp <= 0) isAlive = false;

        if (isAlive) {
            timeInThisDir += Time.deltaTime;
            if (timeInThisDir >= timeToChangeDir) {
                input *= -1;
                timeInThisDir = 0;
            }

            float targetVelocityX = input * moveSpeed;
            velocity.x = Mathf.SmoothDamp(velocity.x, targetVelocityX, ref velocityXSmoothing, (controller.collisions.below) ? accelerationTimeGrounded : accelerationTimeAirborne);

            if (controller.collisions.above || controller.collisions.below) {
                velocity.y = 0;
            }

            controller.Move(velocity * Time.deltaTime);
            transform.localScale = new Vector3(Mathf.Sign(velocity.x), 1, 1);

            if (isAlive) {
                for (int i = 0; i < controller.horizontalRayCount; i++) {

                    Vector3 rayOrigin = controller.raycastOrigins.bottomLeft;
                    rayOrigin += (controller.horizontalRaycastSpacing * i) * Vector3.up;
                    Debug.DrawRay(rayOrigin, Vector3.left, Color.green);
                    RaycastHit2D hitPlayer = Physics2D.Raycast(rayOrigin, Vector3.left, 1, playerMask);

                    if (hitPlayer) {
                        Player playerScript = hitPlayer.transform.GetComponent<Player>();
                        if (hitPlayer.distance <= 0.1f) {
                            if (!playerScript.isHurt) {
                                playerScript.isHurt = true;
                                playerScript.hp -= damage;
                                playerScript.damageDir = -1;

                                continue;
                            }
                        }
                    }

                    rayOrigin = controller.raycastOrigins.bottomRight;
                    rayOrigin += (controller.horizontalRaycastSpacing * i) * Vector3.up;
                    Debug.DrawRay(rayOrigin, Vector3.right, Color.green);
                    hitPlayer = Physics2D.Raycast(rayOrigin, Vector3.right, 1, playerMask);

                    if (hitPlayer) {
                        Player playerScript = hitPlayer.transform.GetComponent<Player>();
                        if (hitPlayer.distance <= 0.1f) {
                            if (!playerScript.isHurt) {
                                playerScript.isHurt = true;
                                playerScript.hp -= damage;
                                playerScript.damageDir = 1;

                                continue;
                            }
                        }
                    }
                }
            }
        }
        else {
            animator.SetBool("isAlive", false);
            controller.Move(fallVelocity * Time.deltaTime);
        }


        if (controller.collisions.below) {
            AudioSource.PlayClipAtPoint(duckHuntTune, transform.position);
            Destroy(gameObject);
        }








    }

    void PlayBlastSound() {
        
        AudioSource.PlayClipAtPoint(blastSound, transform.position, 100);
        AudioSource.PlayClipAtPoint(quackSound, transform.position, 100);
    }

    void Fall() {
        fallVelocity = new Vector2(0,-7);
    }

}
