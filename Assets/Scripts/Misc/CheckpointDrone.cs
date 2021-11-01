using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CheckpointDrone : MonoBehaviour
{
    public float YHeight = 5f;
    public float Speed = 1f;

    private void Start()
    {
        StartCoroutine(GoToSetHeight());
    }

    private IEnumerator GoToSetHeight()
    {
        while(transform.position.y != YHeight)
        {
            transform.position = Vector3.Lerp(transform.position, new Vector3(transform.position.x, YHeight, transform.position.z), Speed * Time.deltaTime);
            if(transform.position.y > YHeight)
            {
                StopAllCoroutines();
                transform.position = new Vector3(transform.position.x, YHeight, transform.position.z);
            }
            yield return transform.position.y;
        }
        yield return null;
    }
}
