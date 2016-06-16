using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class PlayerShot : Bullet {

    public LayerMask playerLayerMask;

    [Range (0,2)]
    public int chargeLevel;
    public List<ShotStats> shot;
    public float direction = 1;


    public PlayerShot(int chargeLevel) {
        this.chargeLevel = chargeLevel;
    }

    void Awake() {

    }

	// Use this for initialization
	public override void Start () {
        base.Start();
        shot = new List<ShotStats>();

        // Building shots from lowest charge to highest
        shot.Add(new ShotStats(1, new Vector2 (10,0),1));
        shot.Add(new ShotStats(2, new Vector2 (3,0),3));
        shot.Add(new ShotStats(4, new Vector2 (3,0),5));
        this.speed = shot[chargeLevel].speed  * Mathf.Sign(direction);
    }
	
	// Update is called once per frame
	public override void Update () {
        transform.Translate(this.speed * Time.deltaTime);
        
	}

    public override void OnTriggerEnter2D(Collider2D col) {
        base.OnTriggerEnter2D(col);

        if (col.name != "Player") {
            Destroy(gameObject);
            
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
