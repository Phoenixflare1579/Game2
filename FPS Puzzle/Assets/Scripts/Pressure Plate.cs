using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PressurePlate : MonoBehaviour
{
    public bool active;
    public Material green;
    public Material basic;
    public void Update()
    {
        if(active)
        {
            GetComponent<SkinnedMeshRenderer>().materials[1] = green;
        }
        else
        {
            GetComponent<SkinnedMeshRenderer>().materials[1] = basic;
        }
    }
    private void OnCollisionStay(Collision collision)
    {
        GetComponent<Animator>().SetTrigger("Down");
    }
    private void OnCollisionExit(Collision collision)
    {
        GetComponent<Animator>().SetTrigger("Up");
    }
}
