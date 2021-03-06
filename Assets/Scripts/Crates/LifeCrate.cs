using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LifeCrate : MonoBehaviour, ICrateBase
{
    public GameObject Life;
    public GameObject BrokenCrate;

    private Checkpoint CheckPointCrate;
    private float YOffset = 0.5f;
    private Collider[] HitColliders;
    private bool IsBroken;
    private bool HasBodySlammed;

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
                Top();
                break;
            //Bottom
            case 2:
                Bottom();
                break;
            //Attack
            case 7:
                SpawnLife();
                break;
            //Bodyslam
            case 8:
                Top();
                break;
            //Slide
            case 9:
                SpawnLife();
                break;
            //Explosion
            case 10:
                DisableCrate();
                break;
        }
    }
    public void ResetCrate()
    {
        HasBodySlammed = false;
        IsBroken = false;
        gameObject.SetActive(true);
    }

    private void Top()
    {
        HitColliders = Physics.OverlapBox(new Vector3(transform.position.x, transform.position.y + 0.8f, transform.position.z), new Vector3(1.25f, 1, 1.25f));

        foreach (Collider Collider in HitColliders)
        {
            PlayerScript Player = Collider.GetComponent<PlayerScript>();
            if (Player != null)
            {
                if (Player.IsBodyslamPerforming && !HasBodySlammed)
                {
                    HasBodySlammed = true;
                    Player.StartCoroutine(Player.DownwardsForce());
                    SpawnLife();
                }
                else
                {
                    Bounce(Player);
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
                SpawnLife();
            }
        }
    }

    private void SpawnLife()
    {
        Instantiate(Life, new Vector3(transform.position.x, transform.position.y + YOffset, transform.position.z), Quaternion.identity);
        DisableCrate();
    }

    private void Bounce(PlayerScript Player)
    {
        if (Player.Grounded)
        {
            Player.IsBounce = true;
            SpawnLife();
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
