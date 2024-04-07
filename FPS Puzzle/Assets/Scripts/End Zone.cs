using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EndZone : MonoBehaviour
{
    bool check;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.tag == "Player" && !check)
        {
            foreach (GameObject p in GameObject.FindGameObjectsWithTag("Player"))
            {
                p.GetComponent<PlayerInfo>().SendUpdate("End", string.Empty);
                check = true;
            }
        }
    }
}
