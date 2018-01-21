using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Breakable : Photon.PunBehaviour {
    public float maxHp = 3f;
    float hp = 3f;
	// Use this for initialization
	void Start () {
        hp = maxHp;
    }

    void OnPhotonSerializeView(PhotonStream stream, PhotonMessageInfo info) {
        if (stream.isWriting) {
            stream.SendNext(hp);

        }
        else {
            hp = (float)stream.ReceiveNext();
        }
    }

    // Update is called once per frame
    void Update () {
		
	}
    [PunRPC]
    public void Break() {
        Destroy(gameObject);
    }

    public void getHitLocal(float damage) {
        hp -= damage;
        if(hp <= 0) {
            Break();
            PhotonNetwork.RPC(photonView, "Break", PhotonTargets.OthersBuffered,false);
        }
    }
}
