using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class DangerDoor : MonoBehaviour
{
    AudioSource audioc;
    // Start is called before the first frame update
    void Start()
    {
        audioc = GetComponent<AudioSource>();
        this.gameObject.GetComponent<Animator>().SetTrigger("DD");
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.tag == "Player")
        {
            collision.gameObject.GetComponent<PlayerInfo>().SendCommand("HP", (collision.gameObject.GetComponent<PlayerInfo>().HP - 1).ToString());
        }
    }
    private void OnTriggerStay(Collider other)
    {
        if (other.gameObject.tag == "Player" && Time.timeScale >= 10f)
        {
            this.gameObject.GetComponent<Animator>().SetTrigger("Corner");
            audioc.loop = false;

        }
    }
}
