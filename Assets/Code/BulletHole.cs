using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BulletHole : MonoBehaviour {
    float time = 0;
	// Use this for initialization
	void Start () {
	}
	
	// Update is called once per frame
	void Update () {
        time += Time.deltaTime;
        if(time > 5) {
            float s = 1 - (time - 5);
            transform.localScale = new Vector3(s, s, s);
            if (time > 6)
                Destroy(gameObject);
        }
	}
}
