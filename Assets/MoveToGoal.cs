using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.MLAgents;
using Unity.MLAgents.Actuators;
using Unity.MLAgents.Sensors;

//mlagents-learn nn.yaml --run-id=NewObs0 --resume

public class MoveToGoal : Agent
{
    [SerializeField] public TrackCheckpoints trackCheckpoints;
    float distanceReward = 0;

    public void ResetCheckpoints()
    {
        foreach (GameObject c in trackCheckpoints.checkpointsList)
        {
            c.transform.localPosition = new Vector3(Random.Range(-7f, 7f), 0, c.transform.localPosition.z);
            c.SetActive(false);
        }

        trackCheckpoints.checkpointsList[0].SetActive(true);
    }

    public void Start()
    {
        trackCheckpoints = GetComponentInParent<TrackCheckpoints>();
    }

    public override void OnEpisodeBegin()
    {
        ResetCheckpoints();
        transform.localPosition = new Vector3(Random.Range(-4f, 4), 0, -20);
        distanceReward = 0;

        trackCheckpoints.nextCheckpointIndex = 0;
    }

    public override void CollectObservations(VectorSensor sensor)
    {
        sensor.AddObservation(transform.localPosition);
        sensor.AddObservation(trackCheckpoints.checkpointsList[trackCheckpoints.nextCheckpointIndex].transform.localPosition);
        sensor.AddObservation(Vector3.Distance(transform.position, trackCheckpoints.checkpointsList[trackCheckpoints.nextCheckpointIndex].transform.position));
    }

    public override void OnActionReceived(ActionBuffers actions)
    {
        distanceReward = 1 / Vector3.Distance(transform.position, trackCheckpoints.checkpointsList[trackCheckpoints.nextCheckpointIndex].transform.position);

        float moveX = actions.ContinuousActions[0];
        float moveZ = actions.ContinuousActions[1];

        float moveSpeed = 25f;
        transform.position += new Vector3(moveX, 0, moveZ) * Time.deltaTime * moveSpeed;
    }

    void OnTriggerEnter(Collider other)
    {
        if (other.tag.Equals("Goal"))
        {
            bool right = trackCheckpoints.WentThroughCheckpoint(other.gameObject);

            if (right == true)
            {
                if (trackCheckpoints.nextCheckpointIndex == trackCheckpoints.checkpointsList.Count - 1)
                {
                    AddReward(10);
                    EndEpisode();
                }
                else
                {
                    trackCheckpoints.checkpointsList[trackCheckpoints.nextCheckpointIndex].gameObject.SetActive(false);
                    trackCheckpoints.checkpointsList[trackCheckpoints.nextCheckpointIndex + 1].gameObject.SetActive(true);
                    trackCheckpoints.nextCheckpointIndex++;

                    AddReward(1.5f + distanceReward);
                }
            }
            else
            {
                AddReward(-0.5f);
            }
        }
    }

    void OnCollisionStay(Collision other)
    {
        if (other.gameObject.tag.Equals("Wall"))
        {
            Debug.Log("Wall");
            SetReward(-2f);
            EndEpisode();
        }
    }
}
