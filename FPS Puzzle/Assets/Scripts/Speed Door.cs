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
        if (other.gameObject.tag == "Player" && Time.timeScale >= 0.8f && !open)
        {
             open = true;
             this.gameObject.GetComponent<Animator>().SetTrigger(type);
             audioc.Play();
        }
        else if (other.gameObject.tag == "Player" && Time.timeScale < 0.8f && open)
        {
             open = false;
             this.gameObject.GetComponent<Animator>().SetTrigger(type + " Close");
             audioc.Play();
        }
    }
}
