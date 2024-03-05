using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using NETWORK_ENGINE;
using UnityEngine.EventSystems;

public class MovingEnemy : NetworkComponent
{
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
    public int direction;

    public override void NetworkedStart()
    {
        

        if (IsServer)
        {
            rb.velocity = new Vector3(1, 0, 0) * direction;
            StartCoroutine(ChangeDirectionCoroutine());
        }
    }

    public override IEnumerator SlowUpdate()
    {
        yield return new WaitForSecondsRealtime(0.1f);
    }

    void Start()
    {
        rb = GetComponent<Rigidbody>();
    }

    IEnumerator ChangeDirectionCoroutine()
    {
        while (true)
        {
            yield return new WaitForSecondsRealtime(3);

            if (IsServer)
            {
                direction *= -1;
                rb.velocity = new Vector3(1, 0, 0) * direction;
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
                    IsDirty = false;
                }
            }
        }
    }

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
    private void Update()
    {
        if (IsClient)
        {
            rb.velocity = LastVelocity;
            rb.angularVelocity = LastAngVelocity;
        }
    }
}
