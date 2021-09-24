using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SetParentTransform : MonoBehaviour
{
    Transform SpawnItemsHolder;
    private void Awake()
    {
        SpawnItemsHolder = GameManager.Instance.BreakableCrateContainer.transform;
        transform.SetParent(SpawnItemsHolder);
    }

}
