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

    // Player damage
    public float damageDir; 
    public bool isHurt;
    private bool wasHurt;
    public float hurtTime;
    public Vector3 hurtVelocity;

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

    public Controller2D controller;

    //Animation variables
    private Animator animator;
    private Animator chargeAnimator;
    private Transform spriteBody;

    // Shot variables
    private Vector3 gunLocation = new Vector3(2.0f,0.5f,-1);
    public GameObject bulletObject;
    public float shotCooldown;
    public float chargeInterval;
    private float shotTimetamp;
    private float chargeTime;
    private int chargeLevel;

    // ------Sound-------
    
    // Charge sounds 
    public AudioSource gunAudioSource;
    public AudioClip[] chargeSounds;
    IEnumerator chargeCoroutine;


    

    void Start() {
        controller = GetComponent<Controller2D>();

        gravity = -(2 * jumpHeight) / Mathf.Pow(timeToJumpApex, 2);
        jumpVelocity = Mathf.Abs(gravity) * timeToJumpApex;
        print("Gravity: " + gravity + "  Jump Velocity: " + jumpVelocity);

        // Initialize animation variables
        animator = GetComponent<Animator>();
        spriteBody = gameObject.GetComponentInChildren<SpriteRenderer>().transform;
        chargeAnimator = transform.GetChild(1).GetComponent<Animator>();
        chargeCoroutine = PlayChargeSound();

        hurtTime = .5f;
    }

    void Update() {
        // Reset animation triggers
        animator.ResetTrigger("shot");
        animator.ResetTrigger("chargeShot");
        animator.SetBool("isShooting", false);

        // Get player left and right input
        Vector2 input = new Vector2(Input.GetAxisRaw("Horizontal"), Input.GetAxisRaw("Vertical"));
        int wallDirX = (controller.collisions.left) ? -1 : 1;

        // if player is hurt then override player input
        if (isHurt) {
            input.x = damageDir;
            if (!wasHurt) {
                animator.SetTrigger("hurtTrigger");
                animator.SetBool("isHurt", true);
                StartCoroutine(HurtTimer(hurtTime, animator));
                wasHurt = true;
            }
        }
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
            Jump(input, wallDirX, wallSliding);
        }

        // Make player shoot 
        if (Input.GetKeyDown(KeyCode.Z)) {
            
            // Shoot if shot cooldown is expired
            if (shotTimetamp <= Time.time) {
                StartCoroutine(chargeCoroutine);
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
            // Animate the charging
            if (chargeTime > chargeInterval * 0.2f && chargeTime < chargeInterval ) {
                chargeAnimator.SetInteger("chargeStage", 1);
            }
            else if (chargeTime > chargeInterval && chargeTime < chargeInterval * 2) {
                chargeAnimator.SetInteger("chargeStage", 2);
            }
            else if (chargeTime > chargeInterval * 2) {
                chargeAnimator.SetInteger("chargeStage", 3);
            }
        }

        // Shoot a charge shot
        if (Input.GetKeyUp(KeyCode.Z)) {
            gunAudioSource.Stop();
            StopCoroutine(chargeCoroutine);
            chargeCoroutine = PlayChargeSound();
            chargeAnimator.SetInteger("chargeStage", 0);
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

        // Add effect of gravity 
        velocity.y += gravity * Time.deltaTime;

        // Finally move the player
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

    private void Jump(Vector2 input, int wallDirX, bool wallSliding) {
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

    public void Jump() {
        velocity.y = jumpVelocity;
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
        
        Instantiate( bulletPrefab, gunLocation + transform.position, Quaternion.identity);
        
    }

    IEnumerator PlayChargeSound() {
        yield return new WaitForSeconds(.06f);
        gunAudioSource.clip = chargeSounds[0];
        gunAudioSource.loop = false;
        gunAudioSource.Play();
        yield return new WaitForSeconds(gunAudioSource.clip.length);
        gunAudioSource.clip = chargeSounds[1];
        gunAudioSource.loop = true;
        gunAudioSource.Play();
    }

    IEnumerator HurtTimer(float n, Animator anim) {
        
        yield return new WaitForSeconds(n);
        isHurt = false;
        wasHurt = false;
        anim.SetBool("isHurt", false);
        Debug.Log("Reseting the hurt bool. Hurt time was: "+ n);
    }
}

