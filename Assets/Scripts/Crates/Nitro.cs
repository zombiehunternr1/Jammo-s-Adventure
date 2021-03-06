using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Nitro : MonoBehaviour, ICrateBase
{
    public GameObject BrokenCrate;
    public ParticleSystem ExplosionEffect;
    
    private Checkpoint CheckPointCrate;
    private bool HasExploded;
    private bool IsBroken;

    public Checkpoint Checkpoint { get => CheckPointCrate; set => CheckPointCrate = value; }

    private void Start()
    {
        transform.parent = GameManager.Instance.AllCrateTypes.transform;
    }

    public void Break(int Side)
    {
        switch (Side)
        {
            //Top
            case 1:
                Explosion();
                break;
            //Bottom
            case 2:
                Explosion();
                break;
            //Forward
            case 3:
                Explosion();
                break;
            //Back
            case 4:
                Explosion();
                break;
            //Left
            case 5:
                Explosion();
                break;
            //Right
            case 6:
                Explosion();
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
        IsBroken = false;
        gameObject.SetActive(true);
    }

    public void Explosion()
    {
        if (!HasExploded)
        {
            HasExploded = true;
            Instantiate(ExplosionEffect, transform.position, Quaternion.identity);
            DisableCrate();
        }
    }

    public void DisableCrate()
    {
        if (!IsBroken)
        {
            IsBroken = true;
            GameManager.Instance.UpdateCrateCount(this);
            gameObject.SetActive(false);
            Instantiate(BrokenCrate, transform.position, Quaternion.identity);
        }
    }
}
