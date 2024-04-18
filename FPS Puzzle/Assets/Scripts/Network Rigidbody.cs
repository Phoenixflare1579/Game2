using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using NETWORK_ENGINE;
using UnityEngine.EventSystems;

public class NetworkRigidbody : NetworkComponent
{
    public float threshhold = 0.1f;
    public float ethreshhold = 5f;
    public Vector3 LastPosition;
    public Vector3 LastRotation;
    public Vector3 LastVelocity;
    public Vector3 LastAngVelocity;
    Rigidbody rb;
    public Vector3 adaptVelocity;
    public bool equipped;

    public override void HandleMessage(string flag, string value)
    {
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
        if (flag == "Vel")
        {
            if (IsClient)
            {
                LastVelocity = NetworkCore.Vector3FromString(value) + adaptVelocity;
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
                if ((LastRotation - rb.rotation.eulerAngles).magnitude > ethreshhold)
                {
                    rb.rotation = Quaternion.Euler(LastRotation);
                }
            }
        }
        if (flag == "E")
        {
            if(IsClient)
            {
                equipped = bool.Parse(value);
                if (equipped)
                {
                    rb.transform.localPosition = Vector3.zero;
                    if (name.Contains("sword"))
                    {
                        rb.transform.localRotation = Quaternion.LookRotation(new Vector3(90f, 0, 0));
                    }
                    else if(name.Contains("gun"))
                    {
                        rb.transform.localRotation = Quaternion.identity;
                    }
                    else if(name.Contains("cube"))
                    {
                        rb.transform.localRotation = Quaternion.LookRotation(new Vector3(90f, 0, 0));
                    }
                    
                }
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
                if (this.transform.parent != null && tag == "Equippable")
                {
                    rb.transform.localPosition = Vector3.zero;
                    if (name.Contains("sword"))
                    {
                        rb.transform.localRotation = Quaternion.LookRotation(new Vector3(90f, 0, 0));
                    }
                    else if (name.Contains("gun"))
                    {
                        rb.transform.localRotation = Quaternion.identity;
                    }
                    else if (name.Contains("cube"))
                    {
                        rb.transform.localRotation = Quaternion.LookRotation(new Vector3(90f, 0, 0));
                    }
                    equipped = true;
                    SendUpdate("E", equipped.ToString());
                }
                else
                {
                    equipped = false;
                    SendUpdate("E", equipped.ToString());
                }
                SendUpdate("Pos", rb.position.ToString());
                LastPosition = rb.position;

                SendUpdate("Vel", rb.velocity.ToString());
                LastVelocity = rb.velocity;

                SendUpdate("Rot", rb.rotation.ToString());
                LastRotation = rb.rotation.eulerAngles;

                SendUpdate("AVel", rb.angularVelocity.ToString());
                LastAngVelocity = rb.angularVelocity;
                if (IsDirty)
                {
                    SendUpdate("Pos", rb.position.ToString());
                    SendUpdate("Rot", rb.rotation.ToString());
                    SendUpdate("Vel", rb.velocity.ToString());
                    SendUpdate("AVel", rb.angularVelocity.ToString());
                    SendUpdate("E", equipped.ToString());
                    IsDirty = false;
                }
            }
            yield return new WaitForSecondsRealtime(0.1f);
        }
    }

    void Start()
    {
        rb = GetComponent<Rigidbody>();
    }
    private void Update()
    {
        if (IsClient)
        {
            rb.velocity = LastVelocity;
            rb.angularVelocity = LastAngVelocity;
        }
    }
}
