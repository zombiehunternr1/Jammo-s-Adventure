using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AddToSpawnedItems : MonoBehaviour
{
    private void Awake()
    {
        GameManager.Instance.UpdateItemContainerList(gameObject);
    }
}
