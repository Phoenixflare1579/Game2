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
        if (flag == "HP")//testing for if HandleMessage works for child classes.
        {
            if (IsServer)
            {
                HP = int.Parse(value);
                SendUpdate("HP", HP.ToString());
            }
            if (IsLocalPlayer)
            {
                HP = int.Parse(value);
            }
        }
    }

    public override void NetworkedStart()
    {
        
    }

    public override IEnumerator SlowUpdate()
    {
        yield return new WaitForSeconds(0.1f); 
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
