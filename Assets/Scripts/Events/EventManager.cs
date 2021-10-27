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

    public delegate void CollectedShardUpdate();
    public static event CollectedShardUpdate OnCollectedShardUpdate;

    public delegate void PlayerHit();
    public static event PlayerHit OnPlayerHit;

    public delegate void EnablePlayersMovement();
    public static event EnablePlayersMovement OnEnablePlayerMovement;

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

    public static void CollectShardUpdate()
    { if(OnCollectedShardUpdate != null) { OnCollectedShardUpdate(); } }

    public static void PlayerGotHit()
    { if (OnPlayerHit != null) OnPlayerHit(); }

    public static void EnablePlayerMovement()
    { if (OnEnablePlayerMovement != null) OnEnablePlayerMovement(); }

    public static void ClearItemsContainer()
    { if (OnClearItemsContainer != null) OnClearItemsContainer(); } 

    public static void ResetGameOver()
    { if (OnResetGameOverPlayer != null) OnResetGameOverPlayer(); }
}
   
