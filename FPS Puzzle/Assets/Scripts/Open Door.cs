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
    AudioSource audioc;

    private void Start()
    { 
        audioc = GetComponent<AudioSource>();
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
            if (lockbreaker.GetComponent<Target>() != null)
            {
                if ((locked && lockbreaker.GetComponent<Target>().hit))
                {
                    open = true;
                    this.gameObject.GetComponent<Animator>().SetTrigger(type);
                    audioc.Play();
                    locked = false;
                }
            }
            else if (lockbreaker.GetComponent<PressurePlate>() != null)
            {
                if ((locked && lockbreaker.GetComponent<PressurePlate>().active))
                {
                    open = true;
                    this.gameObject.GetComponent<Animator>().SetTrigger(type);
                    audioc.Play();
                    locked = false;
                }
                else if ((!locked && !lockbreaker.GetComponent<PressurePlate>().active))
                {
                    locked = true;
                    open = false;
                    this.gameObject.GetComponent<Animator>().SetTrigger(type + " Close");
                    audioc.Play();
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
            GetComponent<AudioSource>().Play();
        }
    }
}
