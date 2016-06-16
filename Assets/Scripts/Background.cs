using UnityEngine;
using System.Collections;

public class Background : MonoBehaviour {

    public Transform cameraTransform;
    public Transform playerTransform;
    [Range(0,0.02f)]
    public float scrollSpeedMod;
    Material mat;

    Renderer rend;
	// Use this for initialization
	void Start () {
        rend = GetComponent<Renderer>();
    }

    // Update is called once per frame
    void Update() {
        
        transform.position = new Vector2(cameraTransform.position.x, cameraTransform.position.y);
        float x = Mathf.Repeat(playerTransform.position.x * scrollSpeedMod, 1);
        rend.sharedMaterial.SetTextureOffset("_MainTex", new Vector2(x, 0));
    }
}
