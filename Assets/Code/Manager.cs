using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Manager : Photon.MonoBehaviour {

    public static Manager instance;

    public string currentMode = "waiting";

    public Camera activCam;
    public Camera lobbyCam;

    public OperatorConst[] allOperators;
    public WeaponConst[] allWeapons;

    public Transform[] spawns;

    // Use this for initialization
    private void Awake() {
        activCam = lobbyCam;
        if (instance) {
            Debug.LogWarning("2 Managers!");
            return;
        }
        else {
            instance = this;
        }

        for (int i = 0; i < allOperators.Length; i++) {
            allOperators[i].id = i;
        }
        for (int i = 0; i < allWeapons.Length; i++) {
            allWeapons[i].id = i;
        }

        PhotonNetwork.ConnectUsingSettings("1.0");
        PhotonNetwork.autoJoinLobby = true;
    }
    void Start () {
        

    }

    void OnJoinedLobby() {
        PhotonNetwork.JoinOrCreateRoom("dev", null, null);
    }

    void OnJoinedRoom() {
        currentMode = "room";
    }

    // Update is called once per frame
    void Update () {
		
	}

    public void SpawnWitch(OperatorConst operat) {
        Manager.instance.currentMode = "playing";
        //chose random spawnpoint
        var spawnpoint = GetRandomSpawn();

        var clone = PhotonNetwork.Instantiate("Player", spawnpoint, Quaternion.identity, 0);
        var player = clone.GetComponent<Player>();
        player.local = operat;
        player.mine = true;
        activCam = player.cam;
    }

    public Vector3 GetRandomSpawn() {
        int index = Random.Range(0, spawns.Length);
        return spawns[index].position;
    }

    [PunRPC]
    public void SetPlayerConsts() {

    }

}
