using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using NETWORK_ENGINE;
using System;

public class PlayerInfo : Info
{
    public bool isReady = false;
    public GameObject Respawn;
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
        if (flag == "Respawned")
        {
            if(IsClient)
            {
                this.gameObject.GetComponent<MeshRenderer>().enabled = true;
            }
        }
    }

    public override IEnumerator SlowUpdate()
    {
        while (true)
        {
            if (IsServer) 
            { 
                if (IsDirty)
                {
                    SendUpdate("HP", HP.ToString());
                    SendUpdate("R", isReady.ToString());
                }
            }
            yield return new WaitForSecondsRealtime(0.1f);
        }
    }

    void Update()
    {
        if (HP <= 0)
        {
            if (IsServer)
            {
                HP = MaxHP;
                SendUpdate("HP", HP.ToString());
                this.gameObject.transform.position = Respawn.transform.position + new Vector3 (0,2,0);
                SendUpdate("Respawned", string.Empty);
            }
            if (IsClient)
            {
                this.gameObject.GetComponent<MeshRenderer>().enabled = false;
            }
        }
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (!IsServer) return;

        if (collision.gameObject.tag == "Projectile" || collision.gameObject.tag == "Enemy")
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
    private void OnTriggerEnter(Collider other)
    {
        if (IsServer)
        {
            if (other.gameObject.tag == "Respawn")
            {
                Respawn = other.gameObject;
            }
        }
    }
}
