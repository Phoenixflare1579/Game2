using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using NETWORK_ENGINE;

public class MovingEnemy : NetworkComponent
{
    Rigidbody rb;
    public int direction=1;
    public override void HandleMessage(string flag, string value) { }
    public override void NetworkedStart() 
    { }
    public override IEnumerator SlowUpdate()
    { yield return new WaitForSeconds(.1f); }
    void Start()
    {
        rb = GetComponent<Rigidbody>();
        rb.velocity = new Vector3(1,0,0)*direction;
        StartCoroutine(changedirection());
    }
    IEnumerator changedirection()
    {
        yield return new WaitForSecondsRealtime(10);
        direction *= -1;
        Start();
    }
}
