using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Regular : MonoBehaviour, ICrateBase
{
    public GameObject BrokenCrate;
    public int RandomDropRange;
    public int DropAmount;
    public List<GameObject> Bolts;

    private float YOffset = 0.5f;
    private Collider[] HitColliders;

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
                SpawnBoltTypes();
            }
        }
    }

    private void SpawnBoltTypes()
    {
        for(int i = 0; i < DropAmount; i++)
        {
            if(i > 0)
            {
                var OffsetX = Random.Range(-RandomDropRange, RandomDropRange);
                var OffsetZ = Random.Range(-RandomDropRange, RandomDropRange);
                int SelectedItemDrop = Random.Range(0, Bolts.Count);
                GameObject ItemDrop = Instantiate(Bolts[SelectedItemDrop], new Vector3(transform.position.x + OffsetX, transform.position.y + YOffset, transform.position.z + OffsetZ), Quaternion.identity);
            }
            else
            {
                int SelectedItemDrop = Random.Range(0, Bolts.Count);
                GameObject ItemDrop = Instantiate(Bolts[SelectedItemDrop], new Vector3(transform.position.x, transform.position.y + YOffset, transform.position.z), Quaternion.identity);
            }
        }
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
        gameObject.SetActive(false);
        Instantiate(BrokenCrate, transform.position, Quaternion.identity);
    }
}
