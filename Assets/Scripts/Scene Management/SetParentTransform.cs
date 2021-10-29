using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SetParentTransform : MonoBehaviour
{
    public List<AudioClip> CrateBreaking;

    Transform SpawnItemsHolder;
    AudioSource AudioSource;
    private void Awake()
    {
        AudioSource = GetComponent<AudioSource>();
        GameManager.Instance.UpdateItemContainerList(gameObject);
        int ChosenSFX = Random.Range(0, CrateBreaking.Count);
        AudioSource.clip = CrateBreaking[ChosenSFX];
        AudioSource.Play();
    }
}
