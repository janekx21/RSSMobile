using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ClassChoserOp : MonoBehaviour {
    public OperatorConst local;
    public Text nameText;
    public Text codeNameText;
    public Text ageText;
    public Image portrai;
    public Image background;
	// Use this for initialization
	void Start () {
        if (!local) {
            Destroy(gameObject);
            return;
        }
        nameText.text = local.realName;
        codeNameText.text = local.codeName;
        ageText.text = local.alter.ToString();
        portrai.sprite = local.portrai;
    }
	
	// Update is called once per frame
	void Update () {
		if(IngameUI.instance.selected == this) {
            background.color = Color.red;
        }
        else {
            background.color = Color.white;
        }
	}
}
