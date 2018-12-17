using System.Collections.Generic;
using UnityEngine;
using MLAgents;

public class PlayerAgent : Agent
{
    Rigidbody2D rBody;
    Vector3 startPosition;


    void Start()
    {
        rBody = GetComponent<Rigidbody2D>();
        startPosition = transform.position;
        float distanceToTarget = Vector3.Distance(this.transform.position,
                                          Target.position);
        previousDistance = distanceToTarget;
    }

    public Transform Target;
    public override void AgentReset()
    {
        if (this.transform.position.y < -1.0)
        {
            // The Agent fell
            this.transform.position = startPosition;
            this.rBody.angularVelocity = 0;
            this.rBody.velocity = Vector3.zero;
            //float distanceToTarget = Vector3.Distance(this.transform.position,
            //                      Target.position);
            //previousDistance = distanceToTarget;
        }
        else
        {
            // Reset Agent
            //this.transform.position = startPosition;
            //this.rBody.angularVelocity = 0;
            //this.rBody.velocity = Vector3.zero;

            // Move the target to a new spot
            Target.position = new Vector3(Random.value * 5 - 5, Target.position.y);

            //float distanceToTarget = Vector3.Distance(this.transform.position,
            //          Target.position);
            //previousDistance = distanceToTarget;

            // Need to make sure it does not get placed right on player?
        }
    }

    public override void CollectObservations()
    {
        // Calculate relative position
        Vector3 relativePosition = Target.position - this.transform.position;

        // Relative position
        AddVectorObs(relativePosition.x);
        //AddVectorObs(relativePosition.y / 5);

        //// Distance to edges of platform
        //AddVectorObs((this.transform.position.x + 5) / 5);
        //AddVectorObs((this.transform.position.x - 5) / 5);
        //AddVectorObs((this.transform.position.z + 5) / 5);
        //AddVectorObs((this.transform.position.z - 5) / 5);

        // Agent velocity
        AddVectorObs(rBody.velocity.x);
        //AddVectorObs(rBody.velocity.y / 5);
    }

    public float speed = 10;
    private float previousDistance = float.MaxValue;

    public override void AgentAction(float[] vectorAction, string textAction)
    {
        //print(vectorAction[0]);
        // Rewards
        float distanceToTarget = Vector3.Distance(this.transform.position,
                                                  Target.position);

        // Reached target
        if (distanceToTarget < 1.1f)
        {
            AddReward(1.0f);
            Done();
        }

        var movement = (int)vectorAction[0];

        int direction = 0;

        switch (movement)
        {
            case 1:
                direction = -1;
                break;
            case 2:
                direction = 1;
                break;
        }

        float distanceChange = previousDistance - distanceToTarget;

        // Move towards target
        if(distanceChange > 0)
        {
            //print("Getting Closer!");
            //print(previousDistance);
            //print(distanceToTarget);
            AddReward(0.1f);// * distanceChange * 100);
        }
        previousDistance = distanceToTarget;

        // Time penalty
        AddReward(-0.05f);

        // Fell off platform
        if (this.transform.position.y < -1.0)
        {
            AddReward(-1.0f);
            Done();
        }

        //print(this.GetCumulativeReward());

        // Actions, size = 1
        //Vector3 controlSignal = Vector3.zero;
        //controlSignal.x = vectorAction[0];
        //controlSignal.z = vectorAction[1];

        rBody.AddForce(Vector2.right * direction * speed);
        //rBody.velocity = new Vector2(vectorAction[0] * speed, rBody.velocity.y);
        //print(controlSignal);
    }
}