using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using UnityEngine.UI;


public class IngameUI : MonoBehaviour {

    public static IngameUI instance;

    public GameObject classChoser;
    public GameObject loaderLogo;
    public GameObject ingame;
    public Transform choserContent;
    public Image charImage;
    public RectTransform[] crosses;
    public Image bloodyScreen;
    public Text ping;
    public Text magazinText;
    public Text ammunitionText;

    public GameObject hitThing;
    public GameObject hitSprite;

    class GHit {
        public float time = 0f;
        public Vector2 o;
        public GameObject sprite;
        public GHit(Vector2 o,GameObject sprite) {
            this.o = o;
            this.sprite = sprite;
        }
    }
    List<GHit> hits;


    public ClassChoserOp selected;
    // Use this for initialization

    public GameObject classChoserOperator;  //Prefap

    public Button spawnButton;

    List<ClassChoserOp> chosers;


    private void Awake() {
        instance = this;
        hits = new List<GHit>();
    }

    void Start () {
        chosers = new List<ClassChoserOp>();
       // Destroy(choserContent.GetChild(0).gameObject);

        for (int i=0;i< Manager.instance.allOperators.Length;i++) {
            var item = Instantiate(classChoserOperator, new Vector2(0, 0), Quaternion.identity, choserContent);
            /////////////////////////////////////////// Position is not local so ejust new lower
            var clc = item.GetComponent<ClassChoserOp>();
            clc.local = Manager.instance.allOperators[i];

            item.transform.localPosition = new Vector2(0, i * -75);



            item.GetComponent<Button>().onClick.AddListener(() => choseClass(clc));
            chosers.Add(clc);
        }
        spawnButton.onClick.AddListener(spawnWithSelectedClass);

    }

    void choseClass(ClassChoserOp caller) {
        selected = caller;

    }

    void spawnWithSelectedClass() {
        if (selected)
            Manager.instance.SpawnWitch(selected.local);
        else
            print("dint select");
    }


	
	// Update is called once per frame
	void Update () {
        string mode = Manager.instance.currentMode;

            classChoser.SetActive(mode == "room");

            loaderLogo.SetActive(mode == "waiting");

            ingame.SetActive(mode == "playing");

        if(mode == "playing") {
            if (Player.localPlayer) {
                foreach (var item in crosses) {
                    item.localPosition = new Vector2(0, Player.localPlayer.mov.magnitude * 500 / Time.deltaTime / 80);
                }
                magazinText.text = Player.localPlayer.magazin.ToString();
                ammunitionText.text = Player.localPlayer.amunition.ToString();
                bloodyScreen.color = new Color(0, 0, 0, 1 - Player.localPlayer.hp / Player.localPlayer.local.maxHp);
            }
            
        }


        if (selected) {
            charImage.sprite = selected.local.charImage;
        }

        ping.text = string.Format("Ping: {0}", PhotonNetwork.GetPing());

        for (int i = 0; i < hits.Count; i++) {
            GHit item = hits[i];
            UpdateGHit(item);
            
        }

    }

    void UpdateGHit(GHit item) {
        item.time += Time.deltaTime;
        if (item.time > 1f) {
            hits.Remove(item);
            Destroy(item.sprite);
        }
        Vector3 cam = Player.localPlayer.cam.transform.forward;
        Vector2 cam2 = new Vector2(cam.x, cam.z);
        float angle = Vector2.Angle(cam2, item.o);
        Vector2 pos = item.o;
        pos = Quaternion.Euler(0, 0, Player.localPlayer.cam.transform.rotation.eulerAngles.y) * pos;
        if ((cam2 - item.o).x < 0) {
            angle = -angle;
        }
        //item.sprite.transform.rotation = Quaternion.Euler(new Vector3(0,0,angle));
        //item.sprite.transform.localPosition = pos.normalized * 100;
        item.sprite.transform.rotation = Quaternion.Euler( 0,0,Vector2.SignedAngle(Vector2.up, pos.normalized));
        item.sprite.GetComponent<Image>().color = new Color(1,1,1,1 - item.time);
    }

    public void GotHit(Vector3 o) { // angle to Vector3.forward
        var clone = Instantiate(hitSprite, Vector3.zero, Quaternion.identity, hitThing.transform);
        clone.transform.localPosition = Vector3.zero;
        GHit g = new GHit(new Vector2(o.x, o.z), clone);
        UpdateGHit(g);
        hits.Add(g);
    }
}
