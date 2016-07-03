using UnityEngine;
using System.Collections;

public class Goomba : Enemy {

    public int damage;

    // prefabs
    public GameObject explosion;

    // Physics
    private float gravity;
    public float jumpHeight = 4;
    public float timeToJumpApex = 0.4f;
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

        if (hp <= 0) isAlive = false;

        if (isAlive) {

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
                        Death(true);
                        
                        continue;
                    }
                }
            }

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
            Death(false);
        }
    }

    private void Death(bool stepedOn) {
        isAlive = false;
        if (stepedOn) {
            animator.SetBool("isDead", true);
            audioSource.Play();
        }
        else {
            //GameObject explosion = AssetDatabase.LoadAssetAtPath("Assets/Models/Enemies/effects/explosions/SmallExplosion.prefab", typeof(GameObject)) as GameObject;
            Instantiate(explosion, transform.position, Quaternion.identity);
        }
        StartCoroutine(Die());
        GetComponent<Goomba>().enabled = false;
        GetComponent<Goomba>().enabled = false;
     
    }

    IEnumerator Die() {
        yield return new WaitForSeconds(deathTime);
        Destroy(gameObject);
    }
}



