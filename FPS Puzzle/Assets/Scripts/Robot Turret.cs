using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using NETWORK_ENGINE;
using Unity.VisualScripting;

public class RobotTurret : NetworkComponent
{
    public bool open = false;
    GameObject target;
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
                target = null;
                shots = 0;
            }
            else
            {
                GetComponent<Collider>().enabled = true;
                if (IsServer && target == null && shots <=3)
                {
                    if (GameObject.FindGameObjectWithTag("Player") != null)
                    {
                        float distance = 100f;
                        foreach (GameObject p in GameObject.FindGameObjectsWithTag("Player"))
                        {
                            if (Vector3.Distance(this.transform.position, p.transform.position) < distance)
                            {
                                distance = Vector3.Distance(this.transform.position, p.transform.position);
                                target = p;
                            }
                        }
                        if (target != null)
                        {
                            GameObject temp = MyCore.NetCreateObject(0, Owner, this.transform.position); 
                            target = null;
                            shots++;
                        }
                    }
                }
            }
            yield return new WaitForSecondsRealtime(0.1f);
        }
    }
}
