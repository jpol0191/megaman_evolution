using UnityEngine;
using System.Collections;


[RequireComponent (typeof(Controller2D))]
public class Player : MonoBehaviour {

    Controller2D controller;

    
    public float jumpHeight = 4;
    public float timeToJumpApex = 0.4f;
    public float moveSpeed = 6;

    public float wallSlideSpeedMax = 3;

    Vector3 velocity;
    float jumpVelocity;
    float gravity;
    
    
	// Use this for initialization
	void Start () {
        controller = GetComponent<Controller2D>();

        gravity = -(2 * jumpHeight)/Mathf.Pow(timeToJumpApex,2);
        jumpVelocity = Mathf.Abs(gravity) * timeToJumpApex;
	}
	
	// Update is called once per frame
	void Update () {
        bool wallSliding = false;

        if ((controller.collisions.left || controller.collisions.right) && !controller.collisions.below && velocity.y < 0) {
            wallSliding = true;

            if (velocity.y < -wallSlideSpeedMax) {
                velocity.y = -wallSlideSpeedMax;
            }
        }

        //Stop gaining gravity when collision happens
        if (controller.collisions.above || controller.collisions.below) {
            velocity.y = 0;
        }

        // Jump if space is pressed and player is on the ground
        if (Input.GetKeyDown(KeyCode.Space) && controller.collisions.below) {
            velocity.y = jumpVelocity;
        }

        Vector2 input = new Vector2(Input.GetAxisRaw("Horizontal"), Input.GetAxisRaw("Vertical"));
        // move player based on move speed
        velocity.x = input.x * moveSpeed;
        // update velocity due to gravity
        velocity.y += gravity * Time.deltaTime;
        controller.Move(velocity * Time.deltaTime);
	}
}
