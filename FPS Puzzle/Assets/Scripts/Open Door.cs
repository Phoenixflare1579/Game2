using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class OpenDoor : MonoBehaviour
{
    bool open = false;
    public string type;//Corner,T&B,Sides
    public bool locked;
    public bool lockable;
    public GameObject lockbreaker;
    AudioSource audio;

    private void Start()
    { 
        audio = GetComponent<AudioSource>();
        this.gameObject.GetComponent<Animator>().SetTrigger(type + " Close");
        if (lockable)
        {
            locked = true;
        }
    }
    void Update()
    {
        if (lockable)
        {
            if (lockbreaker == null)
            {
                open = true;
                this.gameObject.GetComponent<Animator>().SetTrigger(type);
            }
            else if (lockbreaker.name.Contains("p"))
            {
                if ((locked && lockbreaker.GetComponent<PressurePlate>().active))
                {
                    open = true;
                    this.gameObject.GetComponent<Animator>().SetTrigger(type);
                    audio.Play();
                    locked = false;
                }
                else if ((!locked && !lockbreaker.GetComponent<PressurePlate>().active))
                {
                    locked = true;
                    open = false;
                    this.gameObject.GetComponent<Animator>().SetTrigger(type + " Close");
                    audio.Play();
                }
            }
        }
    }
        
    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.tag == "Player" && !open && !locked)
        {
            open = true;
            this.gameObject.GetComponent<Animator>().SetTrigger(type);
            audio.Play();
        }
    }
}
