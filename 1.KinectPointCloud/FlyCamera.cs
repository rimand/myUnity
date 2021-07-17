using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FlyCamera : MonoBehaviour
{
    /*
    Writen by Windexglow 11-13-10.  Use it, edit it, steal it I don't care.  
    Converted to C# 27-02-13 - no credit wanted.
    Simple flycam I made, since I couldn't find any others made public.  
    Made simple to use (drag and drop, done) for regular keyboard layout  
    wasd : basic movement
    shift : Makes camera accelerate
    space : Moves camera on X and Z axis only.  So camera doesn't gain any height*/


    float mainSpeed = 5.0f; //regular speed
    float shiftAdd = 10.0f; //multiplied by how long shift is held.  Basically running
    float maxShift = 20.0f; //Maximum speed when holdin gshift
    public float camSens = 0.25f; //How sensitive it with mouse
    private Vector3 lastMouse = new Vector3(255, 255, 255); //kind of in the middle of the screen, rather than at the top (play)
    private float totalRun = 1.0f;
    private bool state = true;

    public KeyCode Jump;

    public bool OnGround = true;

    public float jumpForce;
    public float jumpDuration;
    private Rigidbody rigidbody;

    float lastTime;

    void Start()
    {
        jumpForce = 500;
        jumpDuration = 1;

        rigidbody = GetComponent<Rigidbody>();
        lastTime = Time.time;
    }


    void Update()
    {
        if(Time.time - lastTime >= 1)
        {
            Debug.Log("Ground : " + OnGround);
        }

        if (Input.GetMouseButton(1)) state = true;
        else state = false;

        if (Input.GetKey(KeyCode.Q))
        {
            transform.eulerAngles += new Vector3(0, -1, 0);
        }

        if (Input.GetKey(KeyCode.E))
        {
            transform.eulerAngles += new Vector3(0, 1, 0);
        }

        lastMouse = Input.mousePosition - lastMouse;
        lastMouse = new Vector3(-lastMouse.y * camSens, lastMouse.x * camSens, 0);
        lastMouse = new Vector3(transform.eulerAngles.x + lastMouse.x, transform.eulerAngles.y + lastMouse.y, 0);

        if (state)
        {
            transform.eulerAngles = lastMouse;
        }

        lastMouse = Input.mousePosition;

        //Mouse camera angle done.  

        //Keyboard commands
        Vector3 p = GetBaseInput();
        if (p.sqrMagnitude > 0)
        { // only move while a direction key is pressed
            if (Input.GetKey(KeyCode.LeftShift))
            {
                totalRun += Time.deltaTime;
                p = p * totalRun * shiftAdd;
                p.x = Mathf.Clamp(p.x, -maxShift, maxShift);
                p.y = Mathf.Clamp(p.y, -maxShift, maxShift);
                p.z = Mathf.Clamp(p.z, -maxShift, maxShift);
            }
            else
            {
                totalRun = Mathf.Clamp(totalRun * 0.5f, 1f, 1000f);
                p = p * mainSpeed;
            }

            p = p * Time.deltaTime;
            Vector3 newPosition = transform.position;

            //if (Input.GetKey(KeyCode.Space))
            //{ //If player wants to move on X and Z axis only
            //    transform.Translate(p);
            //    newPosition.x = transform.position.x;
            //    newPosition.z = transform.position.z;
            //    transform.position = newPosition;
            //}
            //else
            //{
            //    transform.Translate(p);
            //}

            transform.Translate(p);
        }

        if (Input.GetKey(KeyCode.Space) && OnGround == true)
        {
            rigidbody.AddForce(Vector3.up * jumpForce);
            OnGround = false;
        }
    }

    private Vector3 GetBaseInput()
    { 
        Vector3 p_Velocity = new Vector3();
        if (Input.GetKey(KeyCode.W))
        {
            p_Velocity += new Vector3(0, 0, 1);
            rigidbody.velocity = Vector3.zero;
            rigidbody.angularVelocity = Vector3.zero;
        }
        if (Input.GetKey(KeyCode.S))
        {
            p_Velocity += new Vector3(0, 0, -1);
            rigidbody.velocity = Vector3.zero;
            rigidbody.angularVelocity = Vector3.zero;
        }
        if (Input.GetKey(KeyCode.A))
        {
            p_Velocity += new Vector3(-1, 0, 0);
            rigidbody.velocity = Vector3.zero;
            rigidbody.angularVelocity = Vector3.zero;
        }
        if (Input.GetKey(KeyCode.D))
        {
            p_Velocity += new Vector3(1, 0, 0);
            rigidbody.velocity = Vector3.zero;
            rigidbody.angularVelocity = Vector3.zero;
        }

        return p_Velocity;
    }

    void OnCollisionEnter(Collision col)
    {
        if (col.collider.tag == "Ground")
        {
            OnGround = true;
        }
    }
}
