using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface IInteractable
{
    GameObject gameObject { get; }

    void Interact(int Side);

    void ResetCrate();

    void DisableCrate();
}
