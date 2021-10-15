using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TNT : MonoBehaviour, ICrateBase
{
    public GameObject BrokenCrate;
    public ParticleSystem ExplosionEffect;

    private bool HasExploded;
    private Animation CountdownAnim;

    private void Start()
    {
        transform.parent = GameManager.Instance.BreakableCrateContainer.transform;
        CountdownAnim = GetComponent<Animation>();
    }

    public void Break(int Side)
    {
        switch (Side)
        {
            //Top
            case 1:
                Countdown();
                break;
            //Bottom
            case 2:
                Countdown();
                break;
            //Attack
            case 7:
                Explosion();
                break;
            //Bodyslam
            case 8:
                Explosion();
                break;
            //Slide
            case 9:
                Explosion();
                break;
            //Explosion
            case 10:
                Explosion();
                break;
        }
    }
    public void ResetCrate()
    {
        HasExploded = false;
        gameObject.SetActive(true);
    }

    private void Countdown()
    {
        CountdownAnim.Play();
    }

    private void Explosion()
    {
        if (!HasExploded)
        {
            HasExploded = true;
            CountdownAnim.Stop();
            Instantiate(ExplosionEffect, transform.position, Quaternion.identity);
            DisableCrate();
        }
    }

    public void DisableCrate()
    {
        GameManager.Instance.UpdateCrateCount(this);
        gameObject.SetActive(false);
        Instantiate(BrokenCrate, transform.position, Quaternion.identity);
    }
}
