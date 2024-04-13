using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class OpenDoor : MonoBehaviour
{
    bool open = false;
    public string type;//Corner,T&B,Sides
    public bool isDanger;
    bool broken = false;
    bool done = false;
    public bool locked;
    public GameObject lockbreaker;

    private void Start()
    {
        this.gameObject.GetComponent<Animator>().SetTrigger(type + " Close");
        if (isDanger)
        {
           this.gameObject.GetComponent<Animator>().SetTrigger("DD");
        }
    }
    void Update()
    {
        if (!done && isDanger)
        {
            foreach (GameObject p in GameObject.FindGameObjectsWithTag("Player"))
            {
                if (Vector3.Distance(p.transform.position, this.gameObject.transform.position) < 25f)
                {
                    broken = true;
                }
            }
            if (Time.timeScale >= 10 && broken)
            {
                this.gameObject.GetComponent<Animator>().SetTrigger("Corner");
                done = true;
            }
        }
        if (lockbreaker != null || locked)
        {
            if (lockbreaker == null || (locked && lockbreaker.GetComponent<PressurePlate>().active))
            {
                open = true;
                this.gameObject.GetComponent<Animator>().SetTrigger(type);
                locked = false;
            }
            else if ((!locked && !lockbreaker.GetComponent<PressurePlate>().active) || lockbreaker.name.Contains("target"))
            {
                locked = true;
                open = false;
                this.gameObject.GetComponent<Animator>().SetTrigger(type + " Close");
            }
        }
    }
        private void OnCollisionEnter(Collision collision)
        {
            if (collision.gameObject.tag == "Player" && isDanger)
            {
                collision.gameObject.GetComponent<PlayerInfo>().SendCommand("HP", (collision.gameObject.GetComponent<PlayerInfo>().HP - 1).ToString());
            }
        }
    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.tag == "Player" && !open && !isDanger && !locked)
        {
            open = true;
            this.gameObject.GetComponent<Animator>().SetTrigger(type);
        }
    }
}
