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
            yield return new WaitForSecondsRealtime(0.1f);
        }
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (!IsServer) return;

        if (collision.gameObject.tag == "Projectile")
        {
            HP -= 1;
            SendUpdate("HP", HP.ToString());
        }
    }
}
