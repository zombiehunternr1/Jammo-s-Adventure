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

    public static void CollectLifeDisplay()
    { if(OnCollectedLifeDisplay != null){ OnCollectedLifeDisplay(); } }

    public static void CollectBoltDisplay()
    { if (OnCollectedBoltDisplay != null) { OnCollectedBoltDisplay(); } }

    public static void CollectLifeUpdate()
    { if (OnCollectedLifeUpdate != null) { OnCollectedLifeUpdate(); } }

    public static void CollectBoltUpdate()
    { if (OnCollectedBoltUpdate != null) { OnCollectedBoltUpdate(); } }
}
