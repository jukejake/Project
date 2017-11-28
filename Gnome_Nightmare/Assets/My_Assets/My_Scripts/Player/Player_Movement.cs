﻿using UnityEngine;

public class Player_Movement : MonoBehaviour {

    public GameObject Menu_Prefab;

    public float moveSpeed = 10.0f;
    public float SensitivityXAxis = 15.0f;
    public float SensitivityYAxis = 10.0f;
    public Vector3 Jump = new Vector3(3.0f, 0.0f, 10.0f); //MinJump,JumpPressure, MaxJumpPressure
    public Animator anim;
    
    private Rigidbody m_Rigidbody;
    //private AudioSource m_audioSource;

	private Vector3 moveDirection = Vector3.zero;
	private float rotX;
    private float rotY;
    public float animPer_H;
    public float animPer_V;
    private float RotationNeededForCamera;

    private bool m_IsGrounded = true;

    // Use this for initialization
    void Start () {
        //Spawns an empty gameObject named [World]
        GameObject EmptyWorld = GameObject.Find("World");
        //Spawns Prefab used for the Menu
        GameObject Menu = (GameObject)Instantiate(Menu_Prefab);
        Menu.transform.SetParent(EmptyWorld.transform);
        Menu.name = Menu_Prefab.name;
        //Spawns Prefab used for the Player
        this.transform.SetParent(EmptyWorld.transform);
        this.name = "Player";

        m_Rigidbody = gameObject.GetComponent<Rigidbody>();
        //m_audioSource = gameObject.GetComponent<AudioSource>();
        Camera.main.GetComponent<CameraFollow>().SetPosition(this.gameObject.transform);
    }

    // Update is called once per frame
    void Update() {
        //If the player is in a menu they wont move
        if (this.gameObject.GetComponent<PlayerManager>().MenuOpen) { return; }
        if (this.gameObject.GetComponent<PlayerStats>().isDead) { return; }

        //Get the Xaxis rotation
        if (Input.GetAxis("Right Joystick X") != 0.0f) { rotX = Input.GetAxis("Right Joystick X") * SensitivityXAxis; }
        else { rotX = Input.GetAxis("Mouse X") * SensitivityXAxis; }

        //Get the Yaxis rotation
        if (Input.GetAxis("Right Joystick X") != 0.0f) { rotY += Input.GetAxis("Right Joystick Y") * SensitivityYAxis; }
        else { rotY -= Input.GetAxis("Mouse Y") * SensitivityYAxis; }

        //Clamp the camera rotation in the Yaxis
        rotY = Mathf.Clamp(rotY, -60f, 70f);  //high, low

        //Rotate player
        transform.Rotate(0, rotX, 0);
        //Rotate camera
        RotationNeededForCamera += rotX;
        Camera.main.GetComponent<CameraFollow>().SetRotation(new Vector2(RotationNeededForCamera, rotY));
    }
    
    void FixedUpdate() {
        //If the player is in a menu they wont move
        if (this.gameObject.GetComponent<PlayerManager>().MenuOpen) { return; }
        if (this.gameObject.GetComponent<PlayerStats>().isDead) { return; }
        
        //Get movement
        moveDirection = new Vector3(Input.GetAxis("Horizontal") * moveSpeed, 0.0f, Input.GetAxis("Vertical") * moveSpeed);

        //Holding jump button
        if (Input.GetButton("Jump") && m_IsGrounded) {
            if (Jump.y < Jump.z) { Jump.y += moveSpeed * Time.deltaTime; }
            else { Jump.y = Jump.z; }
        }
        //Not holding jump button
        else {
            if (Jump.y > 0.0f) {
                Jump.y = Jump.y + (Jump.x*2.0f);
                moveDirection.y = Jump.y;
                Jump.y -= Jump.x * 2.15f;
                //Jump.y = 0.0f;
            }
            else if (Jump.y < 0.0f) {
                Jump.y = 0.0f;
            }
        }

        //If the player is grounded do not apply additional gravity
        if (m_IsGrounded) { moveDirection.y -= 0.0f * Time.deltaTime; }
        else { moveDirection.y -= 30.0f * Time.deltaTime; }

        //Move player
        moveDirection = transform.rotation * moveDirection * 0.65f;
        m_Rigidbody.MovePosition(m_Rigidbody.position + moveDirection * Time.deltaTime);

        //Update Animator Perameters
        animPer_H = Input.GetAxis("Horizontal");
        animPer_V = Input.GetAxis("Vertical");
        anim.SetFloat("inputH", animPer_H);
        anim.SetFloat("inputV", animPer_V);

    }

    void OnCollisionEnter(Collision collision) {
        //Needs to fix as it allows wall jumping
        if (collision.contacts.Length > 0) {
            if (Vector3.Dot(transform.up, collision.contacts[0].normal) > 0.5f) {
                m_IsGrounded = true;
            }
        }
    }
}
