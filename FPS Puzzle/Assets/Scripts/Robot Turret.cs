using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using NETWORK_ENGINE;
using Unity.VisualScripting;

public class RobotTurret : NetworkComponent
{
    public bool open = false;
    GameObject target;

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
            }
            else
            {
                GetComponent<Collider>().enabled = true;
                if (IsServer && target == null)
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
                            temp.GetComponent<Rigidbody>().velocity = Vector3.zero;
                            temp.GetComponent<Rigidbody>().position = Vector3.MoveTowards(temp.transform.position, target.transform.position, 10f * Time.deltaTime);
                            target = null;
                        }
                    }
                }
            }
            yield return new WaitForSecondsRealtime(0.1f);
        }
    }
}
