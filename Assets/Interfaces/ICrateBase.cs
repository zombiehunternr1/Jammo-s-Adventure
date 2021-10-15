using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface ICrateBase
{
    GameObject gameObject { get; }
    void Break(int Side);

    void DisableCrate();
}
