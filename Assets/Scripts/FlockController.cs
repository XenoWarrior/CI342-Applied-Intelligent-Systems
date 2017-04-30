using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FlockController : MonoBehaviour {

    public GameObject Agent;

    public static int TotalAgents = 20;
    public static List<GameObject> AgentList = new List<GameObject>();
    public static Vector3 GoalPosition = Vector3.zero;
    public static float OpenSpace = 5;

	// Use this for initialization
	void Start ()
    {
        // Add some agents to the screen
        for(int i = 0; i < TotalAgents; i++)
        {
            var AgentPos = new Vector3(Random.RandomRange(-OpenSpace, OpenSpace), Random.RandomRange(-OpenSpace, OpenSpace), Random.RandomRange(-OpenSpace, OpenSpace));
            AgentList.Add(Instantiate(Agent, AgentPos, Quaternion.identity));
        }
	}
	
	// Update is called once per frame
	void Update ()
    {
        // Update the random goal position if the chance is less than 50
		if(Random.Range(0, 10000) < 50)
        {
            GoalPosition = new Vector3(Random.RandomRange(-OpenSpace, OpenSpace), Random.RandomRange(-OpenSpace, OpenSpace), Random.RandomRange(-OpenSpace, OpenSpace));
        }
	}
}
