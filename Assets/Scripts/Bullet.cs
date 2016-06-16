using UnityEngine;
using System.Collections;
[RequireComponent(typeof(BoxCollider2D))]
public class Bullet : MonoBehaviour {

    public Vector2 speed;

	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
        transform.Translate(speed * Time.deltaTime);
	}

    void OnTriggerEnter2D(Collider2D col) {
        Debug.Log("Trigger entered with: "+ col.name);
    }

    void OnCollisionEnter2D (Collision2D col) {
        Debug.Log("Collision entered with: " + col.transform.name);
    }
}
