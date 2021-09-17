using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Life : MonoBehaviour, ICollectable
{
    Transform LifeUI;
    float Speed = 2;
    float DistanceToTarget;

    private void Awake()
    {
        LifeUI = (Transform)GameObject.Find("LifeModelPosition").gameObject.GetComponent(typeof(Transform));
    }

    public void Collect()
    {
        gameObject.GetComponent<Animator>().SetTrigger("Disable");
        EventManager.CollectLifeDisplay();
        DestroyObject();
    }

    public void DestroyObject()
    {
        StartCoroutine(MoveToUI());
    }

    IEnumerator MoveToUI()
    {
        while(transform.position != LifeUI.transform.position)
        {
            transform.position = Vector3.Lerp(transform.position, LifeUI.position, Speed * Time.deltaTime);
            DistanceToTarget = Vector3.Distance(LifeUI.position, transform.position);
            if (DistanceToTarget < 0.5)
            {
                EventManager.CollectLifeUpdate();
                Destroy(gameObject);
            }
            yield return transform.position;
        }
        yield return null;
    }
}
