using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using NETWORK_ENGINE;

public class Info : NetworkComponent //new parent class for players and enemies health for damage calculations.
{
    public int HP;
    public int MaxHP;
    public override void HandleMessage(string flag, string value)
    {

    }

    public override void NetworkedStart()
    {
        
    }

    public override IEnumerator SlowUpdate()
    {
        yield return new WaitForSecondsRealtime(0.1f); 
    }

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
