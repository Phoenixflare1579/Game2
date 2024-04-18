using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using NETWORK_ENGINE;
using System;
public class TimeManagement : NetworkComponent
{
    public GameObject[] players;
    public float temp;
    public bool Start = false;
    public GameObject Level1;

    public override void HandleMessage(string flag, string value)
    {
        if (flag == "Time")
        {
            if(IsClient)
            {
                Time.timeScale = float.Parse(value);
                foreach(AudioSource s in GameObject.FindObjectsOfType(typeof(AudioSource)))
                {
                    s.pitch = Time.timeScale;
                }
            }
        }
    }

    public override void NetworkedStart()
    {
        MyCore.MaxConnections = 4;
    }

    public override IEnumerator SlowUpdate()
    {
        while(MyCore.IsConnected)
        {
            if (IsServer)
            {
                if (!Start)
                {
                    players = GameObject.FindGameObjectsWithTag("Player");//pulls the array of gameobjects to check for velocity and isready.
                    if (players.Length > 2)
                    {
                        Start = true;
                        foreach (GameObject p in players)
                        {
                            if (!p.GetComponent<PlayerInfo>().isReady)
                            {
                                Start = false;
                            }
                        }
                    }
                    if(Start)
                    {
                        MyCore.NotifyGameStart();
                        foreach (GameObject p in players)
                        {
                            p.GetComponent<Rigidbody>().position = Level1.transform.position + new Vector3(0,5f,0);
                        }
                    }
                }
                else if (Start)//Setting time for all the players once everyone has readied up
                {
                    temp = players[0].gameObject.GetComponent<Rigidbody>().velocity.magnitude;
                    foreach (GameObject p in players)
                    {
                        if (p.gameObject != null && p.gameObject.GetComponent<Rigidbody>().velocity.magnitude != float.NaN)
                        {
                            if (Math.Abs(p.gameObject.GetComponent<Rigidbody>().velocity.magnitude) > Math.Abs(temp))
                            {
                                temp = Math.Abs(p.GetComponent<Rigidbody>().velocity.magnitude);
                            }
                        }
                    }
                    if (temp > 1f)
                    {
                        if (temp <= 500.0f)
                        {
                            Time.timeScale = temp / 10.0f;
                        }
                        else
                        {
                            Time.timeScale = 500.0f / 10.0f;
                        }
                    }
                    else
                    {
                        temp = 1;
                        Time.timeScale = temp / 10.0f;
                    }
                }
                SendUpdate("Time", Time.timeScale.ToString());//Sending time to the client
                if (IsDirty)
                {
                    SendUpdate("Time", Time.timeScale.ToString());
                    IsDirty = false;
                }
            }
            yield return new WaitForSecondsRealtime(0.1f);
        }
    }
}
