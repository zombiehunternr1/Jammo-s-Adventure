using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Regular : MonoBehaviour, ICrateBase
{
    public GameObject BrokenCrate;
    public int RandomDropRange;
    public int DropAmount;
    public List<GameObject> Bolts;

    private float YOffset = 0.5f;

    public void Break(int Side)
    {
        switch (Side)
        {
            case 1:
                SpawnBoltTypes();
            break;
        }
    }

    private void SpawnBoltTypes()
    {
        for(int i = 0; i < DropAmount; i++)
        {
            if(i > 0)
            {
                var OffsetX = Random.Range(-RandomDropRange, RandomDropRange);
                var OffsetZ = Random.Range(-RandomDropRange, RandomDropRange);
                int SelectedItemDrop = Random.Range(0, Bolts.Count);
                GameObject ItemDrop = Instantiate(Bolts[SelectedItemDrop], new Vector3(transform.position.x + OffsetX, transform.position.y + YOffset, transform.position.z + OffsetZ), Quaternion.identity);
            }
            else
            {
                int SelectedItemDrop = Random.Range(0, Bolts.Count);
                GameObject ItemDrop = Instantiate(Bolts[SelectedItemDrop], new Vector3(transform.position.x, transform.position.y + YOffset, transform.position.z), Quaternion.identity);
            }
        }
        DisableCrate();
    }

    public void DisableCrate()
    {
        gameObject.SetActive(false);
        Instantiate(BrokenCrate, transform.position, Quaternion.identity);
    }
}
