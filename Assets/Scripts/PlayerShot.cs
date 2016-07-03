using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class PlayerShot : Bullet {

    public LayerMask playerLayerMask;

    [Range (0,2)]
    public int chargeLevel;
    public List<ShotStats> shot;
    public float direction = 1;

    private int pierce;
    private Animator animator;

    public AudioSource audioSource;
    public AudioClip[] shotSounds;

	// Use this for initialization
	public override void Start () {
        base.Start();
        animator = GetComponent<Animator>();
        shot = new List<ShotStats>();
        
        // Building shots from lowest charge to highest
        shot.Add(new ShotStats(1, new Vector2 (10,0),1));
        shot.Add(new ShotStats(4, new Vector2 (10,0),3));
        shot.Add(new ShotStats(10, new Vector2 (10,0),5));
        speed = shot[chargeLevel].speed  * Mathf.Sign(direction);
        damage = shot[chargeLevel].damage;
        pierce = shot[chargeLevel].pierce;

        animator.SetInteger("charge_Level",chargeLevel);
        if (chargeLevel > 0 ) {
            audioSource.clip = shotSounds[1];
        }
        else {
            audioSource.clip = shotSounds[0];
        }
        audioSource.Play();
    }
	
	// Update is called once per frame
	public override void Update () {
        transform.Translate(this.speed * Time.deltaTime);
	}
    
    public override void OnTriggerEnter2D(Collider2D col) {
        base.OnTriggerEnter2D(col);
        
        if (col.tag == "Stage" ) {
            Destroy(gameObject);
        }

        if (col.tag == "Enemy") {
            Enemy enemyScript = col.GetComponent<Enemy>();
            if (enemyScript) {
                enemyScript.hp -= damage;
            }
            pierce -= enemyScript.hardness;
            if(pierce <= 0) {
                Destroy(gameObject);
            }
        }
    }

    /// <summary>
    /// Holds the stats for shot types for megaman.
    /// </summary>
    public struct ShotStats {
        public int damage;  
        public Vector2 speed;
        public int pierce;

        public ShotStats (int damage, Vector2 speed, int pierce ) {
            this.damage = damage;
            this.speed = speed;
            this.pierce = pierce;
        }
    }
}
