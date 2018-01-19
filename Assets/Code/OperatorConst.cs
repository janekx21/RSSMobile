using System;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "newOperator", menuName = "Operator", order = -2)]
public class OperatorConst : ScriptableObject{
    public string realName;
    public string codeName;
    public int alter;
    public Sprite portrai;
    public Sprite charImage;
    public float maxHp=10;
    public WeaponConst defaultWeapon;
    public int id = -1;
    public float speed = .1f;
    /*
    public OperatorConst(string n,int a) {
        realName = n;
        alter = a;
    }*/
}

