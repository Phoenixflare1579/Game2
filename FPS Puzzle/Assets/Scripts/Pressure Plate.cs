using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PressurePlate : MonoBehaviour
{
    public bool active;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private void OnCollisionStay(Collision collision)
    {
        if (collision.transform.position.y > this.transform.position.y)
        {
            GetComponent<Animator>().SetTrigger("Down");
            StartCoroutine(Down());
        }
    }
    private void OnCollisionExit(Collision collision)
    {
        if (collision.transform.position.y > this.transform.position.y)
        {
            GetComponent<Animator>().SetTrigger("Up");
            StartCoroutine(Up());
        }
    }
    public IEnumerator Down()
    {
        yield return new WaitForSeconds(0.7f);
        active = true;
    }
    public IEnumerator Up()
    {
        yield return new WaitForSeconds(0.5f);
        active = false;
    }
}
