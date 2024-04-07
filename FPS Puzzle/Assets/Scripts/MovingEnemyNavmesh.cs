using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using NETWORK_ENGINE;
using UnityEngine.AI;
using UnityEditor;

public class MovingEnemyNavmesh : NetworkComponent
{

    public NavMeshAgent myAgent;
    public List<GameObject> goals;
    public GameObject currentGoal;
    public override void HandleMessage(string flag, string value)
    {
        
    }

    public override void NetworkedStart()
    {
        
    }

    public override IEnumerator SlowUpdate()
    {
        if (!myAgent.hasPath)
        {
            foreach (GameObject g in goals)
            {
                if (g != currentGoal)
                {
                    currentGoal = g;
                    myAgent.destination = currentGoal.transform.position;
                }
            }
        }
        yield return new WaitForSeconds(0.1f);
    }

    // Start is called before the first frame update
    void Start()
    {
        GameObject[] temp = GameObject.FindGameObjectsWithTag("NavPoint");
        goals = new List<GameObject>();
        foreach (GameObject g in temp)
        {
            goals.Add(g);
        }
        currentGoal = GameObject.FindGameObjectWithTag("NavPoint");
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
