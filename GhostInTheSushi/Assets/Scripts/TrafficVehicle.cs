﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TrafficVehicle : MonoBehaviour {

    static float ARRIVAL_DISTANCE = 1.0f;

    private UnityEngine.AI.NavMeshAgent agent;

    private TrafficLight trafficLight;

    private float defaultSpeed;

    private List<GameObject> StopStakeholders = new List<GameObject>();
    private List<GameObject> GoStakeholders = new List<GameObject>();

    // Use this for initialization
    void Awake () {
        agent = GetComponent<UnityEngine.AI.NavMeshAgent>();
        defaultSpeed = agent.speed;
	}

    // Gets called when the traffic spawner makes the vehicle
    public void SetTarget(GameObject target)
    {
        agent.SetDestination(target.transform.position);
    }
	
	void FixedUpdate () {
	    if (agent.hasPath && Vector3.Distance(transform.position, agent.destination) < ARRIVAL_DISTANCE)
        {
            // Delete the entire vehicle
            Destroy(transform.gameObject);
        }

        // traffic light logic, wait for the light to change
        if(trafficLight && trafficLight.IsGo)
        {
            trafficLight = null;
            Continue(this.gameObject);
        }
	}

    // When the vehicle approaches a traffic light, the light will call this method to tell the car there is a light in front of it
    public void AssignTrafficLight(TrafficLight trafficLight)
    {
        // If its green, we're just going to pass through it so ignore
        if (trafficLight.IsGo)
            return;

        // Otherwise it's red, so hold a reference and wait for the light to change
        this.trafficLight = trafficLight;
        Stop(this.gameObject);
    }

    // Pass in a gameObject to say that gameObject wants the traffic vehicle to stop
    // We can then arbitrate between the stakeholders to decide on the behaviour
    public void Stop(GameObject stakeholder)
    {
        StopStakeholders.Add(stakeholder);
        GoStakeholders.Remove(stakeholder);

        updateStakeholders();
    }

    // Pass in a gameObject to say that gameObject wants the traffic vehicle to stop
    // We can then arbitrate between the stakeholders to decide on the behaviour
    public void Continue(GameObject stakeholder)
    {
        StopStakeholders.Remove(stakeholder);
        GoStakeholders.Add(stakeholder);

        updateStakeholders();
    }

    private void updateStakeholders()
    {
        // If anyone is saying stop, then stop
        if (StopStakeholders.Count > 0)
        {
            agent.speed = 0;
        }
        else
        {
            agent.speed = defaultSpeed;
        }
    }
}