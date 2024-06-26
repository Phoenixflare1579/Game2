using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using NETWORK_ENGINE;
using UnityEngine.AI;
using UnityEditor;
using static UnityEngine.GraphicsBuffer;

public class MovingEnemyNavmesh : NetworkComponent
{

    public NavMeshAgent myAgent;
    public List<GameObject> goals;
    public int index = 0;
    public bool DeadZone = false;
    GameObject target;
    public override void HandleMessage(string flag, string value)
    {
        
    }

    public override void NetworkedStart()
    {
        
    }

    public override IEnumerator SlowUpdate()
    {
        while (IsConnected)
        {
            if (IsServer)
            {
                myAgent.destination = goals[0].transform.position;
                if (DeadZone)
                {
                    tag = "Enemy";
                    GetComponent<Animator>().SetBool("Cube", true);
                    if (target != null)
                    {
                        myAgent.SetDestination(target.transform.position);
                        myAgent.speed = 15f;
                    }
                }
                else if (Vector3.Distance(myAgent.transform.position, myAgent.destination) <= 2f)
                {
                    index++;
                    if (index >= goals.Count)
                    {
                        index = 0;
                    }

                    myAgent.SetDestination(goals[index].transform.position);
                }
            }
            yield return new WaitForSeconds(0.1f);
        }
    }

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if (IsServer)
        {
            if (GameObject.FindGameObjectWithTag("Player") != null)
            {
                float distance = 10f;
                foreach (GameObject p in GameObject.FindGameObjectsWithTag("Player"))
                {
                    if (Vector3.Distance(this.transform.position, p.transform.position) < distance && p.GetComponent<PlayerInfo>().DeadZone)
                    {
                        distance = Vector3.Distance(this.transform.position, p.transform.position);
                        target = p;
                    }
                }
            }
        }
    }
}
