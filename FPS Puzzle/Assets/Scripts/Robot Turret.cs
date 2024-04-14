using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using NETWORK_ENGINE;
using Unity.VisualScripting;

public class RobotTurret : NetworkComponent
{
    public bool open = false;
    int shots;

    public override void HandleMessage(string flag, string value)
    {

    }

    public override void NetworkedStart()
    {

    }

    public override IEnumerator SlowUpdate()
    {
        while (IsConnected)
        {
            if (!open)
            {
                GetComponent<Collider>().enabled = false;
                if (IsServer)
                {
                    shots = 0;
                }
            }
            else
            {
                GetComponent<Collider>().enabled = true;
                if (IsServer)
                {
                    if (shots < 3)
                    {
                        GameObject temp = MyCore.NetCreateObject(0, this.Owner, this.transform.position + this.transform.forward + new Vector3(0,-2f,0), Quaternion.identity);
                        shots++;
                    }
                }
            }
            yield return new WaitForSecondsRealtime(0.1f);
        }
    }
}
