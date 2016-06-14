using UnityEngine;
using System.Collections;

public class MusicManager : MonoBehaviour {

    public AudioClip[] levelMusicChangeArray;

    private AudioSource audioSource;

	void Awake() {
        DontDestroyOnLoad(gameObject);
        Debug.Log("Don't destroy on load " + name);
    }

    void Start() {
        audioSource = GetComponent<AudioSource>();
        audioSource.clip = levelMusicChangeArray[0];
        audioSource.loop = true;
        audioSource.Play();
    }
	
	void OnLevelWasLoaded(int level) {
        Debug.Log("Playing music for level: " + levelMusicChangeArray[level]);
        AudioClip thisLevelMusic = levelMusicChangeArray[level];
        if (thisLevelMusic) {
            audioSource.clip = thisLevelMusic;
            audioSource.loop = true;
            audioSource.Play();
        }
    }
}
