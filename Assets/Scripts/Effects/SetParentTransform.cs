using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SetParentTransform : MonoBehaviour
{
    Transform SpawnItemsHolder;

    private void Awake()
    {
        SpawnItemsHolder = (Transform)GameObject.Find("SpawnedItems").gameObject.GetComponent(typeof(Transform));
        transform.SetParent(SpawnItemsHolder);
    }

}
