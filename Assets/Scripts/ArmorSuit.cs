using UnityEngine;
using System.Collections;

public class ArmorSuit : Enemy {

    public int damage;
    public GameObject player;
    public float distanceToWake;
    private Bounds bounds;
    public GameObject explosion;

    // Physics
    private float gravity;
    public float jumpHeight = 4;
    public float timeToJumpApex = 0.4f;
    private float jumpVelocity;
    float accelerationTimeAirborne = 0.1f;
    float accelerationTimeGrounded = 0.0f;
    Vector3 velocity;
    float velocityXSmoothing;
    public CurrentState currentState;


    public float input;
    private Transform spriteBody;

    // Use this for initialization
    public override void Start () {
        base.Start();
        gravity = -(2 * jumpHeight) / Mathf.Pow(timeToJumpApex, 2);
        input = -1;

        spriteBody = gameObject.GetComponentInChildren<SpriteRenderer>().transform;
        
        currentState = CurrentState.Sleeping;
    }
	
	// Update is called once per frame
	void Update () {
        if (hp <= 0) {
            isAlive = false;
        }

        switch (currentState) {
            case CurrentState.Sleeping:
                Sleeping();
                break;
            case CurrentState.Moving:
                Walking();
                break;
            case CurrentState.Dead:
                Dead();
                break;
        }

        float targetVelocityX = input * moveSpeed;
        velocity.x = Mathf.SmoothDamp(velocity.x, targetVelocityX, ref velocityXSmoothing, (controller.collisions.below) ? accelerationTimeGrounded : accelerationTimeAirborne);

        if (controller.collisions.above || controller.collisions.below) {
            velocity.y = 0;
        }

        velocity.y += gravity * Time.deltaTime;
        controller.Move(velocity * Time.deltaTime);

        spriteBody.localScale = new Vector3((velocity.x == 0)? spriteBody.localScale.x : -Mathf.Sign(velocity.x), 1, 1);

        if (isAlive) {

            for (int i = 0; i < controller.verticalRayCount; i++) {
                Vector3 rayOrigin = controller.raycastOrigins.topLeft;
                rayOrigin += (controller.verticalRaycastSpacing * i) * Vector3.right;
                Debug.DrawRay(rayOrigin, Vector3.up, Color.blue);
                RaycastHit2D hitPlayer = Physics2D.Raycast(rayOrigin, Vector3.up, 1, playerMask);

                // if player jumps on goomba head
                if (hitPlayer) {
                    Player playerScript = hitPlayer.transform.GetComponent<Player>();
                    if (hitPlayer.distance <= 0.1f && !playerScript.controller.collisions.below && !playerScript.isHurt) {
                        playerScript.isHurt = true;
                        playerScript.hp -= damage;
                        playerScript.damageDir = -1;
                        Debug.Log("Dealing damage to player");
                        continue;
                    }
                }
            }

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

    private void Dead() {
        input = 0;
        isAlive = false;
    }

    void RandExplosions() {
        bounds = GetComponent<BoxCollider2D>().bounds;
        Vector3 randomLocation = new Vector3(Random.Range(bounds.min.x, bounds.max.x), Random.Range(bounds.min.y, bounds.max.y), -6);
        Instantiate(explosion, randomLocation, Quaternion.identity);
    }
    private void Sleeping() {
        if (isAlive) {
            input = 0;
            if (Vector3.Distance(transform.position, player.transform.position) <= distanceToWake) {
                animator.SetTrigger("wakeup");
            }
        }
        else {
            isAlive = false;
            currentState = CurrentState.Dead;
            animator.SetTrigger("dead");
        }
    }

    public void DestroyArmorSuit() {
        Destroy(gameObject);
    }

    public void Wakeup() {
            currentState = CurrentState.Moving;   
    }

    private void Walking() {
        if (isAlive) {
            
                input = -Mathf.Sign(transform.position.x - player.transform.position.x);
      
        }
        else {
            isAlive = false;
            currentState = CurrentState.Dead;
            animator.SetTrigger("dead");
        }
    }

    private void Waiting() {
        input = 0;
    }

    public enum CurrentState {
        Moving,
        Waiting,
        Sleeping,
        Dead
    }
}
