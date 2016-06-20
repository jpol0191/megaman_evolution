using UnityEngine;
using System.Collections;

[RequireComponent (typeof (Controller2D))]
[RequireComponent (typeof(Animator))]
[RequireComponent (typeof(AudioSource))]
public class Enemy : MonoBehaviour {

    //Enemy stats
    public int hp;
    public float moveSpeed;
    public bool isAlive = true;

    public LayerMask playerMask;

    // Components
    protected Animator animator;
    protected Controller2D controller;
    protected AudioSource audioSource;

	// Use this for initialization
	public virtual void Start () {
        animator = GetComponent<Animator>();
        controller = GetComponent<Controller2D>();
        audioSource = GetComponent<AudioSource>();
	}

}
