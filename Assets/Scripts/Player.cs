using UnityEngine;
using System.Collections;
using UnityEditor;

[RequireComponent(typeof(Controller2D))]
public class Player : MonoBehaviour {

    public float jumpHeight = 4;
    public float timeToJumpApex = 0.4f;
    float accelerationTimeAirborne = 0.1f;
    float accelerationTimeGrounded = 0.0f;
    public float moveSpeed = 6;

    public Vector2 wallJumpClimb;
    public Vector2 wallJumpOff;
    public Vector2 wallLeap;

    public float wallSlideSpeedMax = 3;
    public float wallStickTime = 0.25f;
    float timeToWallUnstick;

    float gravity;
    float jumpVelocity;
    Vector3 velocity;
    float velocityXSmoothing;

    Controller2D controller;

    //Animation variables
    private Animator animator;
    private Transform spriteBody;

    // Shot variables
    private Vector3 gunLocation = new Vector3(2.0f,0.5f,-1);
    public GameObject bulletObject;
    public float shotCooldown;
    public float chargeInterval;
    private float shotTimetamp;
    private float chargeTime;
    private int chargeLevel;
    

    void Start() {
        controller = GetComponent<Controller2D>();

        gravity = -(2 * jumpHeight) / Mathf.Pow(timeToJumpApex, 2);
        jumpVelocity = Mathf.Abs(gravity) * timeToJumpApex;
        print("Gravity: " + gravity + "  Jump Velocity: " + jumpVelocity);

        // Initialize animation variables
        animator = GetComponent<Animator>();
        spriteBody = gameObject.GetComponentInChildren<SpriteRenderer>().transform;

        Debug.Log(transform.GetChild(0).name);
    }

    void Update() {
        // Reset animation triggers
        animator.ResetTrigger("shot");
        animator.SetBool("isShooting", false);

        // Get player left and right input
        Vector2 input = new Vector2(Input.GetAxisRaw("Horizontal"), Input.GetAxisRaw("Vertical"));
        int wallDirX = (controller.collisions.left) ? -1 : 1;

        // Move Player across the x axis
        float targetVelocityX = input.x * moveSpeed;
        velocity.x = Mathf.SmoothDamp(velocity.x, targetVelocityX, ref velocityXSmoothing, (controller.collisions.below) ? accelerationTimeGrounded : accelerationTimeAirborne);


        // Wall sliding code
        bool wallSliding = false;
        animator.SetBool("isWallSliding", false);
        if ((controller.collisions.left || controller.collisions.right) && !controller.collisions.below && velocity.y < 0) {
            wallSliding = true;
            animator.SetBool("isWallSliding", true);
            if (velocity.y < -wallSlideSpeedMax) {
                velocity.y = -wallSlideSpeedMax;
            }

            if (timeToWallUnstick > 0) {
                velocityXSmoothing = 0;
                velocity.x = 0;

                if (input.x != wallDirX && input.x != 0) {
                    timeToWallUnstick -= Time.deltaTime;
                }
                else {
                    timeToWallUnstick = wallStickTime;
                }
            }
            else {
                timeToWallUnstick = wallStickTime;
            }
        }

        if (controller.collisions.above || controller.collisions.below) {
            velocity.y = 0;
        }


        // Make Player jump if they press the space button
        if (Input.GetKeyDown(KeyCode.Space)) {
            if (wallSliding) {
                if (wallDirX == input.x) {
                    velocity.x = -wallDirX * wallJumpClimb.x;
                    velocity.y = wallJumpClimb.y;
                }
                else if (input.x == 0) {
                    velocity.x = -wallDirX * wallJumpOff.x;
                    velocity.y = wallJumpOff.y;
                }
                else {
                    velocity.x = -wallDirX * wallLeap.x;
                    velocity.y = wallLeap.y;
                }
            }
            if (controller.collisions.below) {
                velocity.y = jumpVelocity;
            }
        }

        // Make player shoot 
        if (Input.GetKeyDown(KeyCode.Z)) {

            // Shoot if shot cooldown is expired
            if (shotTimetamp <= Time.time) {
                chargeLevel = 0; 
                shotTimetamp = Time.time + shotCooldown; //setting shot cooldown
                if (!animator.GetBool("isJumping")) {
                    if (animator.GetBool("isRunning")) {
                        animator.SetBool("isShooting", true);
                        Shoot();
                    }
                    animator.SetTrigger("shot");
                }
                else {
                    animator.SetBool("isShooting", true);
                    Shoot();
                }
            }


        }
        if (Input.GetKey(KeyCode.Z)) {
            chargeTime += Time.deltaTime; 
        }

        // Shoot a charge shot
        if (Input.GetKeyUp(KeyCode.Z)) {
            if (chargeTime >= chargeInterval * 2) {
                Debug.Log("Shooting a charge shot level 2");
                chargeLevel = 2;
                animator.SetTrigger("chargeShot");
                Shoot();
            }
            else if (chargeTime >= chargeInterval) {
                Debug.Log("Shooting a charge shot level 1");
                chargeLevel = 1;
                animator.SetTrigger("chargeShot");
                Shoot();
            }
            chargeLevel = 0;
            chargeTime = 0;
        }

        velocity.y += gravity * Time.deltaTime;
        controller.Move(velocity * Time.deltaTime);


        // ---------------Animation code-----------------------------
        // Jump animations 
        if (controller.collisions.below) {
            animator.SetBool("isJumping", false);
        }
        else {
            animator.SetBool("isJumping", true);
            spriteBody.localScale = new Vector3(Mathf.Sign(velocity.x), 1, 1);
        }

        // Wall slide

        if (wallSliding) {
            if (controller.collisions.left) {
                spriteBody.localScale = new Vector3(-1, 1, 1);
            }
            else {
                spriteBody.localScale = new Vector3(1, 1, 1);
            }

        }
        

        // Running animations
        if(controller.collisions.below && input.x != 0) {
            animator.SetBool("isRunning", true);
            spriteBody.localScale = new Vector3(Mathf.Sign(velocity.x),1,1);
        }
        else {
            animator.SetBool("isRunning", false);
        }

        
       
    }

    void Shoot() {
        GameObject bulletPrefab = AssetDatabase.LoadAssetAtPath("Assets/Models/Megaman/Bullet.prefab", typeof(GameObject)) as GameObject;
        PlayerShot bullet = bulletPrefab.GetComponent<PlayerShot>();
        bullet.chargeLevel = chargeLevel;
        bullet.direction = Mathf.Sign(spriteBody.localScale.x);
        gunLocation.x = Mathf.Sign(spriteBody.localScale.x) * Mathf.Abs(gunLocation.x);
        if (animator.GetBool("isWallSliding")) {
            gunLocation.x *= -1;
            bullet.direction *= -1;
        }
        bulletPrefab.transform.localScale = new Vector3(Mathf.Sign(gunLocation.x), 1, 1);
        
        GameObject bulletObj = Instantiate( bulletPrefab, gunLocation + transform.position, Quaternion.identity) as GameObject;
        
    }
}

