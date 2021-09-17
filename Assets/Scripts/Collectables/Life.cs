using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Life : MonoBehaviour, ICollectable
{
    public void Collect()
    {
        EventManager.CollectLife();
        DestroyObject();
    }

    public void DestroyObject()
    {
        Destroy(gameObject);
    }
}
