using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

public class Player_CTRL : NetworkBehaviour
{
    public float speed = 2.0f;
    public float rotSpeed = 80f;
    public float rot = 0f;
    public float gravity = 8.0f;

    public float mouseSensitive = 1000.0f;
    public Transform playerBody;
    float xRotation = 0f;
    Vector3 moveDir = Vector3.zero;

    CharacterController controller;

    public GameObject bulletPrefeb;
    public Transform bulletSpawn;

    void Start()
    {
        controller = GetComponent<CharacterController>();
        if (isLocalPlayer)
        {
            Cursor.lockState = CursorLockMode.Locked;
            {
                Camera.main.transform.LookAt(this.transform.position);
                Camera.main.transform.position = this.transform.position - this.transform.up * -0.3f + this.transform.forward * 0.1f /*+ this.transform.up * 0.5f*/;
                Camera.main.transform.parent = this.transform;
            }
        }
    }

    void Update()
    {
        if(!isLocalPlayer)
        {
            return;
        }

        if (Input.GetKey(KeyCode.W))
        {
            moveDir = new Vector3(0, 0, 1);
            moveDir *= speed;
            moveDir = transform.TransformDirection(moveDir);
        }
        else if (Input.GetKey(KeyCode.S))
        {
            moveDir = new Vector3(0, 0, -1);
            moveDir *= speed;
            moveDir = transform.TransformDirection(moveDir);
        }
        /*else if (Input.GetKey(KeyCode.A))
        {
            moveDir = new Vector3(-1, 0, 0);
            moveDir *= speed;
            moveDir = transform.TransformDirection(moveDir);
        }
        else if (Input.GetKey(KeyCode.D))
        {
            moveDir = new Vector3(1, 0, 0);
            moveDir *= speed;
            moveDir = transform.TransformDirection(moveDir);
        }*/
        else
        {
            moveDir = Vector3.zero;
        }

        rot = Input.GetAxis("Mouse X") * mouseSensitive * Time.deltaTime;

        xRotation = Mathf.Clamp(xRotation, -180f, 180f);

        playerBody.Rotate(Vector3.up * rot);

        moveDir.y -= gravity * Time.deltaTime;
        controller.Move(moveDir * Time.deltaTime);

        if (Input.GetKeyDown(KeyCode.Space))
        {
            CmdFire();
        }
    }


    [Command]
    void CmdFire()
    {
        GameObject bullet = (GameObject)Instantiate(bulletPrefeb,bulletSpawn.position,bulletSpawn.rotation);

        bullet.GetComponent<Rigidbody>().velocity = bullet.transform.forward*6.0f;

        NetworkServer.Spawn(bullet);

        Destroy(bullet,2);
    }

    public override void OnStartLocalPlayer()
    {
        GetComponent<MeshRenderer>().material.color = Color.blue;
    }
}
