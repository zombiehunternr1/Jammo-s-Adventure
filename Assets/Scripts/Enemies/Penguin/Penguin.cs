using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Penguin : MonoBehaviour
{
    public List<Transform> Checkpoints;
    public float MoveSpeed;
    public float RotationSpeed;

    private float MinDistanceToCheckpoint = 0.1f;
    private int CheckpointIndex = 0;
    private float Distance;
    private bool IsRotating;

    void Start()
    {
        transform.LookAt(Checkpoints[CheckpointIndex].position);
    }

    void Update()
    {
        Distance = Vector3.Distance(transform.position, Checkpoints[CheckpointIndex].position);
        if(Distance < MinDistanceToCheckpoint)
        {
            transform.position = Checkpoints[CheckpointIndex].position;
            IncreaseIndex();
            IsRotating = true;
        }
        if (!IsRotating)
        {
            Patrol();
        }
        else
        {
            Vector3 Dir = Checkpoints[CheckpointIndex].position - transform.position;
            Quaternion LookRotation = Quaternion.LookRotation(Dir);
            Vector3 Rotation = Quaternion.Slerp(transform.rotation, LookRotation, Time.deltaTime * RotationSpeed).eulerAngles;
            transform.rotation = Quaternion.Euler(0, Rotation.y, 0);
            float Angle = Quaternion.Angle(transform.rotation, LookRotation);
            if (transform.rotation == LookRotation || Angle < 0.6f)
            {
                transform.rotation = Quaternion.RotateTowards(transform.rotation, LookRotation, RotationSpeed);
                IsRotating = false;
            }
        }
    }

    void Patrol()
    {
        transform.Translate(Vector3.forward * MoveSpeed * Time.deltaTime);
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
