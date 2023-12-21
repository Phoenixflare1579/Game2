using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.Windows;
using NETWORK_ENGINE;

public class PlayerMove : NetworkComponent
{
    public PlayerInput MyInput;
    public InputActionAsset MyMap;
    public Rigidbody rb;
    public float speed = 3f;
    public Vector2 lastDirection;
    public bool canJump = true;
    public bool lastJump = false;
    void Start()
    {
        MyInput = GetComponent<PlayerInput>();
        rb = GetComponent<Rigidbody>();

        if (MyInput != null)
        {
            MyMap = MyInput.actions;
        }
        if (rb != null)
        {
            rb.velocity = Vector3.zero;
        }
    }

    // Update is called once per frame
    void Update()
    {
        rb.velocity = new Vector3(lastDirection.x, 0, lastDirection.y).normalized * speed + new Vector3(0, rb.velocity.y, 0);
        if (lastJump)
        {
            rb.velocity += new Vector3(0, 3, 0);
            lastJump = false;
            canJump = false;
        }
        if (rb.velocity.magnitude >= 3f && rb.velocity.magnitude < 6f) 
        {
            Time.timeScale = 1f;
        }
        else if (rb.velocity.magnitude >= 6f)
        {
            Time.timeScale = 1.4f;
        }
        else
        {
            Time.timeScale = 0.1f;
        }

    }
    public void Move(InputAction.CallbackContext c)
    {
        if (c.performed)
        {
            lastDirection = c.ReadValue<Vector2>();
        }
        else if (c.canceled)
        {
            lastDirection = Vector2.zero;
        }
    }
    public void Jump(InputAction.CallbackContext c)
    {
        if (c.performed && canJump == true)
        {
            lastJump = true;
        }
    }
    public void Sprint(InputAction.CallbackContext c)
    {
        if (c.performed == true)
        {
            speed *= 2;
        }
        else if (c.canceled == true)
        {
            speed /= 2;
        }
    }
    public void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.tag == "Floor")
        {
            canJump = true;
        }
    }
    public override void HandleMessage(string flag, string value) { }
    public override void NetworkedStart()
    { }
    public override IEnumerator SlowUpdate()
    { yield return new WaitForSeconds(.1f); }
}
