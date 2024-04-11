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
    public GameObject equipslot;
    RaycastHit[] hit;
    public Animator animator;
    Ray ray;
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
        }
        if (flag == "Equip")
        {
            PickUp((MyCore.NetObjs[int.Parse(value)]).gameObject);
            if (IsServer)
            {
                SendUpdate("Equip", value);
            }
        }
        if (flag == "Drop")
        {
            Drop(equipped);
            if(IsServer)
            {
                SendUpdate("Drop", string.Empty);
            }
        }
        if (flag == "Crouch")
        {
            crouch = bool.Parse(value);
            if (IsServer)
            {
                SendUpdate("Crouch", value);
            }
            if(crouch)
            {
                GetComponent<BoxCollider>().center = new Vector3(0, -1f, 0);
                GetComponent<BoxCollider>().size = new Vector3(2.6f, 4f, 1.5f);
            }
            else
            {
                GetComponent<BoxCollider>().center = new Vector3(0, 0.2f, 0);
                GetComponent<BoxCollider>().size = new Vector3(2.6f, 6.5f, 1f);
            }
        }
        if (flag == "Jump")
        {
            if (IsServer)
            {
                jump = bool.Parse(value);
                rb.velocity += new Vector3(0, 8f, 0);
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
                    string[] args = value.Split(',');
                    MyCore.NetCreateObject(0, MyId.Owner, transform.forward + LastPosition * 2, Quaternion.identity);
                    SendUpdate("Fire", "G");
                }
                else if (equipped.name.Contains("sword"))
                {
                    if (value != string.Empty)
                    {
                        string[] args = value.Split(',');
                        if (args[0] == "Pr")
                        { 
                         //   MyCore.NetObjs[int.Parse(args[1])].gameObject.GetComponent<Rigidbody>().velocity *= -1; needs to attach to sword for a second and then fly in the direction of ray.
                         //   args[2] is the direction of the raycast
                        }
                        else if (args[0] == "E")
                        {
                            GameObject temp = MyCore.NetObjs[int.Parse(args[1])].gameObject;
                            temp.GetComponent<EnemyInfo>().HP -= 1;
                            temp.GetComponent<EnemyInfo>().SendUpdate("HP", temp.GetComponent<EnemyInfo>().HP.ToString());
                        }
                        else if (args[0] == "Pl")
                        {
                            GameObject temp = MyCore.NetObjs[int.Parse(args[1])].gameObject;
                            temp.GetComponent<PlayerInfo>().HP -= 1;
                            temp.GetComponent<PlayerInfo>().SendUpdate("HP", temp.GetComponent<PlayerInfo>().HP.ToString());
                        }
                    }
                    SendUpdate("Fire", "S");
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
                if(equipped.name.Contains("sword"))
                {
                    foreach (RaycastHit h in hit)
                    {
                        if(h.transform.gameObject.tag == "Projectile")
                        {
                            SendCommand("Fire", "Pr" + "," + h.transform.gameObject.GetComponent<NetworkID>().NetId.ToString() + "," + ray.direction);
                            break;
                        }
                        else if (h.transform.gameObject.tag == "Android" || h.transform.gameObject.tag == "Enemy") 
                        {
                            SendCommand("Fire", "E" + "," + h.transform.gameObject.GetComponent<NetworkID>().NetId.ToString());
                            break;
                        }
                        else if (h.transform.gameObject.tag == "Player")
                        {
                            SendCommand("Fire", "Pl" + "," + h.transform.gameObject.GetComponent<NetworkID>().NetId.ToString());
                            break;
                        }
                    }
                    SendCommand("Fire", string.Empty);
                }
                else
                {
                    SendCommand("Fire", ray.GetPoint(0.5f).ToString() + "," + ray.direction.ToString());
                }
            }
            if (IsClient)
            {
                if (equipped.name.Contains("sword"))
                {
                    animator.SetTrigger("Slash");
                }
                else
                {
                    animator.SetTrigger("Shoot");
                }
            }
        }
    }
    public void Grab(InputAction.CallbackContext c)
    {
        foreach (RaycastHit h in hit)
        {
            if (equipped == null && c.started && h.transform.gameObject.tag == "Equippable" && IsLocalPlayer)
            {
                SendCommand("Equip", h.transform.gameObject.GetComponent<NetworkID>().NetId.ToString());
                break;
            }
        }
    }
    public void Throw(InputAction.CallbackContext c)
    {
        if (equipped != null && c.started)
        {
            SendCommand("Drop", string.Empty);
        }
    }

    private void PickUp(GameObject e)//Picking up objects.
    {
        e.GetComponent<Rigidbody>().useGravity = false;

        e.transform.SetParent(equipslot.transform);

        equipped = e;

        if (equipped.name.Contains("gun") || equipped.name.Contains("sword"))
        {
            if (IsClient)
            {
                animator.SetTrigger("Grab");
            }
            isWeapon = true;
        }

        if (equipped.name.Contains("cube"))
        {
            if (IsClient)
            {
                animator.SetTrigger("Cube");
            }
            e.transform.localRotation = Quaternion.Euler(new Vector3(90f,0,0));
        }

        e.transform.localPosition = Vector3.zero;

        
        e.GetComponent<Collider>().enabled = false;

    }

    public void Stop()
    {
        animator.speed = 0;
    }

    private void Drop(GameObject e)
    {
        e.transform.SetParent(null);
        equipped = null;
        
        e.GetComponent<Rigidbody>().useGravity = true;

        e.GetComponent<Rigidbody>().velocity = rb.velocity;

        e.GetComponent<Rigidbody>().AddForce(rb.transform.forward * 3f, ForceMode.Impulse);
        e.GetComponent<Rigidbody>().AddForce(rb.transform.up * 0.5f, ForceMode.Impulse);

        
        e.GetComponent<Collider>().enabled = true;
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

                if (IsDirty)
                {
                    SendUpdate("Pos", rb.position.ToString());
                    SendUpdate("Rot", rb.transform.localRotation.ToString());
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
        animator = GetComponent<Animator>();
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
                RaycastHit[] hit2 = Physics.RaycastAll(transform.position, -transform.up, 1.5f);
                Debug.DrawRay(transform.position, -transform.up, Color.blue);
                foreach(RaycastHit h in hit2)
                {
                    if (h.transform.gameObject != this.gameObject)
                    {
                        jump = true;
                        SendUpdate("Jump", jump.ToString());
                        break;
                    }
                }
            }
        }
        if (IsClient)//Sending speed to client
        {
            rb.velocity = LastVelocity;
            rb.transform.localRotation = xQuat;
            animator.SetBool("Crouch", crouch);
            if (rb.velocity.x + rb.velocity.z != animator.GetFloat("SpeedH"))
            {
                animator.SetFloat("SpeedH", rb.velocity.x + rb.velocity.z);
            }
            if (rb.velocity.y != animator.GetFloat("SpeedV"))
            {
                animator.SetFloat("SpeedV", rb.velocity.y);
            }
        }
        if (IsLocalPlayer)//Setting up camera tracking.
        {
            ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            hit = Physics.RaycastAll(ray, 3f);
            Cursor.lockState = CursorLockMode.Locked;
            Vector3 offset;
            if (!crouch)
            {
                offset = new Vector3(0, 1.45f, 0);
            }
            else
            {
                offset = new Vector3(0, 1f, 0);
            }
            rotation.x += Input.GetAxis(xAxis) * sensitivity;
            rotation.y += Input.GetAxis(yAxis) * sensitivity;
            rotation.y = Mathf.Clamp(rotation.y, -yRotationLimit, yRotationLimit);
            xQuat = Quaternion.AngleAxis(rotation.x, Vector3.up);
            yQuat = Quaternion.AngleAxis(rotation.y, Vector3.left);

            Camera.main.transform.localRotation = xQuat * yQuat;
            transform.localRotation = xQuat;
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
