using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using NETWORK_ENGINE;

[RequireComponent(typeof(NetworkRigidbody))]
public class Bullet : NetworkComponent
{

    public override void HandleMessage(string flag, string value)
    {

    }
    public override void NetworkedStart()
    {
        if (IsServer)
        {
            StartCoroutine(TTD());
        }
    }
    public override IEnumerator SlowUpdate()
    {
        while (MyCore.IsConnected)
        {
            if (IsServer)
            {
                if (IsDirty)
                {
                    IsDirty = false;
                }
            }
            yield return new WaitForSecondsRealtime(0.1f);
        }
    }
    void Start()
    {

    }
    private void OnCollisionEnter(Collision collision)
    {
        if (IsServer)
        {
            MyCore.NetDestroyObject(MyId.NetId);
        }
    }
    public IEnumerator TTD()
    {
        yield return new WaitForSeconds(35f);
        MyCore.NetDestroyObject(MyId.NetId);
    }
}
