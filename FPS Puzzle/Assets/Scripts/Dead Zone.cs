using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DeadZone : MonoBehaviour
{
    private void OnTriggerStay(Collider other)
    {
        if(other.gameObject.tag == "Player")
        {
            other.GetComponent<PlayerInfo>().DeadZone = true;
        }
        if (other.gameObject.tag == "Android")
        {
            other.GetComponent<MovingEnemyNavmesh>().DeadZone = true;
        }
    }
    private void OnTriggerExit(Collider other)
    {
        if (other.gameObject.tag == "Player")
        {
            other.GetComponent<PlayerInfo>().DeadZone = false;
        }
    }
}
