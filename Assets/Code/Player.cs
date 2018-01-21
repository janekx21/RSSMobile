using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : MonoBehaviour {
    public Camera cam;
    public static Player localPlayer;
    public OperatorConst local;
    public PhotonView photonView;
    CharacterController chara;

    public Animator gunAnim;
    public AudioSource gunAudio;
    public HitBox[] hitBoxes;
    public GameObject gunPoint;
    AudioSource ownAudio;
    public AudioClip hitmarker;
    public AudioClip getHitSound;

    private WeaponConst _weaponConst;
    [SerializeField]
    public WeaponConst weaponConst { get { return _weaponConst; }
        set {
            this._weaponConst = value;
            for (int i = 0; i < gunPoint.transform.childCount; i++) {
                Destroy(gunPoint.transform.GetChild(i).gameObject);
            }
            

            var clone = Instantiate(_weaponConst.model, Vector3.zero, Quaternion.identity, gunPoint.transform);
            gunAnim = clone.GetComponent<Animator>();   
            clone.transform.localPosition = Vector3.zero;
            clone.transform.localRotation = Quaternion.identity;

            fireRate = _weaponConst.fireRate;
            damage = _weaponConst.damage;
            amunition = _weaponConst.maxAmunition;
            magazin = _weaponConst.maxMagazin;
        }
    }
    float fireRate = 300; //RPM
    float damage = 1f;

    float lastShoot = 0f;
    float reloadTimer = 0f;
    bool isReloading = false;
    public int magazin = 0;
    public int amunition = 0;
    

    public float movementSpeed = 1;
   // public float maxSpeed = 2;
    public float gravity = .1f;
    public float jumpHeight = 1f;
    public Vector3 mov = Vector3.zero;
    bool previouslyGroundet = false;

    public float hp = 10;

    public Animator playerAnimator;
    float horizontalMovement = 0;
    float verticalMovement = 0;
    public GameObject[] outsideRepr;

    public bool mine = false;
    public bool dev;
    // Use this for initialization
    private void Awake() {
        if (dev) {
        //    Destroy(gameObject);
         //   return;
        }
        photonView = GetComponent<PhotonView>();
        chara = GetComponent<CharacterController>();
        ownAudio = GetComponent<AudioSource>();
        foreach (var item in hitBoxes) {
            item.player = this;
        }
    }

    void OnJoinedRoom() {
        if (dev) {
            photonView.TransferOwnership(PhotonNetwork.player);
        }
    }

    void Start () {


        if (mine != photonView.isMine) {
            Debug.LogWarning("mine error");
        }

        if (mine) {
            Player.localPlayer = this;
            foreach (var item in outsideRepr) {
                item.SetActive(false);
            }
            
        }
        
        weaponConst = local.defaultWeapon;
        
        hp = local.maxHp;
        movementSpeed = local.speed;
    }

    void OnPhotonSerializeView(PhotonStream stream, PhotonMessageInfo info) {
        if (stream.isWriting) {
            stream.SendNext(hp);
            stream.SendNext(System.Array.IndexOf(Manager.instance.allOperators, local));
            stream.SendNext(System.Array.IndexOf(Manager.instance.allWeapons, weaponConst));
            stream.SendNext(verticalMovement);
            stream.SendNext(horizontalMovement);
        }
        else {
           hp = (float)stream.ReceiveNext();
            local = (OperatorConst)Manager.instance.allOperators[(int)stream.ReceiveNext()];
            weaponConst = (WeaponConst)Manager.instance.allWeapons[(int)stream.ReceiveNext()];
            verticalMovement = Mathf.Lerp(verticalMovement,(float)stream.ReceiveNext(),.5f);
            horizontalMovement = Mathf.Lerp(horizontalMovement,(float)stream.ReceiveNext(),.5f);
        }
    }

    // Update is called once per frame
    void Update () {
        if (mine) {
            lastShoot += Time.deltaTime;
            if (Cursor.lockState == CursorLockMode.Locked) {
                transform.Rotate(Vector3.up, Input.GetAxis("Mouse X"));
                cam.transform.Rotate(Vector3.right, -Input.GetAxis("Mouse Y"));
            }

            if (Input.GetMouseButtonDown(0)) {
                Cursor.lockState = CursorLockMode.Locked;
            }
            if (Input.GetKeyDown(KeyCode.Escape)) {
                Cursor.lockState = CursorLockMode.None;
            }
            

            if (Input.GetMouseButton(0)) {
                if (lastShoot > (60f/ fireRate)) {
                    if (!isReloading) {
                        if (magazin > 0) {

                            Shoot();
                            magazin--;

                        }
                        else {
                            Reload();
                        }
                    }
                }
            }

            if (Input.GetKeyDown(KeyCode.R)) {
                if(!isReloading)
                    Reload();
            }

            

            var zmov = Vector3.zero;
            zmov.x = Input.GetAxis("Horizontal");
            zmov.z = Input.GetAxis("Vertical");
            zmov = Vector3.ClampMagnitude(zmov, 1);
            horizontalMovement = Mathf.Lerp(horizontalMovement, zmov.x, .1f);
            verticalMovement = Mathf.Lerp(verticalMovement, zmov.z, .1f);
            zmov *= movementSpeed * Time.deltaTime * 50;
            zmov = Quaternion.Euler(0, transform.rotation.eulerAngles.y, 0) * zmov;

            mov.x = zmov.x;
            mov.z = zmov.z;
            if (!chara.isGrounded)
                mov.y -= gravity * Time.deltaTime * 50;
            if(chara.isGrounded && previouslyGroundet)
                if(mov.y <0)
                    mov.y = -gravity; // TODO

            if (chara.isGrounded) {
                if (Input.GetButtonDown("Jump")) {
                    mov.y = jumpHeight;
                }
            }
            chara.Move(mov);
            previouslyGroundet = chara.isGrounded;
            if(isReloading) {
                reloadTimer -= Time.deltaTime;
            }
            if(reloadTimer < 0 && isReloading) {
                isReloading = false;
                reloadTimer = 0;
                FinishedReloading();
            }

            

        }
        playerAnimator.SetFloat("WalkingHorizontal", horizontalMovement);
        playerAnimator.SetFloat("WalkingVertical", verticalMovement);
        

    }

    void Die() {
        //Destroy(gameObject);

        transform.position = Manager.instance.GetRandomSpawn();
        hp = local.maxHp;
        weaponConst = weaponConst;
        //Manager.instance.currentMode = "room";
    }

    [PunRPC]
    void getHit(float damage,Vector3 origen) {

        hp -= damage;
        if (hp <= 0) {
            Die();
        }
        print("im getting hit");
        //    print(hp);
        Vector3 pos = origen - cam.transform.position;
        pos.y = 0;
        IngameUI.instance.GotHit(pos.normalized);
        ownAudio.PlayOneShot(getHitSound);
    }


    void Shoot() {
        var r = new Ray(cam.transform.position, cam.transform.forward);
        RaycastHit hit = new RaycastHit();
        if(Physics.Raycast(r, out hit,LayerMask.GetMask("HitBox"))) {

            HitBox hitbox = hit.collider.GetComponent<HitBox>();
            if (hitbox) {
                Player other = hitbox.player;
                Breakable breakable = hitbox.breakable;
                if (other) {
                    print("you hit -|-");
                    ownAudio.PlayOneShot(hitmarker);
                    PhotonNetwork.RPC(other.photonView, "getHit", other.photonView.owner, false, damage,cam.transform.position);//1 is damage
                }else if (breakable) {
                    breakable.getHitLocal(damage);
                }
            }
            else {
                PhotonNetwork.Instantiate("devBall", hit.point, Quaternion.LookRotation(hit.normal), 0);
            }
            
        }
        gunAudio.pitch = Random.Range(.98f, 1.02f);
        gunAudio.PlayOneShot(weaponConst.shootSound);
        
        PhotonNetwork.RPC(photonView, "ShootSound", PhotonTargets.Others, false);
        gunAnim.SetFloat("rate",1/( 60f / fireRate));
        gunAnim.Play("gunShoot");
        playerAnimator.Play("Armature|shootArms", 1);
        lastShoot = 0f;
    }

    void Reload() {
        if (magazin == weaponConst.maxMagazin) return;
        if (amunition == 0) return;
        reloadTimer = weaponConst.reloadTime;
        isReloading = true;
        gunAnim.Play("gunReload");
        playerAnimator.Play("Armature|reloadArms", 1);
        PhotonNetwork.RPC(photonView, "ReloadTrigger", PhotonTargets.Others, false);
    }

    [PunRPC]
    void ReloadTrigger() {
        playerAnimator.Play("Armature|reloadArms", 1);
    }
    void FinishedReloading() {
        int rest = amunition - weaponConst.maxMagazin + magazin;
        print(rest);
        if(rest < weaponConst.maxMagazin) {
            if(rest >0)
                magazin = Mathf.Abs(rest);
            amunition = 0;
        }
        else {
            magazin = weaponConst.maxMagazin;
            amunition = rest;
        }
    }

    [PunRPC]
    void ShootSound() {
        gunAudio.pitch = Random.Range(.98f, 1.02f);
        gunAudio.PlayOneShot(weaponConst.shootSound);
        playerAnimator.Play("Armature|shootArms", 1);
    }
    /*
    [PunRPC]
    void mainGetHit(float damage ,PhotonPlayer target) {
        print(string.Format("{0} got hit with {1} damage",  target.ToStringFull(),damage));
        PhotonNetwork.RPC(photonView, "getHit", target, false, damage);
    }*/
}
