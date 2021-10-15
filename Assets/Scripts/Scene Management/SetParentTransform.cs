using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SetParentTransform : MonoBehaviour
{
    Transform SpawnItemsHolder;
    private void Awake()
    {
        GameManager.Instance.UpdateItemContainerList(gameObject);
    }

}
