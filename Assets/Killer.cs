﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class Killer : MonoBehaviour
{

    // NOTATION:
    // These variables are the properties of the killer
    // The killer search for students and shoot them
    // Killers implement INDISCRIMINATE KILLING
    // END(3.26)
    public float KP; // Kill probability
    public int ID; // ID of the killer
    public int KillScore = 0; // Number of students killed
    private NavMeshAgent agent;
    private GameObject exit;
    private GameObject GameController;
    private GameObject killTarget;
    private List<GameObject> nodes = new List<GameObject>();
    private List<GameObject> nodesInSight = new List<GameObject>();
    private List<GameObject> studentsInSight = new List<GameObject>();
    private List<GameObject> studentsAlive = new List<GameObject>();
    private List<int> visited = new List<int>();

    // Use this for initialization
    void Start()
    {
        KP = 0.01f;
        exit = GameObject.FindGameObjectWithTag("exit");
        GameController = GameObject.FindGameObjectWithTag("GameController");

        agent = GetComponent<NavMeshAgent>();
        foreach (GameObject a in GameObject.FindGameObjectsWithTag("agent"))
        {
            studentsAlive.Add(a);
        }
        foreach (GameObject n in GameObject.FindGameObjectsWithTag("node"))
        {
            nodes.Add(n);
            visited.Add(0);
        }
    }

    // Update is called once per frame
    void Update()
    {
        if (GameController.GetComponent<GameControllerCode>().getStatus() == 1)
        {
            killTarget = GetStudentInSight();
            if (killTarget != null)
            {
                // kill student
                if (Random.Range(0.0f, 1.0f) < KP) // Kill success
                {
                    // kill success
                    killTarget.GetComponent<Student>().Dead();
                    KillScore++;
                    studentsAlive.Remove(killTarget);
                }
            }
            else
            {
                if (!agent.pathPending && (agent.remainingDistance - agent.stoppingDistance) < 0.1f)
                    agent.SetDestination(RandomNode().transform.position);
            }
        }
        else if(GameController.GetComponent<GameControllerCode>().getStatus() == 1)
            agent.GetComponent<NavMeshAgent>().enabled = false;
    }

    GameObject RandomNode()
    {
        // Get a node randomly
        GameObject node = null;
        node = nodes[Random.Range(0, nodes.Count)];
        return node;
    }

    GameObject GetStudentInSight()
    {
        // find a studnet in sight to kill
        NavMeshHit hit;
        studentsInSight.Clear();
        bool found = false;
        GameObject studentFound = null;
        foreach (GameObject student in studentsAlive)
        {
            if (agent != null && !agent.Raycast(student.transform.position, out hit)) // no obstacle. student is alive
            {
                found = true;
                studentsInSight.Add(student);
            }
        }
        if (!found)
            return null;
        studentFound = studentsInSight[Random.Range(0, studentsInSight.Count)];
        return studentFound;
    }
}