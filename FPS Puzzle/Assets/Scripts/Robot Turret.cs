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
        StartCoroutine(Loop());
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
                    float distance = 1000f;
                    foreach(GameObject p in GameObject.FindGameObjectsWithTag("Player"))
                    {
                        if (Vector3.Distance(this.transform.position, p.transform.position) < distance)
                        {
                            distance = Vector3.Distance(this.transform.position,p.transform.position);
                            target = p;
                        }
                    }
                    GameObject temp = MyCore.NetCreateObject(0, Owner, this.transform.forward*2);
                    temp.GetComponent<Rigidbody>().velocity = Vector3.zero;
                    temp.GetComponent<Rigidbody>().position = Vector3.MoveTowards(temp.transform.position, target.transform.position, 10f * Time.deltaTime);
                }
            }
            yield return new WaitForSecondsRealtime(0.1f);
        }
    }
    public IEnumerator Loop()
    {
        open = true;
        yield return new WaitForSeconds(0.5f);
        open = false;
        yield return new WaitForSeconds(1f);
        StartCoroutine(Loop());
    }
}
