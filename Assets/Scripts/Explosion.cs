using UnityEngine;
using System.Collections;

[RequireComponent(typeof(AudioSource))]
public class Explosion : MonoBehaviour {
    AudioSource audioSource;
	// Use this for initialization
	void Start () {
        audioSource = GetComponent<AudioSource>();
        audioSource.Play();
	}

    void SelfDestroy() {
        Destroy(gameObject);
    }
}
