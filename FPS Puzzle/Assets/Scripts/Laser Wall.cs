using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using NETWORK_ENGINE;

public class LaserWall : MonoBehaviour
{
    bool open = false;
    private void OnCollisionEnter(Collision collision)
    {
        if(collision.gameObject.tag == "Player")
        {
            collision.gameObject.GetComponent<PlayerInfo>().SendCommand("HP",0.ToString());
        }
    }
    public void Update()
    {
        if(Time.timeScale < 1f && !open)
        {
            this.GetComponent<Animator>().SetTrigger("Full");
            open = true;
        }
        else if (Time.timeScale >= 1f && open)
        {
            this.GetComponent<Animator>().SetTrigger("Close");
            open = false;
        }
        if(open)
        {
            this.GetComponent<Collider>().enabled = false;
        }
        else
        {
            this.GetComponent<Collider>().enabled = true;
        }
    }
}
