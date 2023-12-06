using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class PlayerInfo : MonoBehaviour
{
    int MaxHP;
    int HP;
    // Start is called before the first frame update
    void Start()
    {
        MaxHP = 3;
        HP = MaxHP;
    }

    // Update is called once per frame
    void Update()
    {
        if (HP == 0)
        {
            SceneManager.LoadScene("Game Over");
        }
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.tag == "Projectile") 
        {
            HP -= 1;
        }
    }
}
