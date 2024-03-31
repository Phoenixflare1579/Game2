using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class OpenDoor : MonoBehaviour
{
    bool open = false;
    // Start is called before the first frame update
    void Start()
    {
        
    }
    // Update is called once per frame
    void Update()
    {
        if (open)
        {
            this.transform.parent.gameObject.GetComponent<BoxCollider>().enabled = false;
        }
        else
        {
            this.transform.parent.gameObject.GetComponent<BoxCollider>().enabled = true;
        }
    }
    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.tag == "Player" && !open)
        {
            StartCoroutine(Wait());
            this.transform.parent.gameObject.GetComponent<Animation>().Play();
        }
    }
    public IEnumerator Wait()
    {
        yield return new WaitForSeconds(1);
        open = true;
    }
}
