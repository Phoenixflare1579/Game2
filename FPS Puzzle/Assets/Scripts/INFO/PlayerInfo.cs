using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using NETWORK_ENGINE;
using System;
using TMPro;
using Unity.VisualScripting;
using UnityEngine.InputSystem;
using UnityEngine.UI;

public class PlayerInfo : Info
{
    public bool isReady = false;
    public GameObject Respawn;
    public int DeathCount = 0;
    public bool End = false;
    public TextMeshProUGUI[] endcard;
    public string PName = "Player";
    public int PColor = 0;
    bool Dead = false;
    public GameObject canvas;
    public TextMeshProUGUI playerNameDisplay;
    public bool DeadZone = false;
    bool respawn = true;
    public Collider triggercollider;
    public override void NetworkedStart()
    {
        if (IsServer)
        {
            MaxHP = 2;
            HP = MaxHP;
            SendUpdate("HP", HP.ToString());
        }
        if (!IsServer)
        {
            MaxHP = 2;
            HP = MaxHP;
        }
        if (!IsLocalPlayer)
        {
            this.gameObject.transform.GetChild(1).gameObject.SetActive(false);
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
                if (isReady)
                {
                    this.gameObject.transform.GetChild(1).GetChild(0).GetComponent<Button>().interactable = false;
                }
                else
                {
                    this.gameObject.transform.GetChild(1).GetChild(0).GetComponent<Button>().interactable = true;
                }
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
        if (flag == "COLOR")
        {
            PColor = int.Parse(value);
            switch (PColor)
            {
                case 0:
                    GetComponent<MeshRenderer>().material.color = Color.red;
                    break;
                case 1:
                    GetComponent<MeshRenderer>().material.color = Color.blue;
                    break;
                case 2:
                    GetComponent<MeshRenderer>().material.color = Color.green;
                    break;
                case 3:
                    GetComponent<MeshRenderer>().material.color = Color.white;
                    break;
            }
            if (IsServer)
            {
                SendUpdate("COLOR", PColor.ToString());
            }
        }
        if (flag == "NAME")
        {
            PName = value;
            playerNameDisplay.text = PName;
            if (IsServer)
            {
                SendUpdate("NAME", value);
            }
        }
        if (flag == "End")//Ending card create
        {
            if (IsServer)
            {
                StartCoroutine(Kill());
            }
            if (IsLocalPlayer)
            {
                PName = value;
                endcard[0].text = PName;
                endcard[1].text = "Deaths: " + DeathCount;
                endcard[2].text = "Rtime: " + Time.realtimeSinceStartup;
                endcard[3].text = "Wtime: " + Time.realtimeSinceStartup * Time.timeScale;
                canvas.SetActive(true);
                SendCommand("End", string.Empty);
            }
        }
    }

    public override IEnumerator SlowUpdate()
    {
        while (IsConnected)
        {
            if (IsServer)
            {
                if (this.transform.position.y < -350)
                {
                    HP = 0;
                    SendUpdate("HP", HP.ToString());
                }
                if (IsDirty)
                {
                    SendUpdate("HP", HP.ToString());
                    SendUpdate("R", isReady.ToString());
                    SendUpdate("NAME", PName);
                    SendUpdate("COLOR", PColor.ToString());
                    IsDirty = false;
                }
            }
            if (HP <= 0 && !Dead)
            {
                if (IsServer)
                {
                    Dead = true;
                    HP = MaxHP;
                    SendUpdate("HP", HP.ToString());
                    this.gameObject.transform.position = Respawn.transform.position + new Vector3(0, 0, 3);
                }
                if (IsClient)
                {
                    Dead = true;
                    DeathCount++;
                    this.gameObject.GetComponent<MeshRenderer>().enabled = false;
                }
                if (IsLocalPlayer)
                {
                    Dead = true;
                    GetComponent<PlayerControls>().enabled = false;

                }
                respawn = true;
                foreach(GameObject p in GameObject.FindGameObjectsWithTag("Player"))
                    if (p.GetComponent<PlayerInfo>().DeadZone)
                    {
                        respawn = false;
                    }            
                triggercollider.enabled = false;
            }

            if (respawn && Dead)
            {
                StartCoroutine(Timer());
                Dead = false;
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
    public void Ready(InputAction.CallbackContext c)
    {
        if (c.started)
        {
            if (IsLocalPlayer)
            {
                isReady = !isReady;
                SendCommand("R", isReady.ToString());
            }
        }
    }
    private void OnTriggerStay(Collider other)
    {
        if (IsServer)
        {
            if (other.gameObject.tag == "Respawn")
            {
                Respawn = other.transform.parent.gameObject;
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
        triggercollider.enabled = false;
    }
    public IEnumerator Kill()
    {
        yield return new WaitForSecondsRealtime(5);
        StartCoroutine(MyCore.DisconnectServer());
    }
}
