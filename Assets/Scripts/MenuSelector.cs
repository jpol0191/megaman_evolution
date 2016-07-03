using UnityEngine;
using System.Collections;
using UnityEngine.SceneManagement;

public class MenuSelector : MonoBehaviour {

    public string[] options;

    private int index;
    private AudioSource audioSource;

	// Use this for initialization
	void Start () {
        audioSource = GetComponent<AudioSource>();
        options = new string[] { "start", "quit"};
        index = 0;
        Debug.Log("Menu selector is working ");
	}
	
	// Update is called once per frame
	void Update () {
        if (Input.GetKeyDown(KeyCode.DownArrow)) {
            audioSource.Play();
            index += 1;
            if (index > options.Length - 1) {
                index = options.Length - 1;
            }
        }else if (Input.GetKeyDown(KeyCode.UpArrow)) {
            audioSource.Play();
            index -= 1;
            if (index < 0 ) {
                index = 0; 
            }
        }

        if (options[index] == "start" ) {
            GetComponent<RectTransform>().localPosition = new Vector3(-211,-160);
        }else if (options[index] == "quit") {
            GetComponent<RectTransform>().localPosition = new Vector3(-191,-283);
        }

        if (Input.GetKeyDown(KeyCode.Z)) {
            if (options[index] == "start") {
                SceneManager.LoadScene(1);
                
            }

            if (options[index] == "quit") {
                Application.Quit();
            }
        }
    }

}
