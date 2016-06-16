using UnityEngine;
using System.Collections;

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
    private Vector2 gunLocation = new Vector2(2.0f,0.5f);

    void Start() {
        controller = GetComponent<Controller2D>();

        gravity = -(2 * jumpHeight) / Mathf.Pow(timeToJumpApex, 2);
        jumpVelocity = Mathf.Abs(gravity) * timeToJumpApex;
        print("Gravity: " + gravity + "  Jump Velocity: " + jumpVelocity);

        // Initialize animation variables
        animator = GetComponent<Animator>();
        spriteBody = gameObject.GetComponentInChildren<SpriteRenderer>().transform;
    }

    void Update() {
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
            animator.SetTrigger("shot");
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

    }
}

