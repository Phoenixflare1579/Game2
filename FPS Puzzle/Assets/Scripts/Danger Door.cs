using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DangerDoor : MonoBehaviour
{
    bool broken = false; 
    public void Update()
    {
        if (Time.timeScale >= 15 && !broken)
        {
            GetComponent<Animation>().RemoveClip("Danger Door");
            GetComponent<Animation>().PlayQueued("Door Corners");
            this.GetComponent<BoxCollider>().enabled = false;
            broken = true;
        }
    }
    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.tag == "Player")
        {
            collision.gameObject.GetComponent<PlayerInfo>().SendCommand("HP", (collision.gameObject.GetComponent<PlayerInfo>().HP-1).ToString());
        }
    }
}
