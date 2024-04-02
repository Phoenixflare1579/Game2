using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using NETWORK_ENGINE;
using System;
using TMPro;

public class PlayerInfo : Info
{
    public bool isReady = false;
    public GameObject Respawn;
    public int DeathCount = 0;
    public int RTime;
    public int WTime;
    bool End = false;
    GameObject endcard;
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
                if(Time.timeScale < 1 && !End)
                {
                    endcard=MyCore.NetCreateObject(1, -1);
                    endcard.transform.GetChild(0).GetComponent<TextMeshProUGUI>().text = "Player " + this.NetId;
                    endcard.transform.GetChild(1).GetComponent<TextMeshProUGUI>().text = "Deaths:  " + DeathCount;
                    endcard.transform.GetChild(2).GetComponent<TextMeshProUGUI>().text = "Rtime: " + Time.realtimeSinceStartup;
                    endcard.transform.GetChild(3).GetComponent<TextMeshProUGUI>().text = "Wtime: " + Time.deltaTime;
                }
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
