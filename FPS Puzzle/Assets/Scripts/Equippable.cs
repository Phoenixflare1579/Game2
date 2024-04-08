using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class Equippable : MonoBehaviour
{
    public Rigidbody rb;
    public BoxCollider coll;
    public Transform player, gun, cam;
    Vector3 distancetoplayer;

    public float PickupRange= 4f;
    public float dropForward = 2f, dropUpward = -1f;

    public bool equipped;
    public static bool Full;

    private void PickUp()
    {
        equipped = true;
        Full = true;

        transform.SetParent(gun);

        transform.localScale = Vector3.one;
        transform.localRotation = Quaternion.Euler(Vector3.zero);
        transform.localPosition = Vector3.zero;

        rb.isKinematic = true;
        coll.isTrigger = true;


    }
    private void Drop()
    {
        equipped = false;
        Full = false;

        transform.SetParent(null);

        rb.velocity = player.GetComponent<Rigidbody>().velocity;

        rb.AddForce(cam.forward* dropForward, ForceMode.Impulse);
        rb.AddForce(cam.up * dropUpward, ForceMode.Impulse);

        rb.isKinematic = false;
        coll.isTrigger = false;
    }
    // Start is called before the first frame update
    void Start()
    {
        if (!equipped)
        {
            rb.isKinematic = false;
            coll.isTrigger = false;
        }
        else if (equipped) 
        {
            rb.isKinematic = true;
            coll.isTrigger = true;
            Full = true;
        }
    }

    // Update is called once per frame
    void Update()
    {
        distancetoplayer=player.transform.position-transform.position;
    }

    public void Grab(InputAction.CallbackContext c)
    {
        if (!equipped && distancetoplayer.magnitude <= PickupRange && !Full && c.started)
        {
            PickUp();
        }
    }
    public void DropItem(InputAction.CallbackContext c)
    {
        if (equipped && c.started)
        {
            Drop();
        }
    }
}
