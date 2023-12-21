using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using NETWORK_ENGINE;

public class PlayerInfo : NetworkComponent
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
    public override void HandleMessage(string flag, string value) { }
    public override void NetworkedStart()
    { }
    public override IEnumerator SlowUpdate()
    { yield return new WaitForSeconds(.1f); }
}
