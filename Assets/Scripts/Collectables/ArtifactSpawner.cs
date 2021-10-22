using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class ArtifactSpawner : MonoBehaviour
{
    public ArtifactsCollected ArtifactsCollected, TotalArtifacts;
    public int Level;

    public List<Transform> Shards;

    private void Awake()
    {
        Shards = new List<Transform>();
        Transform[] Children = GetComponentsInChildren<Transform>();
        for(int i = 0; i < Children.Length; i++)
        {
            if(i !> 1)
            {
                Shards.Add(Children[i]);
            }
        }
        CheckLevel();
    }

    private void CheckLevel()
    {
        for(int i = 0; i < ArtifactsCollected.Artifacts.Count; i++)
        {
            if (Level == ArtifactsCollected.Artifacts[i].Level)
            {
                Disable();
                return;
            }
        }
        SetupArtifact();
    }

    private void Disable()
    {
        gameObject.SetActive(false);
    }

    private void SetupArtifact()
    {
        foreach (Transform Shard in Shards)
        {
            Shard.GetComponent<Renderer>().enabled = false;
        }

        for (int i = 0; i < TotalArtifacts.Artifacts.Count; i++)
        {
            if (TotalArtifacts.Artifacts[i].Level == Level)
            {
                Shards[Convert.ToInt32(TotalArtifacts.Artifacts[i].ActiveShardSelected)].GetComponent<Renderer>().enabled = true;
                Shards[Convert.ToInt32(TotalArtifacts.Artifacts[i].ActiveShardSelected)].transform.position = new Vector3(transform.position.x, 1, transform.position.z);
                return;
            }
        }

        /*Use when wanna enable multiple at once
        for(int i = 0; i < TotalArtifacts.Artifacts.Count; i++)
        {
            if(TotalArtifacts.Artifacts[i].Level == Level)
            {
                Shards[Convert.ToInt32(TotalArtifacts.Artifacts[i].ActiveShardSelected)].GetComponent<MeshRenderer>().enabled = true;
            }
        }
        */
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.GetComponent<PlayerScript>())
        {
            for (int i = 0; i < TotalArtifacts.Artifacts.Count; i++)
            {
                if (Level == TotalArtifacts.Artifacts[i].Level)
                {
                    ArtifactsCollected.Artifacts.Add(TotalArtifacts.Artifacts[i]);
                    EventManager.CollectShardUpdate();
                    gameObject.SetActive(false);
                }
            }
        }
    }
}
