using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Penguin : MonoBehaviour
{
    public List<Transform> Checkpoints;
    public float Speed;
    public float MinDistanceToCheckpoint = 0.1f;

    private int CheckpointIndex = 0;
    private float Distance;

    void Start()
    {
        transform.LookAt(Checkpoints[CheckpointIndex].position);
    }

    void Update()
    {
        Distance = Vector3.Distance(transform.position, Checkpoints[CheckpointIndex].position);
        if(Distance < MinDistanceToCheckpoint)
        {
            IncreaseIndex();
            //transform.LookAt(Checkpoints[CheckpointIndex].position);
            Vector3 Dir = Checkpoints[CheckpointIndex].position - transform.position;
            Quaternion LookRotation = Quaternion.LookRotation(Dir);
            Vector3 Rotation = Quaternion.Lerp(transform.rotation, LookRotation, Time.deltaTime * Speed).eulerAngles;
            transform.rotation = Quaternion.Euler(0, Rotation.y, 0);
        }
        Patrol();
    }

    void Patrol()
    {
        transform.Translate(Vector3.forward * Speed * Time.deltaTime);
    }

    void IncreaseIndex()
    {
        CheckpointIndex++;
        if(CheckpointIndex >= Checkpoints.Count)
        {
            CheckpointIndex = 0;
        }
    }
}
