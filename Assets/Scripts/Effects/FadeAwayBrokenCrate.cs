using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FadeAwayBrokenCrate : MonoBehaviour
{
    public float Fadespeed;
    private Renderer CrateFragment;

    public void Awake()
    {
        CrateFragment = GetComponent<Renderer>();
        StartCoroutine(FadeOut());
    }

    IEnumerator FadeOut()
    {
        while (CrateFragment.material.color.a > 0)
        {
            Color FragmentColor = CrateFragment.material.color;
            float FadeAmount = FragmentColor.a - (Fadespeed * Time.deltaTime);
            FragmentColor = new Color(FragmentColor.r, FragmentColor.g, FragmentColor.b, FadeAmount);
            CrateFragment.material.color = FragmentColor;
            yield return null;
        }
        gameObject.SetActive(false);
        StopCoroutine(FadeOut());
    }
}
