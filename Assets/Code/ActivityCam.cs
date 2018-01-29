using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ActivityCam : MonoBehaviour {
    public Camera cam;
    public AudioListener listener;
	// Use this for initialization
	void Start () {
        cam = GetComponent<Camera>();
        listener = GetComponent<AudioListener>();
        Update();
    }
	
	// Update is called once per frame
	void Update () {
		if(Manager.instance.activCam == cam) {
            cam.tag = "MainCamera";
            listener.enabled = true;
            cam.enabled = true;
            
        }
        else {
            cam.tag = "Untagged";
            listener.enabled = false;
            cam.enabled = false;
        }
	}
}
