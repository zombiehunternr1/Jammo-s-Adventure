using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpawnStaticItem : MonoBehaviour
{
    public List<GameObject> Bolts;

    private List<Transform> SpawnBoltPosition = new List<Transform>();
    private float YOffset = 0.5f;

    private void Awake()
    {
        Transform[] FoundSpawns = GetComponentsInChildren<Transform>();
        foreach(Transform Spawn in FoundSpawns)
        {
            if (!Spawn.GetComponent<SpawnStaticItem>())
            {
                SpawnBoltPosition.Add(Spawn);
            }
        }
        SpawnBoltType();
    }

    public void CheckStaticStatus()
    {
        SpawnBoltType();
        DestroyCheckpointDrones();
    }

    private void SpawnBoltType()
    {
        foreach (Transform Spawn in SpawnBoltPosition)
        {
            if (Spawn.childCount == 0)
            {
                int SelectedItemDrop = Random.Range(0, Bolts.Count);
                GameObject BoltType = Instantiate(Bolts[SelectedItemDrop], new Vector3(Spawn.position.x, Spawn.position.y + YOffset, Spawn.position.z), Quaternion.identity);
                BoltType.GetComponent<Bolt>().IsStatic = true;
                BoltType.transform.parent = Spawn;
            }
        }
    }

    private void DestroyCheckpointDrones()
    {
        CheckpointDrone[] Drones = GetComponentsInChildren<CheckpointDrone>();
        foreach(CheckpointDrone Drone in Drones)
        {
            Destroy(Drone.gameObject);
        }
    }
    
}
