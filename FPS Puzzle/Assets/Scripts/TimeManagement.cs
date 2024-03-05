using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using NETWORK_ENGINE;
public class TimeManagement : NetworkComponent
{
    GameObject[] players;
    public override void HandleMessage(string flag, string value)
    {
        if (flag == "Time")
        {
            if(IsClient)
            {
                Time.timeScale = float.Parse(value) / 10;
            }
        }
    }

    public override void NetworkedStart()
    {
        
    }

    public override IEnumerator SlowUpdate()
    {
        while(MyCore.IsConnected)
        {
            if (IsServer)
            {
                players = GameObject.FindGameObjectsWithTag("Player");
                if (players.Length > 0) 
                {
                    float temp = players[0].GetComponent<Rigidbody>().velocity.magnitude;
                    for(int i = 0; i < players.Length; i++) 
                    {
                        if (players[i].GetComponent<Rigidbody>().velocity.magnitude > temp) 
                        {
                            temp = players[i].GetComponent<Rigidbody>().velocity.magnitude;
                        }
                    }
                    if (temp >= 0)
                    {
                        Time.timeScale = temp / 10;
                    }
                    else
                    {
                        temp = 1;
                        Time.timeScale = temp / 10;
                    }
                    SendUpdate("Time", temp.ToString());
                }
            }
            yield return new WaitForSeconds(MyId.UpdateFrequency);
        }
    }

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
