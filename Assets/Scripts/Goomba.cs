using UnityEngine;
using System.Collections;

public class Goomba : Enemy {


    // Physics
    private float gravity;
    public float jumpHeight = 4;
    public float timeToJumpApex = 0.4f;
    private float jumpVelocity;
    float accelerationTimeAirborne = 0.1f;
    float accelerationTimeGrounded = 0.0f;
    Vector3 velocity;
    float velocityXSmoothing;

    private float input = -1;

    public float deathTime = 0.8f; 

    // Use this for initialization
    public override void Start () {
        base.Start();
        gravity = -(2 * jumpHeight) / Mathf.Pow(timeToJumpApex, 2);
        jumpVelocity = Mathf.Abs(gravity) * timeToJumpApex;
        print("Gravity: " + gravity + "  Jump Velocity: " + jumpVelocity);

    }
	
	// Update is called once per frame
	void Update () {

        float targetVelocityX = input * moveSpeed;
        velocity.x = Mathf.SmoothDamp(velocity.x, targetVelocityX, ref velocityXSmoothing, (controller.collisions.below) ? accelerationTimeGrounded : accelerationTimeAirborne);

        if (controller.collisions.above || controller.collisions.below) {
            velocity.y = 0;
        }

        velocity.y += gravity * Time.deltaTime;
        controller.Move(velocity * Time.deltaTime);

        if (isAlive) {
            for (int i = 0; i < controller.horizontalRayCount; i++) {

                Vector3 rayOrigin = controller.raycastOrigins.bottomLeft;
                rayOrigin += (controller.horizontalRaycastSpacing * i) * Vector3.up;
                Debug.DrawRay(rayOrigin, Vector3.left, Color.green);
                RaycastHit2D hitPlayer = Physics2D.Raycast(rayOrigin, Vector3.left, 1, playerMask);

                if (hitPlayer) {
                    Player playerScript = hitPlayer.transform.GetComponent<Player>();
                    if (hitPlayer.distance <= 0.1f) {
                        playerScript.isHurt = true;
                        playerScript.damageDir = -1;
                        continue;
                    }
                }

                rayOrigin = controller.raycastOrigins.bottomRight;
                rayOrigin += (controller.horizontalRaycastSpacing * i) * Vector3.up;
                Debug.DrawRay(rayOrigin, Vector3.right, Color.green);
                hitPlayer = Physics2D.Raycast(rayOrigin, Vector3.right, 1, playerMask);

                if (hitPlayer) {
                    Player playerScript = hitPlayer.transform.GetComponent<Player>();
                    if (hitPlayer.distance <= 0.1f) {
                        playerScript.isHurt = true;
                        playerScript.damageDir = 1;
                        continue;
                    }
                }
            }


            for (int i = 0; i < controller.verticalRayCount; i++) {
                Vector3 rayOrigin = controller.raycastOrigins.topLeft;
                rayOrigin += (controller.horizontalRaycastSpacing * i) * Vector3.right;
                Debug.DrawRay(rayOrigin, Vector3.up, Color.blue);
                RaycastHit2D hitPlayer = Physics2D.Raycast(rayOrigin, Vector3.up, 1, playerMask);

                // if player jumps on goomba head
                if (hitPlayer) {
                    Player playerScript = hitPlayer.transform.GetComponent<Player>();
                    if (hitPlayer.distance <= 0.1f && !playerScript.controller.collisions.below) {
                        playerScript.Jump();
                        isAlive = false;
                        animator.SetBool("isDead", true);
                        audioSource.Play();
                        StartCoroutine(Die());
                    }
                }
            } 
        }
    }

    IEnumerator Die() {

        yield return new WaitForSeconds(deathTime);
        Destroy(gameObject);
    }
}



