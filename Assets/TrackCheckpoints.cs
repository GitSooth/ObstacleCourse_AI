using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TrackCheckpoints : MonoBehaviour
{
    public List<GameObject> checkpointsList;
    public int nextCheckpointIndex;
    private void Awake()
    {
        checkpointsList = new List<GameObject>();

        Transform checkpointsTransform = transform.Find("Checkpoints");

        foreach (Transform checkpoint in checkpointsTransform)
        {
            checkpointsList.Add(checkpoint.gameObject);
        }

        nextCheckpointIndex = 0;
    }

    public bool WentThroughCheckpoint(GameObject checkpoint)
    {
        if (checkpointsList.IndexOf(checkpoint) == nextCheckpointIndex && nextCheckpointIndex <= checkpointsList.Count)
        {
            Debug.Log("Correct");
            return true;
        }
        else
        {
            Debug.Log("Wrong");
            return false;
        }
    }
}
