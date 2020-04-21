using System.Collections;
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
    public GameObject bullet;
    public Transform firePoint;
    public Gun activeGun;
    private void Awake()
    {
        instance = this;
    }
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        float yStore = moveInput.y;
        Vector3 vertMove = transform.forward * Input.GetAxisRaw("Vertical");
        Vector3 horiMove = transform.right * Input.GetAxisRaw("Horizontal");
        moveInput = horiMove + vertMove;
        moveInput.Normalize();
        if(Input.GetKey(KeyCode.LeftShift))
        {
            moveInput = moveInput * runSpeed;
        }
        else
        {
            moveInput = moveInput * moveSpeed;
        }
        moveInput.y = yStore;
        moveInput.y += Physics.gravity.y * gravityModifier * Time.deltaTime;
        if(charCon.isGrounded)
        {
            moveInput.y = Physics.gravity.y * gravityModifier * Time.deltaTime;
        }
        canJump = Physics.OverlapSphere(groundCheckPoint.position, .25f, whatIsGround).Length > 0;
        if(canJump)
        {
            canDoubleJump = true;//false;
        }
        if(Input.GetKeyDown(KeyCode.Space) && canJump)
        {
            moveInput.y = jumpPower;
            canDoubleJump = true;
        }
        else if(canDoubleJump && Input.GetKeyDown(KeyCode.Space))
        {
            moveInput.y = jumpPower;
            canDoubleJump = false;
        }
        charCon.Move(moveInput * Time.deltaTime);
        Vector2 mouseInput = new Vector2(Input.GetAxisRaw("Mouse X"), Input.GetAxisRaw("Mouse Y")) * mouseSensitivity;
        if(invertX)
        {
            mouseInput.x = -mouseInput.x;
        }
        if (invertY)
        {
            mouseInput.y = -mouseInput.y;
        }
        transform.rotation = Quaternion.Euler(transform.rotation.eulerAngles.x, transform.rotation.eulerAngles.y + mouseInput.x, transform.rotation.eulerAngles.z);
        camTrans.rotation = Quaternion.Euler(camTrans.rotation.eulerAngles + new Vector3(-mouseInput.y, 0f, 0f));
        if(Input.GetMouseButtonDown(0))
        {
            RaycastHit hit;
            if(Physics.Raycast(camTrans.position, camTrans.forward, out hit, 50f))
            {
                if(Vector3.Distance(camTrans.position, hit.point) > 2f)
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
        if(Input.GetMouseButton(0) && activeGun.canAutoFire)
        {
            if(activeGun.fireCounter <=0)
            {
                FireShot();
            }
        }
        anim.SetFloat("moveSpeed", moveInput.magnitude);
        anim.SetBool("onGround", canJump);
    }

    public void FireShot()
    {
        Instantiate(activeGun.bullet, firePoint.position, firePoint.rotation);
        activeGun.fireCounter = activeGun.fireRate;
    }
}
