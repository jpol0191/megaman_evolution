using UnityEngine;
using System.Collections;

[RequireComponent(typeof(BoxCollider2D))]
[RequireComponent(typeof(Animator))]
[RequireComponent(typeof(Rigidbody2D))]
public class Bullet : MonoBehaviour {

    public Vector2 speed;
    public int damage;

	// Use this for initialization
	public virtual void  Start () {
        gameObject.GetComponent<BoxCollider2D>().isTrigger = true;
        gameObject.GetComponent<Rigidbody2D>().isKinematic = true;
	}
	
	// Update is called once per frame
	public virtual void Update () {
        transform.Translate(speed * Time.deltaTime);
	}

    public virtual void OnTriggerEnter2D(Collider2D col) {
        
    }
}
