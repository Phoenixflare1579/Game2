using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using NETWORK_ENGINE;
public class TimeManagement : NetworkComponent
{
    public GameObject[] players;
    public float temp;
    public bool Start = false;
    public bool First = false;

    public override void HandleMessage(string flag, string value)
    {
        if (flag == "Time")
        {
            if(IsClient)
            {
                Time.timeScale = float.Parse(value);
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
                players = GameObject.FindGameObjectsWithTag("Player");
                if (players.Length > 2) 
                {
                    Start = true;
                    for (int i = 0; i < players.Length; i++)
                    {
                        if (!players[i].GetComponent<PlayerInfo>().isReady)
                        {
                            Start=false;
                        }
                    }
                    if (Start)
                    {
                        if(!First)
                        {
                            for (int i = 0; i < players.Length; i++) 
                            {
                                players[i].GetComponent<PlayerInfo>().SendCommand("Start", string.Empty);
                                yield return new WaitForSecondsRealtime(0.1f);
                            }
                            First = true;
                        }
                        temp = players[0].GetComponent<Rigidbody>().velocity.magnitude;
                        for (int i = 0; i < players.Length; i++)
                        {
                            if (players[i].GetComponent<Rigidbody>().velocity.magnitude > temp)
                            {
                                temp = players[i].GetComponent<Rigidbody>().velocity.magnitude;
                            }
                        }
                        if (temp > 0)
                        {
                            if (temp <= 500)
                            {
                                Time.timeScale = temp / 10;
                            }
                            else
                            {
                                Time.timeScale = 500 / 10;
                            }
                        }
                        else if (temp < 0)
                        {
                            if (-temp <= 500)
                            {
                                Time.timeScale = -temp / 10;
                            }
                            else
                            {
                                Time.timeScale = 500 / 10;
                            }
                        }
                        else
                        {
                            temp = 1;
                            Time.timeScale = temp / 10;
                        }
                    }
                    SendUpdate("Time", Time.timeScale.ToString());
                }
                if (IsDirty)
                {
                    SendUpdate("Time", Time.timeScale.ToString());
                }
            }
            yield return new WaitForSecondsRealtime(0.1f);
        }
    }
}
