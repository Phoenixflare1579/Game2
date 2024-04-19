using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using NETWORK_ENGINE;

public class Target : NetworkComponent
{
    public bool hit = false;
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
        if (collision.gameObject.tag == "Projectile")
        {
            hit = true;
            if (IsServer)
            {
                SendUpdate("A", hit.ToString());
            }
        }
    }

    public override IEnumerator SlowUpdate()
    {
        while (IsConnected)
        {
            if (IsServer)
            {
                if (IsDirty)
                {
                    SendUpdate("A", hit.ToString());
                    IsDirty = false;
                }
            }
            yield return new WaitForSecondsRealtime(0.1f);
        }
    }

    public override void HandleMessage(string flag, string value)
    {
        if(flag == "A") 
        {
            if (IsClient)
            {
                hit = bool.Parse(value);
                if (hit)
                {
                    GetComponent<MeshRenderer>().enabled = false;
                }
            }
        }
    }

    public override void NetworkedStart()
    {

    }
}
