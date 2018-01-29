
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Scope : MonoBehaviour {

    RenderTexture ren;
    public Camera cam;
    public Material mat;
    

	// Use this for initialization
	void Start () {
        ren = new RenderTexture(256, 256, 24);
        mat.mainTexture = ren;
        cam.SetTargetBuffers(ren.colorBuffer, ren.depthBuffer);
    }
	
	// Update is called once per frame
	void Update () {
		
	}
}
