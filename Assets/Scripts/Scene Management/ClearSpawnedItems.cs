using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ClearSpawnedItems : MonoBehaviour
{
    [HideInInspector]
    public List<GameObject> Items;
    public void DestroyItems()
    {
        if(Items != null)
        {
            for (int i = 0; i < Items.Count; i++)
            {
                Destroy(Items[i]);
                Items.Remove(Items[i]);
                i--;
            }
        }
    }
}
