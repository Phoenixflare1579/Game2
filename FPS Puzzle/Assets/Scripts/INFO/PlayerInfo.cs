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
    int volume;
    public Slider vslider;
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
            this.gameObject.transform.GetChild(0).gameObject.SetActive(false);
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
                    transform.GetChild(3).GetComponent<SkinnedMeshRenderer>().materials[1].color = Color.red;
                    break;
                case 1:
                    transform.GetChild(3).GetComponent<SkinnedMeshRenderer>().materials[1].color = Color.blue;
                    break;
                case 2:
                    transform.GetChild(3).GetComponent<SkinnedMeshRenderer>().materials[1].color = Color.green;
                    break;
                case 3:
                    transform.GetChild(3).GetComponent<SkinnedMeshRenderer>().materials[1].color = Color.white;
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
                if (this.transform.position.y < -250f)
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
                    SendUpdate("HP", HP.ToString());
                    GetComponent<Rigidbody>().transform.position = Respawn.transform.position + new Vector3(0, 3, 0);
                }
                if (IsClient)
                {
                    Dead = true;
                    transform.GetChild(3).GetComponent<SkinnedMeshRenderer>().enabled = false;
                    GetComponent<Rigidbody>().transform.position = Respawn.transform.position + new Vector3(0, 3, 0);
                }
                if (IsLocalPlayer)
                {
                    Dead = true;
                    DeathCount++;
                    GetComponent<PlayerInput>().enabled = false;
                }
                respawn = true;
                foreach(GameObject p in GameObject.FindGameObjectsWithTag("Player"))
                    if (p.GetComponent<PlayerInfo>().DeadZone)
                    {
                        respawn = false;
                    }            
            }
            if (respawn)
            {
                StartCoroutine(Timer());
                respawn = false;
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
    public void RespawnBtn()
    {
        if (IsLocalPlayer)
        {
            SendCommand("HP", 0.ToString());
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
    public IEnumerator Timer()
    {
        yield return new WaitForSecondsRealtime(3f);
        if (IsServer) 
        {
            HP = MaxHP;
            SendUpdate("HP", HP.ToString());
        }
        if (IsClient)
        {
            transform.GetChild(3).GetComponent<SkinnedMeshRenderer>().enabled = true;
        }
        if (IsLocalPlayer)
        {
            GetComponent<PlayerInput>().enabled = true;
        }
        Dead = false;
    }
    public IEnumerator Kill()
    {
        yield return new WaitForSecondsRealtime(10f);
        StartCoroutine(MyCore.DisconnectServer());
    }
    public void VolumeChange()
    {
        if (IsLocalPlayer)
        {
            AudioListener.volume = vslider.value/100f;
            
        }
    }
}
