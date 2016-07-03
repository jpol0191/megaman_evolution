using UnityEngine;
using System.Collections;
using UnityEngine.SceneManagement;

public class LevelHandler : MonoBehaviour {

    public GameObject player;
    public GameObject boss;

    public float gameoverWait;
    private float gameoverTimeWaited;

    public float winWait;
    private float winTimeWaited;

	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {

        if (!player) {
            gameoverTimeWaited += Time.deltaTime;
            if(gameoverTimeWaited >= gameoverWait) {
                SceneManager.LoadScene(2);
            }
        }

        if (!boss) {
            winTimeWaited += Time.deltaTime;
            if(winTimeWaited >= winWait) {
                SceneManager.LoadScene(3);
            }
        }
	}
}
