using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MovingEnemy : MonoBehaviour
{
    Rigidbody rb;
    public int direction=1;
    void Start()
    {
        rb = GetComponent<Rigidbody>();
        rb.velocity = new Vector3(1,0,0)*direction;
        StartCoroutine(changedirection());
    }

    IEnumerator changedirection()
    {
        yield return new WaitForSecondsRealtime(10);
        direction *= -1;
        Start();
    }
}
