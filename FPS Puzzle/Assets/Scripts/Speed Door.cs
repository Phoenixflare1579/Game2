using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpeedDoor : MonoBehaviour
{
    bool open = false;
    public string type;//Corner,T&B,Sides;
    AudioSource audioc;
    private void Start()
    {
        audioc = GetComponent<AudioSource>();
        this.gameObject.GetComponent<Animator>().SetTrigger(type + " Close");
    }
    private void OnTriggerStay(Collider other)
    {
        if (other.gameObject.tag == "Player" && Time.timeScale >= 1f)
        {
            if (!open)
            {
                open = true;
                this.gameObject.GetComponent<Animator>().SetTrigger(type);
                GetComponent<MeshCollider>().enabled = false;
                audioc.Play();
            }
        }
        else
        {
            if (open)
            {
                open = false;
                this.gameObject.GetComponent<Animator>().SetTrigger(type + " Close");
                GetComponent<MeshCollider>().enabled = true;
                audioc.Play();
            }
        }
    }
}
