using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Bolt : MonoBehaviour, ICollectable
{
    Transform SpawnItemsHolder;
    Transform BoltUI;
    float Speed = 2;
    float DistanceToTarget;
    private bool HasCollided;

    private void Awake()
    {
        BoltUI = (Transform)GameObject.Find("BoltModelPosition").gameObject.GetComponent(typeof(Transform));
        SpawnItemsHolder = (Transform)GameObject.Find("SpawnedItems").gameObject.GetComponent(typeof(Transform));
        transform.SetParent(SpawnItemsHolder);
    }

    public void Collect()
    {
        if (!HasCollided)
        {
            HasCollided = true;
            gameObject.GetComponent<Animator>().SetTrigger("Disable");
            EventManager.CollectBoltDisplay();
            DestroyObject();
        }
    }

    public void DestroyObject()
    {
        StartCoroutine(MoveToUI());
    }
    IEnumerator MoveToUI()
    {
        while (transform.position != BoltUI.transform.position)
        {
            transform.position = Vector3.Lerp(transform.position, BoltUI.position, Speed * Time.deltaTime);
            DistanceToTarget = Vector3.Distance(BoltUI.position, transform.position);
            if (DistanceToTarget < 0.5)
            {
                EventManager.CollectBoltUpdate();
                Destroy(gameObject);
            }
            yield return transform.position;
        }
        yield return null;
    }
}
