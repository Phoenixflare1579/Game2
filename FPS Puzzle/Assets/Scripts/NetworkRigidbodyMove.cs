using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using NETWORK_ENGINE;
using UnityEngine.InputSystem;

public class NetworkRigidbodyMove : NetworkComponent
{
    Vector2 LastInput;
    public float threshhold = 0.1f;
    public float ethreshhold = 3f;
    public Vector3 LastPosition;
    public Vector3 LastRotation;
    public Vector3 LastVelocity;
    public Vector3 LastAngVelocity;
    Rigidbody rb;
    public bool useAdapt = false;
    public Vector3 adaptVelocity;
    public float speed = 10f;
    public bool sprint = false;
    public bool jump = true;
    public GameObject equipped;
    public bool isWeapon = false;
    bool crouch = false;
    public override void HandleMessage(string flag, string value)
    {
        if (IsServer)
        {
            if (flag == "Move")
            {
                string[] tmp = value.Split(',');
                LastInput = new Vector2(int.Parse(tmp[0]), int.Parse(tmp[1]));
            }
            if (flag == "Sprint")
            {
                sprint = bool.Parse(value);
            }
            if (flag == "Crouch")
            {
                crouch = bool.Parse(value);
            }
        }
        if (flag == "Jump")
        {
            if (IsServer)
            {
                jump = bool.Parse(value);
                rb.velocity += new Vector3(0, 5, 0);
            }
            if (IsLocalPlayer)
            {
                jump = bool.Parse(value);
            }
        }
        if (flag == "Pos")
        {
            if (IsClient)
            {
                LastPosition = NetworkCore.Vector3FromString(value);
                if ((LastPosition - rb.position).magnitude > ethreshhold)
                {
                    rb.position = LastPosition;
                }
                else if ((LastPosition - rb.position).magnitude > threshhold)
                {
                    adaptVelocity = (LastPosition - rb.position) / .1f;
                }
                else
                {
                    adaptVelocity = Vector3.zero;
                }
            }
        }
        if (flag == "Fire")
        {
            if(IsServer)
            {
                if(equipped.name.Contains("gun"))
                {
                    MyCore.NetCreateObject(0, MyId.Owner, this.transform.forward, this.transform.rotation);
                }
            }
        }
        if (flag == "Weapon")
        {
            if(IsLocalPlayer)
            {
                isWeapon = bool.Parse(value);
            }
        }
        if (flag == "Vel")
        {
            if (IsClient)
            {
                LastVelocity = NetworkCore.Vector3FromString(value);
                if (useAdapt)
                {
                    LastAngVelocity = adaptVelocity;
                }
            }
        }
        if (flag == "AVel")
        {
            if (IsClient)
            {
                LastAngVelocity = NetworkCore.Vector3FromString(value);
            }
        }
        if (flag == "Rot")
        {
            if (IsClient)
            {
                LastRotation = NetworkCore.Vector3FromString(value);
                if ((LastRotation - rb.rotation.eulerAngles).magnitude > ethreshhold && useAdapt)
                {
                    rb.rotation = Quaternion.Euler(LastRotation);
                }
            }
        }
    }

    public void Move(InputAction.CallbackContext c)
    {
        if (c.started || c.performed)
        {
            if (IsLocalPlayer)
            {
                Vector2 temp = c.ReadValue<Vector2>();
                SendCommand("Move", temp.x + "," + temp.y);
            }
        }
        else if (c.canceled)
        {
            SendCommand("Move", "0,0");
        }
    }
    public void Sprint(InputAction.CallbackContext c)
    {
        if (c.started || c.performed)
        {
            if (IsLocalPlayer)
            {
                SendCommand("Sprint", true.ToString());
            }
        }
        else if (c.canceled)
        {
            SendCommand("Sprint", false.ToString());
        }
    }

    public void Jump(InputAction.CallbackContext c)
    {
        if (c.started && jump == true)
        {
            if (IsLocalPlayer)
            {
                jump = false;
                SendCommand("Jump", jump.ToString());
            }
        }
    }

    public void Crouch(InputAction.CallbackContext c)
    {
        if (c.started || c.performed)
        {
            if (IsLocalPlayer)
            {
                crouch = true;
                SendCommand("Crouch", crouch.ToString());
            }
        }
        else if (c.canceled)
        {
            crouch = false;
            SendCommand("Crouch", crouch.ToString());
        }
    }

    public void Fire(InputAction.CallbackContext c)
    {
        if(c.started && isWeapon)
        {
            if (IsLocalPlayer)
            {
                SendCommand("Fire", string.Empty);
            }
        }
    }

    public override void NetworkedStart()
    {

    }

    public override IEnumerator SlowUpdate()
    {
        while (MyCore.IsConnected)
        {
            if (IsServer)
            {
                SendUpdate("Pos", rb.position.ToString());
                LastPosition = rb.position;

                SendUpdate("Vel", rb.velocity.ToString());
                LastVelocity = rb.velocity;

                SendUpdate("Rot", rb.rotation.ToString());
                LastRotation = rb.rotation.eulerAngles;

                SendUpdate("AVel", rb.angularVelocity.ToString());
                LastAngVelocity = rb.angularVelocity;

                equipped = this.transform.GetChild(0).GetChild(0).gameObject;
                if (equipped.name.Contains("gun") || equipped.name.Contains("sword"))
                {
                    isWeapon = true;
                    SendUpdate("Weapon", isWeapon.ToString());
                }
                else
                {
                    isWeapon = false;
                    SendUpdate("Weapon", isWeapon.ToString());
                }
                if (IsDirty)
                {
                    SendUpdate("Pos", rb.position.ToString());
                    SendUpdate("Rot", rb.rotation.ToString());
                    SendUpdate("Vel", rb.velocity.ToString());
                    SendUpdate("AVel", rb.angularVelocity.ToString());

                    IsDirty = false;
                }
            }
            yield return new WaitForSecondsRealtime(0.1f);
        }
    }

    // Start is called before the first frame update
    void Start()
    {
        rb = this.GetComponent<Rigidbody>();
    }

    // Update is called once per frame
    void Update()
    {
        if (IsServer)
        {
            if (sprint)
            {
                speed = 12f;
            }
            else
            {
                speed = 10f;
            }
            Vector3 tv = new Vector3(LastInput.x, 0, LastInput.y).normalized * speed + new Vector3(0, rb.velocity.y, 0);
            rb.velocity = tv;
        }
        if (IsClient)
        {
            rb.velocity = LastVelocity;
            rb.angularVelocity = LastAngVelocity;
        }
        if (IsLocalPlayer)
        {
            Vector3 offset;
            if (!crouch)
            {
                offset = new Vector3(0, 1, 0);
            }
            else
            {
                offset = new Vector3(0, 0, 0);
            }
            Camera.main.transform.position = Vector3.Lerp(Camera.main.transform.position, this.transform.position + offset, 100 * Time.deltaTime);
        }
    }
    private void OnCollisionEnter(Collision collision)
    {
        if (IsServer)
        {
            if (collision.gameObject.tag == "Floor")
            {
                jump = true;
                SendUpdate("Jump", jump.ToString());
            }
        }
    }
}
