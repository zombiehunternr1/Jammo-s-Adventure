using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerCamera : MonoBehaviour
{
    public Transform target;
    public Vector3 targetOffset = new Vector3(0f, 3.5f, 7);
    public float followSpeed = 6;
    public float inputRotationSpeed = 100;
    public float rotateDamping = 100;
    public float minDistance = 5;

    private Transform followTarget;
    private Vector3 defTargetOffset;
    private bool camColliding;

    //setup objects
    void Awake()
    {
        followTarget = new GameObject().transform;
        followTarget.name = "Camera Target";

        defTargetOffset = targetOffset;
    }

    //run our camera functions each frame
    void LateUpdate()
    {
        AdjustCamera();
        if (target)
        {
            SmoothFollow();
            if (rotateDamping > 0)
                SmoothLookAt();
            else
                transform.LookAt(target.position);
        }
    }

    //rotate smoothly toward the target
    void SmoothLookAt()
    {
        Quaternion rotation = Quaternion.LookRotation(target.position - transform.position);
        transform.rotation = Quaternion.Slerp(transform.rotation, rotation, rotateDamping * Time.deltaTime);
    }

    //move camera when it clips walls
    void AdjustCamera()
    {
        //move cam in/out
        if (camColliding == true)
        {
            if (targetOffset.magnitude > minDistance)
                targetOffset *= 0.99f;
        }
        else
            targetOffset *= 1.01f;

        if (targetOffset.magnitude > defTargetOffset.magnitude)
            targetOffset = defTargetOffset;
    }

    //move camera smoothly toward its target
    void SmoothFollow()
    {
        //move the followTarget (empty gameobject created in awake) to correct position each frame 
        followTarget.position = target.position;
        followTarget.Translate(targetOffset, Space.Self);

        //camera moves to the followTargets position
        transform.position = Vector3.Lerp(transform.position, followTarget.position, followSpeed * Time.deltaTime);
    }
}
