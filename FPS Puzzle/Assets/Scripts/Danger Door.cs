using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DangerDoor : MonoBehaviour
{
    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.tag == "Player")
        {
            collision.gameObject.GetComponent<PlayerInfo>().SendCommand("HP", (collision.gameObject.GetComponent<PlayerInfo>().HP-1).ToString());
            collision.gameObject.GetComponent<Rigidbody>().velocity *= -1;
        }
    }
}
