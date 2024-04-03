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
    public float RTime;
    public float WTime;
    bool End = false;
    GameObject endcard;
    public string PName = "Test";
    public override void NetworkedStart()
    {
        if (IsServer)
        {
            MaxHP = 2;
            HP = MaxHP;
            RTime = Time.realtimeSinceStartup;
            WTime = Time.realtimeSinceStartup;
            SendUpdate("HP", HP.ToString());
        }
        if (!IsServer)
        {
            MaxHP = 2;
            HP = MaxHP;
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
            if (!IsServer)
            {
                isReady = bool.Parse(value);
            }
        }
        if (flag == "HP")
        {
            if (IsServer)
            {
                HP = int.Parse(value);
                SendUpdate("HP", HP.ToString());
            }
            if (!IsServer)
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
        if (flag == "End")
        {
            if (!IsServer)
            {
                string[] args = value.Split(',');
                endcard.transform.SetParent(this.gameObject.transform.GetChild(1).GetChild(2).transform);
                RTime = float.Parse(args[0]);
                WTime = float.Parse(args[1]);
                PName = args[2];
                endcard.transform.GetChild(0).GetComponent<TextMeshProUGUI>().text = PName;
                endcard.transform.GetChild(1).GetComponent<TextMeshProUGUI>().text = "Deaths: " + DeathCount;
                endcard.transform.GetChild(2).GetComponent<TextMeshProUGUI>().text = "Rtime: " + (Time.realtimeSinceStartup - RTime);
                endcard.transform.GetChild(3).GetComponent<TextMeshProUGUI>().text = "Wtime: " + (Time.deltaTime - WTime);
            }
        }
    }

    public override IEnumerator SlowUpdate()
    {
        while (IsConnected)
        {
            if (IsServer)
            {
                if (Time.timeScale < 1 && !End)
                {
                    endcard = MyCore.NetCreateObject(1, this.Owner);
                    endcard.transform.SetParent(this.gameObject.transform.GetChild(1).GetChild(2).transform);
                    SendUpdate("End", RTime + ',' + WTime + ',' + PName);
                    End = true;
                }
                if (IsDirty)
                {
                    SendUpdate("HP", HP.ToString());
                    SendUpdate("R", isReady.ToString());
                    IsDirty = false;
                }
            }
            if (HP <= 0)
            {
                if (IsServer)
                {
                    this.gameObject.transform.position = Respawn.transform.position + new Vector3(0, 2, 0);
                    DeathCount++;
                    HP = MaxHP;
                    SendUpdate("HP", HP.ToString());
                }
                if (IsClient)
                {
                    this.gameObject.GetComponent<MeshRenderer>().enabled = false;
                    StartCoroutine(Timer());
                }
                if (IsLocalPlayer)
                {
                    GetComponent<PlayerControls>().enabled = false;
                    StartCoroutine(Timer());
                }
            }
            yield return new WaitForSecondsRealtime(0.1f);
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
        if(IsServer)
        {
            if (other.gameObject.tag == "Respawn")
            {
                Respawn = other.gameObject;
            }
        }
    }
    public IEnumerator Timer()
    {
        yield return new WaitForSecondsRealtime(3);
        if (IsClient) 
        {
            this.gameObject.GetComponent<MeshRenderer>().enabled = true;
        }
        if (IsLocalPlayer)
        {
            GetComponent<PlayerControls>().enabled = true;
        }
    }
}
