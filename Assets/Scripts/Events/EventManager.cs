using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EventManager : MonoBehaviour
{
    public delegate void CollectedLifeDisplay();
    public static event CollectedLifeDisplay OnCollectedLifeDisplay;

    public delegate void CollectedBoltDisplay();
    public static event CollectedBoltDisplay OnCollectedBoltDisplay;

    public delegate void CollectedLifeUpdate();
    public static event CollectedLifeUpdate OnCollectedLifeUpdate;

    public delegate void CollectedBoltUpdate();
    public static event CollectedBoltUpdate OnCollectedBoltUpdate;

    public delegate void PlayerHasDied();
    public static event PlayerHasDied OnPlayerDied;

    public delegate void EnablePlayersMovement();
    public static event EnablePlayersMovement OnEnablePlayerMovement;

    public delegate void UpdateItemsInContainer();
    public static event UpdateItemsInContainer OnUpdateItemsInContainer;

    public delegate void ClearItemsInContainer();
    public static event ClearItemsInContainer OnClearItemsContainer;

    public delegate void ResetGameOverPlayer();
    public static event ResetGameOverPlayer OnResetGameOverPlayer;

    public static void CollectLifeDisplay()
    { if (OnCollectedLifeDisplay != null) { OnCollectedLifeDisplay(); } }

    public static void CollectBoltDisplay()
    { if (OnCollectedBoltDisplay != null) { OnCollectedBoltDisplay(); } }

    public static void CollectLifeUpdate()
    { if (OnCollectedLifeUpdate != null) { OnCollectedLifeUpdate(); } }

    public static void CollectBoltUpdate()
    { if (OnCollectedBoltUpdate != null) { OnCollectedBoltUpdate(); } }

    public static void PlayerDied()
    { if (OnPlayerDied != null) OnPlayerDied(); }

    public static void EnablePlayerMovement()
    { if (OnEnablePlayerMovement != null) OnEnablePlayerMovement(); }

    public static void UpdateItemContainerList()
    { if (OnClearItemsContainer != null) OnUpdateItemsInContainer(); }
    public static void ClearItemsContainer()
    { if (OnClearItemsContainer != null) OnClearItemsContainer(); } 

    public static void ResetGameOver()
    { if (OnResetGameOverPlayer != null) OnResetGameOverPlayer(); }
}
   
