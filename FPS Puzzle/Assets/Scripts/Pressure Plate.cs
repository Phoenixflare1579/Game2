using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PressurePlate : MonoBehaviour
{
    public bool active;
    public Material green;
    public Material basic;
    public bool stop;
    public void Update()
    {
        if(active)
        {
            GetComponent<SkinnedMeshRenderer>().material = green;
        }
        else
        {
            GetComponent<SkinnedMeshRenderer>().material = basic;
        }
    }
    private void OnCollisionStay(Collision collision)
    {
        active = true;
    }
    private void OnCollisionExit(Collision collision)
    {
        active=false;
    }
}
