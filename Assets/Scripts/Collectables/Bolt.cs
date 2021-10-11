using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Bolt : MonoBehaviour, ICollectable
{
    Transform SpawnItemsHolder;
    Transform BoltUI;
    Rigidbody RB;
    float Speed = 2;
    float DistanceToTarget;
    bool HasCollided;

    private void Start()
    {
        Physics.IgnoreLayerCollision(7, 7);
        RB = GetComponent<Rigidbody>();
        BoltUI = (Transform)GameObject.Find("BoltModelPosition").gameObject.GetComponent(typeof(Transform));
        SpawnItemsHolder = GameManager.Instance.SpawnedItemsContainer.transform;
        transform.SetParent(SpawnItemsHolder);
        GameManager.Instance.UpdateItemContainerList(gameObject);
        StartCoroutine(MoveToGround());
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

    private IEnumerator MoveToGround()
    {
        while(transform.position.y != 0.5f)
        {
            if (transform.position.y <= 0.5f)
            {
                transform.position = new Vector3(transform.position.x, 0.5f, transform.position.z);
                Destroy(RB);
            }
            yield return null;
        }
        StopCoroutine(MoveToGround());
    }
}
