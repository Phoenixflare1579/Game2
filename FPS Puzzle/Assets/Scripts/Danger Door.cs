using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DangerDoor : MonoBehaviour
{
    bool broken = false;
    public AnimationClip open;
    public void Update()
    {
        foreach(GameObject p in GameObject.FindGameObjectsWithTag("Player"))
        {
            if (Vector3.Distance(p.transform.position,this.gameObject.transform.position) < 15f)
            {
                broken = true;
            }
        }
        if (Time.timeScale >= 15 && broken)
        {
            GetComponent<Animation>().clip = open;
            this.GetComponent<BoxCollider>().enabled = false;
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
