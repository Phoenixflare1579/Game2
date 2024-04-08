using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using NETWORK_ENGINE;

[RequireComponent(typeof(NetworkRigidbody))]
public class Bullet : NetworkComponent
{
    Rigidbody rb;
    public override void HandleMessage(string flag, string value)
    {
        
    }

    public override void NetworkedStart()
    {
        if(IsServer)
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
    public IEnumerator TTD()
    {
        yield return new WaitForSeconds(10);
        MyCore.NetDestroyObject(MyId.NetId);
    }
}
