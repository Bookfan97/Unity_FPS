﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    public static PlayerController instance;
    public float moveSpeed, gravityModifier, jumpPower, runSpeed = 12f;
    public CharacterController charCon;
    private Vector3 moveInput;
    public Transform camTrans;
    public float mouseSensitivity;
    public bool invertX, invertY;
    private bool canJump, canDoubleJump;
    public Transform groundCheckPoint;
    public LayerMask whatIsGround;
    public Animator anim;
    //public GameObject bullet;
    public Transform firePoint;
    public Gun activeGun;
    public List<Gun> allGuns = new List<Gun>();
    public List<Gun> unlockableGuns = new List<Gun>();
    public int currentGun;
    public Transform ADSPoint, gunHolder;
    private Vector3 gunStartPos;
    public float ADSSpeed = 2f;
    public AudioSource footstepFast, footstepSlow;
    private float bounceAmount;
    private bool bounce;
    private void Awake()
    {
        instance = this;
    }
    // Start is called before the first frame update
    void Start()
    {
        currentGun--;
        SwitchGun();
        gunStartPos = gunHolder.localPosition;
    }


    // Update is called once per frame
    void Update()
    {
        if (!UIController.instance.pauseScreen.activeInHierarchy)
        {
            float yStore = moveInput.y;
            Vector3 vertMove = transform.forward * Input.GetAxisRaw("Vertical");
            Vector3 horiMove = transform.right * Input.GetAxisRaw("Horizontal");
            moveInput = horiMove + vertMove;
            moveInput.Normalize();
            if (Input.GetKey(KeyCode.LeftShift))
            {
                moveInput = moveInput * runSpeed;
            }
            else
            {
                moveInput = moveInput * moveSpeed;
            }
            moveInput.y = yStore;
            moveInput.y += Physics.gravity.y * gravityModifier * Time.deltaTime;
            if (charCon.isGrounded)
            {
                moveInput.y = Physics.gravity.y * gravityModifier * Time.deltaTime;
            }
            canJump = Physics.OverlapSphere(groundCheckPoint.position, .25f, whatIsGround).Length > 0;
            if (canJump)
            {
                canDoubleJump = true;//false;
            }
            if (Input.GetKeyDown(KeyCode.Space) && canJump)
            {
                moveInput.y = jumpPower;
                canDoubleJump = true;
                AudioManager.instance.PlaySFX(8);
            }
            else if (canDoubleJump && Input.GetKeyDown(KeyCode.Space))
            {
                moveInput.y = jumpPower;
                canDoubleJump = false;
                AudioManager.instance.PlaySFX(8);
            }
            if(bounce)
            {
                bounce = false;
                moveInput.y = bounceAmount;
                canDoubleJump = true;
            }
            charCon.Move(moveInput * Time.deltaTime);
            Vector2 mouseInput = new Vector2(Input.GetAxisRaw("Mouse X"), Input.GetAxisRaw("Mouse Y")) * mouseSensitivity;
            if (invertX)
            {
                mouseInput.x = -mouseInput.x;
            }
            if (invertY)
            {
                mouseInput.y = -mouseInput.y;
            }
            transform.rotation = Quaternion.Euler(transform.rotation.eulerAngles.x, transform.rotation.eulerAngles.y + mouseInput.x, transform.rotation.eulerAngles.z);
            camTrans.rotation = Quaternion.Euler(camTrans.rotation.eulerAngles + new Vector3(-mouseInput.y, 0f, 0f));
            if (Input.GetMouseButtonDown(0) && activeGun.fireCounter <= 0)
            {
                RaycastHit hit;
                if (Physics.Raycast(camTrans.position, camTrans.forward, out hit, 50f))
                {
                    if (Vector3.Distance(camTrans.position, hit.point) > 2f)
                    {
                        firePoint.LookAt(hit.point);
                    }
                }
                else
                {
                    firePoint.LookAt(camTrans.position + (camTrans.forward * 30));
                }
                //Instantiate(bullet, firePoint.position, firePoint.rotation);
                FireShot();
            }
            if (Input.GetMouseButton(0) && activeGun.canAutoFire)
            {
                if (activeGun.fireCounter <= 0)
                {
                    FireShot();
                }
            }
            if (Input.GetKeyDown(KeyCode.Tab))
            {
                SwitchGun();
            }
            if (Input.GetMouseButtonDown(1))
            {
                CameraController.instance.ZoomIn(activeGun.zoomAmount);
            }
            if (Input.GetMouseButton(1))
            {
                gunHolder.position = Vector3.MoveTowards(gunHolder.position, ADSPoint.position, ADSSpeed * Time.deltaTime);
            }
            else
            {
                gunHolder.localPosition = Vector3.MoveTowards(gunHolder.localPosition, gunStartPos, ADSSpeed * Time.deltaTime);
            }
            if (Input.GetMouseButtonUp(1))
            {
                CameraController.instance.ZoomOut();
            }
            anim.SetFloat("moveSpeed", moveInput.magnitude);
            anim.SetBool("onGround", canJump);
        }
    }

    public void FireShot()
    {
        activeGun.currentAmmo--;
        if (activeGun.currentAmmo > 0)
        {
            Instantiate(activeGun.bullet, firePoint.position, firePoint.rotation);
            activeGun.fireCounter = activeGun.fireRate;
            //UIController.instance.ammoText.text = "AMMO: " + activeGun.currentAmmo;
        }
        else
        {
            activeGun.currentAmmo = 0;
        }
        UIController.instance.ammoText.text = "AMMO: " + activeGun.currentAmmo;
    }

    private void SwitchGun()
    {
        activeGun.gameObject.SetActive(false);
        currentGun++;
        if(currentGun>=allGuns.Count)
        {
            currentGun = 0;
        }
        activeGun = allGuns[currentGun];
        Debug.Log("Current Gun: " + currentGun);
        activeGun.gameObject.SetActive(true);
        UIController.instance.ammoText.text = "AMMO: " + activeGun.currentAmmo;
        firePoint.position = activeGun.firePoint.position;
    }
    public void AddGun(string gunToAdd)
    {
        bool gunUnlocked = false;
        if(unlockableGuns.Count >0)
        {
            for(int i=0; i<unlockableGuns.Count; i++)
            {
                if(unlockableGuns[i].gunName == gunToAdd)
                {
                    gunUnlocked = true;
                    allGuns.Add(unlockableGuns[i]);
                    unlockableGuns.RemoveAt(i);
                    i = unlockableGuns.Count;
                }
            }
        }
        if(gunUnlocked)
        {
            currentGun = allGuns.Count - 2;
            SwitchGun();
        }
    }
    public void Bounce(float bounceForce)
    {
        bounceAmount = bounceForce;
        bounce = true;
    }
}
