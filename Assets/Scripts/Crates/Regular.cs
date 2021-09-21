using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Regular : MonoBehaviour, ICrateBase
{
    public GameObject BrokenCrate;

    public void Break(int Side)
    {
        Debug.Log(Side);
    }

    public void DisableCrate()
    {
        throw new System.NotImplementedException();
    }
}
