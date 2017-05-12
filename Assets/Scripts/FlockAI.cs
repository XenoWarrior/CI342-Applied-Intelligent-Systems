using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FlockAI : MonoBehaviour {

    // Speed related for this specific agent.
    public float MaxSpeed = 3.0f;
    public float CurrentSpeed = 1f;
    public float RotationSpeed = 5.0f;

    public bool IsPredator = false;

    // Maximum neighbour distance before leaving group.
    private float NeighbourDistance = 2.0f;

    // Flock
    Vector3 FlockCentre = Vector3.zero;
    Vector3 FlockAvoid = Vector3.zero;
    Vector3 FlockGoal = FlockController.GoalPosition;
    int FlockSize = 0;
    float GroupSpeed = 0.1f;

    // Use this for initialization
    void Start ()
    {
        // Give each agent its own speed
        this.CurrentSpeed = Random.RandomRange(1, 4);
	}
	
	// Update is called once per frame
	void Update ()
    {
        if(IsPredator)
        {
            CurrentSpeed = 5.0f;
        }

        // Making sure that the agent remains in a certain space
        if (Vector3.Distance(transform.position, Vector3.zero) >= FlockController.OpenSpace)
        {
            Vector3 AgentDirection = Vector3.zero - transform.position;
            transform.rotation = Quaternion.Slerp(transform.rotation, Quaternion.LookRotation(AgentDirection), RotationSpeed * Time.deltaTime);
        }
        else
        {
            if (!IsPredator)
            {
                // Apply flocking rules to agent every by a 1 in 5 chance
                if (Random.Range(0, 5) < 1)
                {
                    ApplyRules();
                }

                AvoidPredator();
            }
            else
            {
                // Apply some random predator rules, just to demonstrate the fish avoiding objects.
                if (Random.Range(0, 10) < 3)
                {
                    PredatorRules();
                }
            }
        }

        // Cap the maximum speed from growing above the maximum speed
        if(CurrentSpeed > MaxSpeed && !IsPredator)
        {
            CurrentSpeed = MaxSpeed;
        }

        // Translate towards 0,0 in the application world
        this.transform.Translate(0, 0, Time.deltaTime * CurrentSpeed);
	}

    /// <summary>
    /// Alway avoid predator objects in the world
    /// </summary>
    void AvoidPredator()
    {
        // Calculate predator distance
        Vector3 PredatorAvoid = Vector3.zero;
        float PredatorDistance = Vector3.Distance(FlockController.AgentList[0].transform.position, this.transform.position);

        // If that predator is close
        if(PredatorDistance < 6.0f)
        {
            // Handle avoiding
            PredatorAvoid += this.transform.position - FlockController.AgentList[0].transform.position;

            // Aim the agent away from the predator object
            Vector3 AgentDirection = PredatorAvoid - transform.position;

            // If the direction is not a vector3(0,0,0)
            if (AgentDirection != Vector3.zero)
            {
                // Nicely rotate towards the direction the agent is moving towards.
                transform.rotation = Quaternion.Slerp(transform.rotation, Quaternion.LookRotation(AgentDirection), RotationSpeed * Time.deltaTime);
            }
        }
    }

    /// <summary>
    /// Rules applied to a predator object
    /// </summary>
    void PredatorRules()
    {
        // Pick a random target and move towards it
        var TargetObject = Random.RandomRange(1, FlockController.AgentList.Count - 1);
        transform.rotation = Quaternion.Slerp(transform.rotation, Quaternion.LookRotation(FlockController.AgentList[TargetObject].gameObject.transform.position), RotationSpeed * Time.deltaTime);
    }

    /// <summary>
    /// Handles the primary flocking logic in the application
    /// </summary>
    void ApplyRules()
    {
        // Keeps tracks of the flock centre, avoid and goal positions
        FlockCentre = Vector3.zero;
        FlockAvoid = Vector3.zero;
        FlockGoal = FlockController.GoalPosition;

        // Global group speed
        GroupSpeed = 0.1f;

        // Updated for use by each agent on update rules
        FlockSize = 0;
        float FlockDistance;
        foreach(var Agent in FlockController.AgentList)
        {
            // Make sure we're checking other flock agents
            if(Agent != this.gameObject)
            {
                // Calculate this agents distance from the other agent distance
                FlockDistance = Vector3.Distance(Agent.transform.position, this.transform.position);

                // Ensure it is within the maximum neighbour distance
                if(FlockDistance <= NeighbourDistance)
                {
                    // Update the flock centre point and join the group
                    FlockCentre += Agent.transform.position;
                    FlockSize++;

                    // If we're getting close to another flock agent
                    if (FlockDistance < 1.0f)
                    {
                        // Avoid the flock agent
                        FlockAvoid += this.transform.position - Agent.transform.position;
                    }

                    // Manage the global flock speed
                    FlockAI AgentSpeed = Agent.GetComponent<FlockAI>();
                    GroupSpeed += AgentSpeed.CurrentSpeed;
                }
            }
        }

        // If this agent is in the group
        if(FlockSize > 0)
        {
            // Calculate the centre of the group
            FlockCentre = FlockCentre / FlockSize + (FlockGoal - this.transform.position);

            // Update speed based on flock speed
            CurrentSpeed = GroupSpeed / FlockSize;

            // Aim towards the centre point, minus the avoid point and current position
            Vector3 AgentDirection = (FlockCentre + FlockAvoid) - transform.position;

            // If the direction is not a vector3(0,0,0)
            if(AgentDirection != Vector3.zero)
            {
                // Nicely rotate towards the direction the agent is moving towards.
                transform.rotation = Quaternion.Slerp(transform.rotation, Quaternion.LookRotation(AgentDirection), RotationSpeed * Time.deltaTime);
            }
        }
    }
}
