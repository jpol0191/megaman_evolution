using UnityEngine;
using System.Collections;

public class BossRoom : MonoBehaviour {

    public GameObject cameraMain;
    public Vector3[] restPoint;
    public GameObject door;

    public GameObject Boss;
    private Boss bossScript;

    private CameraFollow cameraScript;
    
    void Start() {
        cameraScript = cameraMain.GetComponent<CameraFollow>();
        bossScript = Boss.GetComponent<Boss>();
    }

	void OnTriggerEnter2D (Collider2D col) {
        if (col.tag == "Player") {
            cameraScript.followPlayer = false;
            bossScript.startBattle = true;
            door.GetComponent<Animator>().enabled = true;
            Destroy(gameObject.GetComponent<BoxCollider2D>());

        }
    }

    void OnDrawGizmos() {
        Gizmos.color = Color.red;

        foreach (Vector3 point in restPoint) {
            Vector3 globalPoint = point + transform.position;
            Gizmos.DrawCube(globalPoint, new Vector3(1, 1));
        }
    }

}
