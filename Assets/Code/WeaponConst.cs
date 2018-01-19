using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "newWeapon", menuName = "Weapon", order = -2)]
public class WeaponConst : ScriptableObject {
    public string weaponName;
    public float damage = 1;
    public int maxMagazin =30;
    public int maxAmunition = 300;
    public float fireRate = 300;
    public float reloadTime = 3f;
    public GameObject model;
    public AudioClip shootSound;
    public AudioClip reloadSound;
    public Sprite weaponSideView;
    public int id = -1;
}

