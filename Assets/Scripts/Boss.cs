using UnityEngine;
using System.Collections;

public class Boss : Enemy {

    public BossRoom bossRoom;
    private CurrentState currentState;
    private CurrentState nextAction;
    private float currentPhaseDuration; // Holds duration the current phase has been running
    public int damage;
    public bool startBattle;
    public GameObject explosion;
    private Bounds bounds;


    //Dead animation
    private float explosionCooldown = .3f;
    private float lastExplosion;
    private float deadTime;

    // Shot Variables
    public float shotCooldown;
    private float timeSinceLastShot;
    public GameObject ruby;
    public GameObject player;

    // Idle Phase
    public float idleDuration;

    // Battle Pattern One Phase
    public float battlePatternOneDuration;
    public Vector3[] wayPointPatternOne;

    // Battle Pattern Two Phase
    public float battlePatternTwoDuration;
    public Vector3[] wayPointPatternTwo;
    
    private int pointIndex;

	// Use this for initialization
	public override void Start () {
        base.Start();
        currentState = CurrentState.BattleAction1;
        pointIndex = 0;
        startBattle = false;

        
    }
	
	// Update is called once per frame
	void Update () {
        bounds = GetComponent<BoxCollider2D>().bounds;
        if (startBattle) {
            if (hp <= 0) currentState = CurrentState.Dead; 

            if(hp < 30) {
                animator.SetInteger("hp", 2);
            }else if(hp < 70) {
                animator.SetInteger("hp", 1);
            }
            else {
                animator.SetInteger("hp", 0);
            }

            if (timeSinceLastShot >= shotCooldown && isAlive) {
                timeSinceLastShot = 0;
                Shoot();
            }

            timeSinceLastShot += Time.deltaTime;

            // State machine to control boss actions
            switch (currentState) {
                case CurrentState.Idle:
                    Idle();
                    break;
                case CurrentState.Dead:
                    Dead();
                    break;
                case CurrentState.Talking:

                    break;
                case CurrentState.BattleAction1:
                    BattlePattern1();
                    break;
                case CurrentState.BattleAction2:
                    BattlePattern2();
                    break;
            } 
        }
	
	}

    void Dead() {
        if (isAlive) {
            isAlive = false;
            deadTime = Time.time + 10;
            Invoke("MessagePlayer", 3f);
            
        }

        transform.position = Vector3.MoveTowards(transform.position, bossRoom.restPoint[0] + transform.parent.position, moveSpeed * Time.deltaTime);
        animator.SetInteger("hp", 3);
        lastExplosion += Time.deltaTime;

        if(lastExplosion >= explosionCooldown) {
            RandExplosions();
            lastExplosion = 0;
        }
        
        if(Time.time >= deadTime) {
            Destroy(gameObject);
            
        }

    }

    private void MessagePlayer() {
        player.GetComponent<Player>().disableInput = true;
        player.GetComponent<Player>().animator.SetBool("levelDone", true);
        player.GetComponent<Player>().chargeTime = 0;
        player.GetComponent<Player>().chargeAnimator.SetInteger("chargeStage", 0);
    }

    void RandExplosions() {
        Vector3 randomLocation = new Vector3(Random.Range(bounds.min.x, bounds.max.x), Random.Range(bounds.min.y, bounds.max.y),-6);
        Instantiate(explosion, randomLocation, Quaternion.identity);
    }

    void Idle() {
        transform.position = Vector3.MoveTowards(transform.position, bossRoom.restPoint[0] + transform.parent.position, moveSpeed * Time.deltaTime);
        currentPhaseDuration += Time.deltaTime;

        if (currentPhaseDuration >= idleDuration) {
            currentPhaseDuration = 0;
            int nextPhase = Random.Range(0, 2);
            Debug.Log(nextPhase);
            
            // Switch to random phase
            switch (nextPhase) {
                case 0:
                    
                    currentState = CurrentState.BattleAction1;
                    break;
                case 1:
                    
                    currentState = CurrentState.BattleAction2;
                    break;
            }
            
        }
    }

    void BattlePattern1() {
        int fromWaypoint = pointIndex % wayPointPatternOne.Length;
        transform.position = Vector3.MoveTowards(transform.position, wayPointPatternOne[fromWaypoint] + transform.parent.position, moveSpeed * Time.deltaTime);
        if (wayPointPatternOne[fromWaypoint] + transform.parent.position == transform.position) pointIndex++;
        currentPhaseDuration += Time.deltaTime;

        if (currentPhaseDuration >= battlePatternOneDuration) {
            currentPhaseDuration = 0;
            pointIndex = 0;
            currentState = CurrentState.Idle;
        }

    }

    void BattlePattern2() {
        int fromWaypoint = pointIndex % wayPointPatternTwo.Length;
        transform.position = Vector3.MoveTowards(transform.position, wayPointPatternTwo[fromWaypoint] + transform.parent.position, moveSpeed * Time.deltaTime);
        if (wayPointPatternTwo[fromWaypoint] + transform.parent.position == transform.position) pointIndex++;
        currentPhaseDuration += Time.deltaTime;

        if (currentPhaseDuration >= battlePatternTwoDuration) {
            currentPhaseDuration = 0;
            pointIndex = 0;
            currentState = CurrentState.Idle;
        }
    }


    void Shoot() {
        
        ruby.GetComponent<Ruby>().player = player;
        Instantiate(ruby, new Vector3(0, -1.2f, -0.5f) + transform.position, Quaternion.identity);
        animator.SetTrigger("shot");
    }

    void OnTriggerEnter2D(Collider2D col) {
        if (isAlive) {
            if (col.tag == player.tag) {
                Player playerScript = col.GetComponent<Player>();
                if (playerScript.isHurt == false) {
                    playerScript.isHurt = true;
                    playerScript.hp -= damage;
                    playerScript.damageDir = -1;
                }
            } 
        }
    }

    void OnDrawGizmos() {

        // Draw the way points for battle pattern one
        Gizmos.color = Color.magenta;
        foreach(Vector3 point in wayPointPatternOne) {
            Gizmos.DrawCube (point + transform.parent.position, new Vector3(1,1));
        }

        // Draw the way points for battle pattern two
        Gizmos.color = Color.green;
        foreach (Vector3 point in wayPointPatternTwo) {
            Gizmos.DrawCube (point + transform.parent.position, new Vector3(1, 1));
        }
    }
    public enum CurrentState {
        Idle,
        BattleAction1,
        BattleAction2,
        Dead,
        Talking
    }


   
}



