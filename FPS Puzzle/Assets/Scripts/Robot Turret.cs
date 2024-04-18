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
            if (GameObject.FindGameObjectsWithTag("Player").Length >= 3)
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
                        GameObject bullet = MyCore.NetCreateObject(0, this.Owner, this.transform.position - (this.transform.up * 2f), Quaternion.identity);
                        bullet.GetComponent<Rigidbody>().AddForce(this.transform.position - (this.transform.up) * 140f);
                        shots++;

                    }
                }
                yield return new WaitForSecondsRealtime(0.1f);
            }
            }
            yield return new WaitForSecondsRealtime(0.1f);
        }
    }
}
