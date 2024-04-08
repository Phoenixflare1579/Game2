using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using NETWORK_ENGINE;
using UnityEngine.InputSystem;
using Unity.VisualScripting;
using System;
using TMPro;
using UnityEngine.UI;

public class PlayerControls : NetworkComponent
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
            if (IsServer)
            {
                if (equipped.name.Contains("gun"))
                {
                    MyCore.NetCreateObject(0, MyId.Owner, LastPosition + this.rb.transform.forward * 2, this.rb.transform.rotation);
                }
                else if (equipped.name.Contains("sword"))
                {

                }
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
                string raw = value.Trim('(').Trim(')');
                string[] sArray = raw.Split(',');

                Quaternion result = new Quaternion(
                    float.Parse(sArray[0]),
                    float.Parse(sArray[1]),
                    float.Parse(sArray[2]),
                    float.Parse(sArray[3]));

                xQuat = result;
            }
            if (IsServer)
            {
                string raw = value.Trim('(').Trim(')');
                string[] sArray = raw.Split(',');

                Quaternion result = new Quaternion(
                    float.Parse(sArray[0]),
                    float.Parse(sArray[1]),
                    float.Parse(sArray[2]),
                    float.Parse(sArray[3]));

                xQuat = result;

                transform.localRotation = xQuat;
            }
        }
        if (flag == "Equip")
        {
            if(IsClient)
            {
                if (value != string.Empty)
                {
                    equipped = GameObject.Find(value.ToString());
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

    public override IEnumerator SlowUpdate()//setting up network rigidbody
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
                rb.transform.localRotation = xQuat;

                SendUpdate("AVel", rb.angularVelocity.ToString());
                LastAngVelocity = rb.angularVelocity;

                if (equipped == null && this.transform.GetChild(0).GetChild(0).gameObject != null)
                {
                    equipped = this.transform.GetChild(0).GetChild(0).gameObject;
                    if(equipped.name.Contains("gun") || equipped.name.Contains("sword"))
                    {
                        isWeapon = true;
                    }
                }

                if (equipped != null)
                {
                    SendUpdate("Equip", equipped.ToString());
                }
                else
                {
                    SendUpdate("Equip", string.Empty);
                }

                if (IsDirty)
                {
                    SendUpdate("Pos", rb.position.ToString());
                    SendUpdate("Rot", rb.transform.localRotation.ToString());
                    SendUpdate("Vel", rb.velocity.ToString());
                    SendUpdate("AVel", rb.angularVelocity.ToString());
                    if (equipped != null)
                    {
                        SendUpdate("Equip", equipped.ToString());
                    }
                    else
                    {
                        SendUpdate("Equip", string.Empty);
                    }
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
        if (IsServer)//setting up velocity for the players based on different booleans
        {
            if (sprint)
            {
                speed = 12f;
            }
            else if (crouch)
            {
                speed = 6f;
            }
            else
            {
                speed = 10f;
            }
            rb.velocity = (transform.right * LastInput.x).normalized * speed + (transform.forward * LastInput.y).normalized * speed + new Vector3(0, rb.velocity.y, 0);
            transform.localRotation = xQuat;
            if (!jump)
            { 
                RaycastHit hit;
                if (Physics.Raycast(transform.position, -transform.up, out hit, 0.5f))
                {
                    Debug.Log(hit.transform.gameObject.name);
                    if (hit.transform.gameObject != this.gameObject && hit.collider.isTrigger == false)
                    {
                        jump = true;
                        SendUpdate("Jump", jump.ToString());
                    }
                }
            }
        }
        if (IsClient)//Sending speed to client
        {
            rb.velocity = LastVelocity;
            rb.transform.localRotation = xQuat;

        }
        if (IsLocalPlayer)//Setting up camera tracking.
        {

            Cursor.lockState = CursorLockMode.Locked;
            Vector3 offset;
            if (!crouch)
            {
                offset = new Vector3(0, 3f, 0);
            }
            else
            {
                offset = new Vector3(0, 1.5f, 0);
            }
            rotation.x += Input.GetAxis(xAxis) * sensitivity;
            rotation.y += Input.GetAxis(yAxis) * sensitivity;
            rotation.y = Mathf.Clamp(rotation.y, -yRotationLimit, yRotationLimit);
            xQuat = Quaternion.AngleAxis(rotation.x, Vector3.up);
            yQuat = Quaternion.AngleAxis(rotation.y, Vector3.left);

            Camera.main.transform.localRotation = xQuat * yQuat;
            transform.localRotation = xQuat;
            Debug.Log(xQuat);
            SendCommand("Rot", xQuat.ToString());

            Camera.main.transform.position = Vector3.Lerp(Camera.main.transform.position, this.transform.position + offset, 100 * Time.deltaTime);
        }
    }

    public Quaternion xQuat;
    public Quaternion yQuat;
    public float Sensitivity
    {
        get { return sensitivity; }
        set { sensitivity = value; }
    }
    [Range(0.1f, 9f)][SerializeField] float sensitivity = 2f;
    [Tooltip("Limits vertical camera rotation. Prevents the flipping that happens when rotation goes above 90.")]
    [Range(0f, 75f)][SerializeField] float yRotationLimit = 72f;

    Vector2 rotation = Vector2.zero;
    const string xAxis = "Mouse X"; //Strings in direct code generate garbage, storing and re-using them creates no garbage
    const string yAxis = "Mouse Y";
}
