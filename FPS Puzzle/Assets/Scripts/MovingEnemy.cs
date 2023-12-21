using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using NETWORK_ENGINE;

public class MovingEnemy : NetworkComponent
{
    Rigidbody rb;
    public int direction = 1;

    public override void NetworkedStart()
    {
        rb = GetComponent<Rigidbody>();

        if (IsServer)
        {
            rb.velocity = new Vector3(1, 0, 0) * direction;
            StartCoroutine(ChangeDirectionCoroutine());
        }
    }

    public override IEnumerator SlowUpdate()
    {
        yield return new WaitForSeconds(0.1f);

        if (IsServer)
        {
            SendUpdate("DirectionChange", direction.ToString()); // Send direction update to synchronize across the network
        }
    }

    void Start()
    {
        // No logic in Start() as the network setup is handled in NetworkedStart()
    }

    IEnumerator ChangeDirectionCoroutine()
    {
        while (true)
        {
            yield return new WaitForSecondsRealtime(10);

            if (IsServer)
            {
                direction *= -1;
                rb.velocity = new Vector3(1, 0, 0) * direction;

                SendUpdate("DirectionChange", direction.ToString()); // Send direction update to synchronize across the network
            }
        }
    }

    public override void HandleMessage(string flag, string value)
    {
        if (flag == "DirectionChange")
        {
            if (!IsLocalPlayer)
            {
                direction = int.Parse(value);
                rb.velocity = new Vector3(1, 0, 0) * direction;
            }
        }
    }
}
