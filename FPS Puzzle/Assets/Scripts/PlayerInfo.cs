using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using NETWORK_ENGINE;

public class PlayerInfo : NetworkComponent
{
    int MaxHP;
    int HP;

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
        // Handle messages sent to this object over the network (if needed)
    }

    public override IEnumerator SlowUpdate()
    {
        while (true)
        {
            yield return new WaitForSeconds(0.1f);

            // Additional logic that needs to run in slower intervals on the server
        }
    }

    void Update()
    {
        if (!IsLocalPlayer) return;

        if (HP <= 0)
        {
            SceneManager.LoadScene("Game Over");
        }
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (!IsServer) return;

        if (collision.gameObject.tag == "Projectile")
        {
            HP -= 1;
            
        }
    }
}
