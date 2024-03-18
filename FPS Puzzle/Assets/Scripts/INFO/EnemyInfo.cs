using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using NETWORK_ENGINE;

public class EnemyInfo : Info
{
    public override void NetworkedStart()
    {
        if (IsServer)
        {
            MaxHP = 3;
            HP = MaxHP;
        }
    }
    public override void HandleMessage(string flag, string value)
    {
        if (flag == "HP")
        {
            if (!IsLocalPlayer)
            {
                HP = int.Parse(value);
            }
        }
    }

    public override IEnumerator SlowUpdate()
    {
        while (true)
        {
            if (IsServer)
            {
                if (HP <= 0)
                {
                    MyCore.NetDestroyObject(this.MyId.NetId);
                }
            }
            yield return new WaitForSeconds(0.1f);
        }
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
