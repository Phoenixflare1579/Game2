using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using NETWORK_ENGINE;

public class Bullet : NetworkComponent
{
    Vector2 LastInput;
    public float threshhold = 0.1f;
    public float ethreshhold = 3f;
    public Vector3 LastPosition;
    public Vector3 LastVelocity;
    public bool useAdapt = false;
    public Vector3 adaptVelocity;
    Rigidbody rb;
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

                if (IsDirty)
                {
                    SendUpdate("Pos", rb.position.ToString());
                    SendUpdate("Vel", rb.velocity.ToString());
                    IsDirty = false;
                }
            }
            yield return new WaitForSecondsRealtime(0.1f);
        }
    }

    // Start is called before the first frame update
    void Start()
    {
        rb = GetComponent<Rigidbody>();
        if (IsServer)
        {
            rb.velocity = transform.forward * 20f;
        }
    }

    // Update is called once per frame
    void Update()
    {
        
    }
    private void OnCollisionEnter(Collision collision)
    {
        if (IsServer)
        {
            MyCore.NetDestroyObject(MyId.NetId);
        }
    }
}
