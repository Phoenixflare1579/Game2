using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using NETWORK_ENGINE;

public class Target : NetworkComponent
{

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.tag == "Projectile" && IsServer)
        {
            MyCore.NetDestroyObject(NetId);
        }
    }

    public override IEnumerator SlowUpdate()
    {
        yield return new WaitForSecondsRealtime(0.1f);
    }

    public override void HandleMessage(string flag, string value)
    {

    }

    public override void NetworkedStart()
    {

    }
}
