using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Life : MonoBehaviour, ICollectable
{
    Transform SpawnItemsHolder;
    Transform LifeUI;
    float Speed = 2;
    float DistanceToTarget;

    private void Start()
    {
        LifeUI = (Transform)GameObject.Find("LifeModelPosition").gameObject.GetComponent(typeof(Transform));
        SpawnItemsHolder = GameManager.Instance.SpawnedItemsContainer.transform;
        transform.SetParent(SpawnItemsHolder);
        GameManager.Instance.UpdateItemContainerList(gameObject);
    }

    public void Collect()
    {
        GoToHover();
        EventManager.CollectLifeDisplay();
        DestroyObject();
    }

    public void GoToHover()
    {
        gameObject.GetComponent<Animator>().Play("Hovering");
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
