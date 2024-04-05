using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using NETWORK_ENGINE;

public class NPM : NetworkComponent
{
    public string PName = "Player";
    public int PColor = 0;

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
        
    }

    public void GetName(string name) 
    { 
        if(IsServer)
        {
            PName = name;
        }
    }
    public void GetColor(int color)
    {
        if (IsServer)
        {
           PColor = color;
        }
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
