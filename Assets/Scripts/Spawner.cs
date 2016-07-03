using UnityEngine;
using System.Collections;

[RequireComponent (typeof(BoxCollider2D))]
public class Spawner : MonoBehaviour {

    public GameObject enemyPrefab;
    public Vector3[] spawnPoints;

	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
	
	}

    void OnTriggerEnter2D(Collider2D col) {
        
        if (col.tag == "Player") {
            GetComponent<BoxCollider2D>().enabled = false;
            foreach(Vector3 spawn in spawnPoints) {
                Vector3 globalPoint = spawn + transform.position;
                Instantiate(enemyPrefab,globalPoint,Quaternion.identity);
            }
        }
    }

    void OnDrawGizmos() {
        Gizmos.color = Color.red;
        
        foreach(Vector3 points in spawnPoints) {
            Vector3 globalPoint = points + transform.position;
            Gizmos.DrawCube(globalPoint, new Vector3(1, 1));
        }
    }
}
