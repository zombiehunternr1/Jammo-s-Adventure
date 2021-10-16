using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TNT : MonoBehaviour, ICrateBase
{
    public GameObject BrokenCrate;
    public ParticleSystem ExplosionEffect;
    public Checkpoint CheckPointCrate;

    private bool HasExploded;
    private Animator TNTAnim;
    private Collider[] HitColliders;
    private bool HasBounced;

    public Checkpoint Checkpoint { get => CheckPointCrate; set => CheckPointCrate = value; }

    private void Start()
    {
        transform.parent = GameManager.Instance.BreakableCrateContainer.transform;
        TNTAnim = GetComponent<Animator>();
    }

    public void Break(int Side)
    {
        switch (Side)
        {
            //Top
            case 1:
                Top();
                break;
            //Bottom
            case 2:
                Bottom();
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

    private void Top()
    {
        HitColliders = Physics.OverlapBox(new Vector3(transform.position.x, transform.position.y + 0.8f, transform.position.z), new Vector3(1.25f, 1, 1.25f));

        foreach (Collider Collider in HitColliders)
        {
            PlayerScript Player = Collider.GetComponent<PlayerScript>();
            if (Player != null)
            {
                if (Player.IsBodyslamPerforming)
                {
                    Player.StartCoroutine(Player.DownwardsForce());
                    Explosion();
                    break;
                }
                else
                {
                    Bounce(Player);
                    break;
                }
            }
        }
    }

    private void Bottom()
    {
        HitColliders = Physics.OverlapBox(new Vector3(transform.position.x, transform.position.y + 0.8f, transform.position.z), new Vector3(1.25f, 1, 1.25f));

        foreach (Collider Collider in HitColliders)
        {
            PlayerScript Player = Collider.GetComponent<PlayerScript>();
            if (Player != null)
            {
                Player.StartCoroutine(Player.DownwardsForce());
                Countdown();
                break;
            }
        }
    }

    private void Bounce(PlayerScript Player)
    {
        if (Player.IsFalling && !HasBounced)
        {
            HasBounced = true;
            Player.IsBounce = true;
            Countdown();
        }
    }

    public void ResetCrate()
    {
        HasBounced = false;
        HasExploded = false;
        gameObject.SetActive(true);
        TNTAnim.ResetTrigger("Countdown");
    }

    private void Countdown()
    {
        TNTAnim.SetTrigger("Countdown");
    }

    private void Explosion()
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
        GameManager.Instance.UpdateCrateCount(this);
        gameObject.SetActive(false);
        Instantiate(BrokenCrate, transform.position, Quaternion.identity);
    }
}
