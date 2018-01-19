using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HouseBuilder : MonoBehaviour {

    static int SIZE = 16;

    int[,] layout = new int[SIZE, SIZE];

    public GameObject[] walls;

    public bool hasGenerated = false;

    PhotonView photonView;

	// Use this for initialization
	void Start () {
        photonView = GetComponent<PhotonView>();
        for (int i = 0; i < transform.childCount; i++) {
          //  Destroy(transform.GetChild(i).gameObject);
        }
        if (PhotonNetwork.isMasterClient) {
            Generate();
            hasGenerated = true;
            int[] oneD = new int[SIZE * SIZE];
            for (int x = 0; x < SIZE; x++) {
                for (int y = 0; y < SIZE; y++) {
                    oneD[x + y * SIZE] = layout[x, y];
                }
            }
            PhotonNetwork.RPC(photonView, "finishedGenerating", PhotonTargets.OthersBuffered, false, oneD);
            GenerateStrukture();
        }
        else {

        }
            
        
    }
    bool hasspawnd = false;
	// Update is called once per frame
	void Update () {
        if (PhotonNetwork.isMasterClient) {
            if (!hasspawnd) {
                if (Input.GetKeyDown(KeyCode.P)) {
                    PhotonNetwork.Instantiate("houseBuilder", transform.position + new Vector3(SIZE, 0, 0), Quaternion.identity, 0);
                    hasspawnd = true;
                }
            }
        }
        else {
        }
	}

    void Generate() {
        int numofrooms = Random.Range(2,8);
        for (int i = 0; i < numofrooms; i++) {
            mkRoom(Random.Range(1, SIZE/2-1), Random.Range(1, SIZE/2-1), Random.Range(SIZE/2+1, SIZE-1), Random.Range(SIZE / 2+1, SIZE-1), 1);
        }
        int doorcount = 0;
        int windowCount = 0;
        for (int i = 0; i < SIZE; i++) {
            for (int j = 0; j < SIZE; j++) {
                //todo
                if (layout[i, j] == 1) {
                    int num = 0;

                    

                    for (int k = -1; k <= 1; k++) {
                        for (int l = -1; l <= 1; l++) {
                            try {
                                if (layout[i + k, j + l] == 0) {
                                    num++;
                                }
                            }
                            catch { };
                        }
                    }
                    
                    if (num > 1) {
                        layout[i, j] = 2;
                    }
                    if (num > 2) {
                        layout[i, j] = 3;
                        bool can = true;
                        for (int m = -1; m <= 1; m++) {
                            for (int n = -1; n <= 1; n++) {
                                if (!(n==0||m==0)) {
                                    if (layout[i + m, j + n] == 4 || layout[i + m, j + n] == 2 || layout[i + m, j + n] == 3) {
                                        can = false;
                                    }
                                }
                            }
                        }
                        if (Random.Range(0, 6) == 0 && doorcount < 2 && can) {
                            layout[i, j] = 9; // DOOR
                            doorcount++;
                        }
                        else if (Random.Range(0, 4) == 0 && windowCount < 8 && can) {
                            windowCount++;
                            layout[i, j] = 8; // WINDOW
                        }
                        }
                    if (num > 3) {
                        layout[i, j] = 4;
                    }
                    if (num > 4) {
                        layout[i, j] = 5;
                    }
                    if (num > 5) {
                        layout[i, j] = 6;
                    }
                    if (num > 6) {
                        layout[i, j] = 7;
                    }
                    if (num > 7) {
                        layout[i, j] = 8;
                    }
                    if (num > 8) {
                        layout[i, j] = 9;
                    }
                }
            }
        }
    }

    void GenerateStrukture() {
        print("im now generating");
        for (int i = 1; i < SIZE - 1; i++) {
            for (int j = 1; j < SIZE - 1; j++) {
                int richtung = 0;
                if (layout[i + 1, j] == 0) richtung = 0;
                if (layout[i - 1, j] == 0) richtung = 2;
                if (layout[i, j + 1] == 0) richtung = 3;
                if (layout[i, j - 1] == 0) richtung = 1;
                Vector3 rot = new Vector3(0, richtung * 90, 0);
                Vector3 pos = new Vector3(i, 0, j) + transform.position;
                switch (layout[i, j]) {
                    case 3:
                    //walls[1].transform.rotation
                    int diagonal = 0;
                    if (isAngleBlock(i + 1, j + 1) && isAngleBlock(i - 1, j - 1)) diagonal = 1;
                    if (isAngleBlock(i + 1, j - 1) && isAngleBlock(i - 1, j + 1)) diagonal = 2;
                    if (diagonal == 1)
                        Instantiate(walls[6], pos, Quaternion.Euler(Vector3.zero), transform);
                    else if (diagonal == 2) {
                        Vector3 roter = new Vector3(0, 90, 0);
                        var clone3 = Instantiate(walls[6], pos, Quaternion.Euler(roter), transform);
                    }
                    else
                        Instantiate(walls[1], pos, Quaternion.Euler(rot), transform);
                    break;
                    case 9:
                    Instantiate(walls[2], pos, Quaternion.Euler(rot), transform);
                    break;
                    case 8:
                    Instantiate(walls[3], pos, Quaternion.Euler(rot), transform);
                    break;
                    case 5:
                    Instantiate(walls[0], pos, Quaternion.identity, transform);
                    break;
                    case 2:
                    int grichtung = 0;
                    if (chValidotherBlock(layout[i + 1, j])) grichtung = 0;
                    if (chValidotherBlock(layout[i - 1, j])) grichtung = 2;
                    if (chValidotherBlock(layout[i, j + 1])) grichtung = 3;
                    if (chValidotherBlock(layout[i, j - 1])) grichtung = 1;
                    int r = grichtung - richtung;
                    if (r == -3) r = 1;
                    if (r == 3) r = -1;
                    if (r == 1)
                        Instantiate(walls[5], pos, Quaternion.Euler(rot), transform);
                    else
                    if (r == -1) {
                        var cloner = Instantiate(walls[5], pos, Quaternion.Euler(rot), transform);
                        cloner.transform.localScale = new Vector3(1, 1, -1);
                    }
                    else
                        print(r);
                    break;
                    case 4:
                    int rrichtung = 0;
                    if (layout[i + 1, j] == 1) rrichtung = 0;
                    if (layout[i - 1, j] == 1) rrichtung = 2;
                    if (layout[i, j + 1] == 1) rrichtung = 3;
                    if (layout[i, j - 1] == 1) rrichtung = 1;

                    int grichtung2 = 0;
                    if (chValidotherBlock(layout[i + 1, j])) grichtung2 = 0;
                    if (chValidotherBlock(layout[i - 1, j])) grichtung2 = 2;
                    if (chValidotherBlock(layout[i, j + 1])) grichtung2 = 3;
                    if (chValidotherBlock(layout[i, j - 1])) grichtung2 = 1;
                    int r3 = grichtung2 - rrichtung;
                    print(string.Format("{0},{1} r:{2}", i, j, grichtung2 - rrichtung));
                    if (r3 == -3) r3 = 1;
                    if (r3 == 3) r3 = -1;
                    GameObject clone = null;
                    Vector3 rot2 = new Vector3(0, rrichtung * 90, 0);
                    if (r3 == 1)
                        clone = Instantiate(walls[5], pos, Quaternion.Euler(rot2), transform);
                    else
                    if (r3 == -1) {
                        clone = Instantiate(walls[5], pos, Quaternion.Euler(rot2), transform);
                        clone.transform.localScale = new Vector3(1, 1, -1);
                    }


                    break;
                }
            }
        }
    }

    bool chValidBlock(int id) {
        return (id == 3 || id == 5 || id == 9 || id == 8);
    }

    bool chValidotherBlock(int id) {
        return (id == 3 || id == 5 || id == 9 || id == 8|| id == 2||id == 4);
    }

    bool isAngleBlock(int x,int y) {
        return layout[x, y] == 2 || layout[x, y] == 4 || layout[x,y] == 3;
    }

    void mkRoom(int x,int y,int x2,int y2,int v) {
        for (int i = x; i < x2; i++) {
            for (int j = y; j < y2; j++) {
                layout[i, j] = v;
            }
        }
    }

    [PunRPC]
    void finishedGenerating(int[] result) {
        for (int x = 0; x < SIZE; x++) {
            for (int y = 0; y < SIZE; y++) {
                layout[x, y] = result[x + y * SIZE];
            }
        }
        
        hasGenerated = true;
        print("must generate now ---");
        GenerateStrukture();
    }

    void OnPhotonSerializeView(PhotonStream stream, PhotonMessageInfo info) {
        if (stream.isWriting) {

        }
        else {

        }
    }
    private void OnDrawGizmosSelected() {
        for (int i = 0; i < SIZE; i++) {
            for (int j = 0; j < SIZE; j++) {
                Color[] colors = { new Color(0, 1, 0, 0), Color.red, Color.blue ,Color.green,Color.black,Color.cyan,Color.gray
                ,Color.yellow,Color.white,new Color(1,1,0),new Color(0,1,1)};
                Gizmos.color = colors[layout[i, j]];
                Gizmos.DrawCube(new Vector3(i, 0, j)+transform.position,new Vector3(1,0,1));
            }
        }
    }
}
