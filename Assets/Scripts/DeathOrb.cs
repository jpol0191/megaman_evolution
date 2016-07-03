using UnityEngine;
using System.Collections;

public class DeathOrb : MonoBehaviour {

    public Vector3 velocity;
    public float waitTime;

	// Update is called once per frame
	void Update () {

        if (waitTime <=0) {
            transform.Translate(velocity * Time.deltaTime);
        }
        else {
            waitTime -= Time.deltaTime;
        }
	}
}
