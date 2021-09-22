using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance;

    [HideInInspector]
    public GameObject BreakableCrateContainer;
    [HideInInspector]
    public GameObject SpawnedItemsContainer;
    private PlayerScript Player;

    private void OnEnable()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(this);
        }
        SetupSceneElements();
    }

    private void SetupSceneElements()
    {
        if(BreakableCrateContainer == null)
        {
            BreakableCrateContainer = GameObject.Find("Breakable Crates");
        }
        if (SpawnedItemsContainer == null)
        {
            SpawnedItemsContainer = GameObject.Find("SpawnedItems");
        }
        if(Player == null)
        {
            Player = GameObject.Find("Player").GetComponent<PlayerScript>();
        }
    }

    public void UpdateItemContainerList(GameObject Item)
    {
        SpawnedItemsContainer.GetComponent<ClearSpawnedItems>().Items.Add(Item);
    }

    public void ClearItemsContainer()
    {
        SpawnedItemsContainer.GetComponent<ClearSpawnedItems>().DestroyItems();
    }

    public void PlayerDied()
    {
        ClearItemsContainer();
    }
}
