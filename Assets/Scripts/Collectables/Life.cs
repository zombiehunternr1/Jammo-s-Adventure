using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Life : MonoBehaviour, ICollectable
{
    public PlayerInfo PlayerInfo;
    public void Collect()
    {
        PlayerInfo.Lives++;
        DestroyObject();
    }

    public void DestroyObject()
    {
        Destroy(gameObject);
    }
}
