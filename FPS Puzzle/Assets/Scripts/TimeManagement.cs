using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using NETWORK_ENGINE;
public class TimeManagement : NetworkComponent
{
    public PlayerInfo[] players;
    public float temp;
    public bool Start = false;

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
                players = GameObject.FindObjectsOfType<PlayerInfo>();
                if (players.Length > 2 && !Start)
                {
                    Start = true;
                    foreach (PlayerInfo p in players)
                    {
                        if (!p.isReady)
                        {
                            Start = false;
                        }
                    }
                }
                    if (Start)
                    {
                        temp = players[0].gameObject.GetComponent<Rigidbody>().velocity.magnitude;
                        foreach (PlayerInfo p in players)
                        {
                            if (p.gameObject.GetComponent<Rigidbody>().velocity.magnitude > temp)
                            {
                                temp = p.GetComponent<Rigidbody>().velocity.magnitude;
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
