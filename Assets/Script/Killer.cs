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
    private int nextNodeID = 0;
    public int KillScore = 0; // Number of students killed
    public float viewAngle = 160;
    public float shootTime = 1; // Time to shoot
    private NavMeshAgent agent;
    private GameObject exit;
    private GameObject GameController;
    private GameObject killTarget;
    private List<GameObject> nodes = new List<GameObject>();
    private List<GameObject> nodesInSight = new List<GameObject>();
    private List<GameObject> studentsInSight = new List<GameObject>();
    private List<GameObject> studentInScene = new List<GameObject>();
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
            studentInScene.Add(a);
        }
        foreach (GameObject n in GameObject.FindGameObjectsWithTag("killerPathPoint"))
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
                Shoot();
            }
            else
            {
                if (!agent.pathPending && (agent.remainingDistance - agent.stoppingDistance) < 0.1f)
                    agent.SetDestination(NextNode().transform.position);
            }
        }
        else if (GameController.GetComponent<GameControllerCode>().getStatus() == -1)
            agent.GetComponent<NavMeshAgent>().enabled = false;
    }

    IEnumerator StandBeforeShoot()
    {
        yield return new WaitForSeconds(shootTime);
    }

    void Shoot()
    {
        agent.enabled = false;
        GameController.GetComponent<GameControllerCode>().shootCounter++;
        GameController.GetComponent<GameControllerCode>().shootHeard();
        StandBeforeShoot();
        if (Random.Range(0.0f, 1.0f) < KP) // Kill success
        {
            // kill success
            killTarget.GetComponent<StudentLearn>().Dead();
            KillScore++;
            studentInScene.Remove(killTarget);
        }
        agent.enabled = true;
    }
    GameObject NextNode()
    {
        // Get the next node to go
        GameObject node = null;
        foreach(GameObject n in nodes)
        {
            if(n.GetComponent<Node>().index == nextNodeID)
                node = n;
        }
        Debug.Log("killer heading to point"+nextNodeID.ToString());
        nextNodeID ++;
        return node;
    }

    GameObject GetStudentInSight()
    {
        // Find a studnet in sight to kill
        NavMeshHit hit;
        float angle;
        studentsInSight.Clear();
        bool found = false;
        GameObject studentFound = null;
        foreach (GameObject student in studentInScene)
        {
            if (student.GetComponent<StudentLearn>().inBuilding == 1)
            {
                if (student.GetComponent<StudentLearn>().isHiding())
                { // Hiding students are not easy to find
                    if (Random.Range(0, 2) == 1)
                    {
                        if (student != null && !agent.Raycast(student.transform.position, out hit)) // no obstacle. student is alive
                        {
                            angle = Vector3.Angle(student.transform.position - transform.position, transform.forward);
                            if (angle < viewAngle / 2)
                            {
                                found = true;
                                studentsInSight.Add(student);
                            }

                        }
                    }
                }
                else
                {
                    if (student != null && !agent.Raycast(student.transform.position, out hit)) // no obstacle. student is alive
                    {
                        angle = Vector3.Angle(student.transform.position - transform.position, transform.forward);
                        if (angle < viewAngle / 2)
                        {
                            found = true;
                            studentsInSight.Add(student);
                        }
                    }
                }
            }
        }
        if (!found)
            return null;
        studentFound = studentsInSight[Random.Range(0, studentsInSight.Count)];
        return studentFound;
    }
}
