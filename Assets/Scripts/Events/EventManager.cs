using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EventManager : MonoBehaviour
{
    public delegate void CollectedLife();
    public static event CollectedLife OnCollectedLife;

    public delegate void CollectedBolt();
    public static event CollectedBolt OnCollectedBolt;

    public static void CollectLife()
    { if(OnCollectedLife != null){ OnCollectedLife(); } }

    public static void CollectBolt()
    { if (OnCollectedBolt != null) { OnCollectedBolt(); } }
}
