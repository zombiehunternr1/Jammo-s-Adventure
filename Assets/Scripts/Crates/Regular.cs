using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Regular : MonoBehaviour, ICrateBase
{
    public GameObject BrokenCrate;
    public List<GameObject> Bolts;

    private Checkpoint CheckPointCrate;
    private int RandomDropRange = 1;
    private int DropAmount = 1;
    private float YOffset = 0.5f;
    private Collider[] HitColliders;
    private bool IsBroken;

    public Checkpoint Checkpoint { get => CheckPointCrate; set => CheckPointCrate = value; }

    private void Start()
    {
        transform.parent = GameManager.Instance.BreakableCrateContainer.transform;
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
                SpawnBoltTypes();
                break;
            //Bodyslam
            case 8:
                Top();
                break;
            //Slide
            case 9:
                SpawnBoltTypes();
                break;
            //Explosion
            case 10:
                DisableCrate();
                break;
        }
    }
    public void ResetCrate()
    {
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
                if (Player.IsBodyslamPerforming)
                {
                    Player.StartCoroutine(Player.DownwardsForce());
                    SpawnBoltTypes();
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
                SpawnBoltTypes();
                break;
            }
        }
    }

    private void SpawnBoltTypes()
    {
        int SelectedItemDrop = Random.Range(0, Bolts.Count);
        Instantiate(Bolts[SelectedItemDrop], new Vector3(transform.position.x, transform.position.y + YOffset, transform.position.z), Quaternion.identity);
        DisableCrate();
    }

    private void Bounce(PlayerScript Player)
    {
        if (Player.IsFalling)
        {
            Player.IsBounce = true;
            SpawnBoltTypes();
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
