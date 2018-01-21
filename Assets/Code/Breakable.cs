using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Breakable : MonoBehaviour {
    public float maxHp = 3f;
    float hp = 3f;
	// Use this for initialization
	void Start () {
        hp = maxHp;

    }
	
	// Update is called once per frame
	void Update () {
		
	}
    public void Break() {
        Destroy(gameObject);
    }

    public void getHitLocal(float damage) {
        hp -= damage;
        if(hp <= 0) {
            Break();
        }
    }
}
