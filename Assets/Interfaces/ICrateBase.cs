using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface ICrateBase
{
    GameObject gameObject { get; }
    
    Checkpoint Checkpoint { get; set; }

    void Break(int Side);

    void ResetCrate();
    void DisableCrate();
}
