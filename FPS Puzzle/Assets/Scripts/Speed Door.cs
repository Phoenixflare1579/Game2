using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpeedDoor : MonoBehaviour
{
    bool open = false;
    public string type;//Corner,T&B,Sides;

    private void Start()
    {
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
            }
        }
        else
        {
            if (open)
            {
                open = false;
                this.gameObject.GetComponent<Animator>().SetTrigger(type + " Close");
            }
        }
    }
}