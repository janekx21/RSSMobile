using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Breakable :  MonoBehaviour {
    public float maxHp = 3f;
    public float hp = 3f;
    public PhotonView photonView;
    // Use this for initialization
    private void Awake() {
        photonView = GetComponent<PhotonView>();
    }
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
    [PunRPC]
    public void getHit(float damage) {
        hp -= damage;
        if(hp <= 0) {
            Break();
            PhotonNetwork.RPC(photonView, "Break", PhotonTargets.OthersBuffered,false); 
        }
        print("got hit");
    }
}
