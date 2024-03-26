using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AnimationLoop : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        StartCoroutine(Loop());
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public IEnumerator Loop()
    {
        this.GetComponent<Animation>().Play();
        yield return new WaitForSeconds(2f);
        StartCoroutine(Loop());
    }
}
