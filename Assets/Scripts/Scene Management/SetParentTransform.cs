using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SetParentTransform : MonoBehaviour
{
    public List<AudioClip> CrateBreakingSFX;

    Transform SpawnItemsHolder;
    AudioSource AudioSource;
    private void Awake()
    {
        AudioSource = GetComponent<AudioSource>();
        GameManager.Instance.UpdateItemContainerList(gameObject);
        int ChosenSFX = Random.Range(0, CrateBreakingSFX.Count);
        AudioSource.clip = CrateBreakingSFX[ChosenSFX];
        AudioSource.Play();
    }
}
