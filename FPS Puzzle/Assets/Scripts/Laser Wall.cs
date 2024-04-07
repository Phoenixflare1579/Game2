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
            this.GetComponent<BoxCollider>().enabled = false;
            open = true;
        }
        else if (Time.timeScale >= 1f && open)
        {
            this.GetComponent<Animator>().SetTrigger("Close");
            this.GetComponent<BoxCollider>().enabled = true;

            open = false;
        }
    }
}
