using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Companion : MonoBehaviour, ICrateBase
{
    public GameObject BrokenCrate;
    public GameObject CompanionRobot;

    private float YOffset = 0.5f;
    private Checkpoint CheckPointCrate;
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
                SpawnCompanion();
                break;
            //Bodyslam
            case 8:
                Top();
                break;
            //Slide
            case 9:
                SpawnCompanion();
                break;
            //Explosion
            case 10:
                DisableCrate();
                break;
        }
    }

    private void SpawnCompanion()
    {
        Instantiate(CompanionRobot, new Vector3(transform.position.x, transform.position.y + YOffset, transform.position.z), Quaternion.identity);
        DisableCrate();
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
                    SpawnCompanion();
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
                SpawnCompanion();
                break;
            }
        }
    }

    private void Bounce(PlayerScript Player)
    {
        if (Player.Grounded)
        {
            Player.IsBounce = true;
            SpawnCompanion();
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

    public void ResetCrate()
    {
        HasBodySlammed = false;
        IsBroken = false;
        gameObject.SetActive(true);
    }
}
