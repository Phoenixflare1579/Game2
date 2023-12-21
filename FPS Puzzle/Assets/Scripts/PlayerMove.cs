using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
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

    void Update()
    {
        if (IsLocalPlayer)
        {
            rb.velocity = new Vector3(lastDirection.x, 0, lastDirection.y).normalized * speed + new Vector3(0, rb.velocity.y, 0);

            if (lastJump)
            {
                rb.velocity += new Vector3(0, 3, 0);
                lastJump = false;
                canJump = false;
            }
            SendUpdate("Move", $"{lastDirection.x},{lastDirection.y},{lastJump},{canJump}");
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

    public override void HandleMessage(string flag, string value)
    {
        if (flag == "Move" && !IsLocalPlayer)
        {
            string[] data = value.Split(',');
            if (data.Length == 4)
            {
                lastDirection = new Vector2(float.Parse(data[0]), float.Parse(data[1]));
                lastJump = bool.Parse(data[2]);
                canJump = bool.Parse(data[3]);
            }
        }
    }

    public override void NetworkedStart()
    {
    }

    public override IEnumerator SlowUpdate()
    {
        yield return new WaitForSeconds(0.1f);

        if (IsLocalPlayer)
        {
            SendUpdate("Move", $"{lastDirection.x},{lastDirection.y},{lastJump},{canJump}");
        }
    }
}

