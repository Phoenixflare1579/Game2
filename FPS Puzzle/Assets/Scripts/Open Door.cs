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

    private void Start()
    {
        if(isDanger)
        {
           this.gameObject.GetComponent<Animator>().SetTrigger("DD");
        }
    }
    void Update()
    {
        if (open)
        {
            this.gameObject.GetComponent<MeshCollider>().enabled = false;
        }
        else
        {
            this.gameObject.GetComponent<MeshCollider>().enabled = true;
        }
        if (!done && isDanger)
        {
            foreach (GameObject p in GameObject.FindGameObjectsWithTag("Player"))
            {
                if (Vector3.Distance(p.transform.position, this.gameObject.transform.position) < 25f)
                {
                    broken = true;
                }
            }
            if (Time.timeScale >= 15 && broken)
            {
                this.GetComponent<MeshCollider>().enabled = false;
                this.gameObject.GetComponent<Animator>().SetTrigger("Corner");
                done = true;
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
        if (other.gameObject.tag == "Player" && !open && !isDanger)
        {
            StartCoroutine(Wait());
            this.gameObject.GetComponent<Animator>().SetTrigger(type);
        }
    }
    public IEnumerator Wait()
    {
        yield return new WaitForSeconds(1);
        open = true;
        yield return new WaitForSeconds(5);
        this.gameObject.GetComponent<Animator>().SetTrigger(type + " Close");
        open = false;
    }
}
