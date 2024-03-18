using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using NETWORK_ENGINE;

public class PlayerInfo : Info
{
    public bool isReady = false;
    public override void NetworkedStart()
    {
        
        if (IsServer)
        {
            MaxHP = 3;
            HP = MaxHP;
            SendUpdate("HP", HP.ToString());
        }
        if (IsLocalPlayer)
        {
            MaxHP = 3;
        }
    }

    public override void HandleMessage(string flag, string value)
    {
        if (flag == "R")
        {
            if (IsServer)
            {
                isReady = bool.Parse(value);
                SendUpdate("R", isReady.ToString());
            }
            if (IsLocalPlayer)
            {
                isReady = bool.Parse(value);
            }
        }
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

    public override IEnumerator SlowUpdate()
    {
        while (true)
        {
            if (IsServer) 
            { 
                if(HP <= 0)
                {
                    this.gameObject.SetActive(false);
                    HP = MaxHP;
                    SendUpdate("HP", HP.ToString());
                }
            }
            yield return new WaitForSecondsRealtime(0.1f);
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
            SendUpdate("HP", HP.ToString());
        }
    }
    public void ReadyUp()
    {
        if(IsLocalPlayer)
        {
            SendCommand("R", true.ToString());
        }
    }
}
