using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using NETWORK_ENGINE;

public class NPM : NetworkComponent
{
    public string PName = "Player";
    public int PColor = 0;
    public bool hasJoined = false;

    public override void HandleMessage(string flag, string value)
    {
       if (flag == "NAME")
        {
            PName = value;
            if (IsServer)
            {
                SendUpdate("NAME", value);
            }
        }

       if (flag == "COLOR")
        {
            PColor = int.Parse(value);
            if (IsServer)
            {
                SendUpdate("COLOR", value);
            }
        }

        if (flag == "JOIN")
        {
            hasJoined = true;
            this.transform.GetChild(0).gameObject.SetActive(false);
            if (IsServer)
            {
                MyCore.NetCreateObject(5, int.Parse(value));
                SendUpdate("JOIN", value);
            }
        }
    }

    public override void NetworkedStart()
    {
        if (!IsLocalPlayer)
        {
            this.transform.GetChild(0).gameObject.SetActive(false);
        }
    }

    public override IEnumerator SlowUpdate()
    {
        while (IsConnected)
        {
            if (IsServer)
            {

                if (IsDirty)
                {
                    SendUpdate("NAME", PName);
                    SendUpdate("COLOR", PColor.ToString());
                    IsDirty = false;
                }
            }
            yield return new WaitForSeconds(.1f);
        }
    }

    // Start is called before the first frame update
    void Start()
    {
        
    }

    public void GetName(string name) 
    { 
        if(IsServer)
        {
            PName = name;
        }
    }
    public void GetColor(int color)
    {
        if (IsServer)
        {
           PColor = color;
        }
    }
    public void UI_NameInput(string s)
    {
        if (IsLocalPlayer)
        {
            SendCommand("NAME", s);
        }

    }
    public void UI_ColorInput(int c)
    {
        if (IsLocalPlayer)
        {
            SendCommand("COLOR", c.ToString());
        }
    }

    public void UI_JoinButton()
    {
        if (IsLocalPlayer)
        {
            SendCommand("JOIN", this.Owner.ToString());
        }
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
