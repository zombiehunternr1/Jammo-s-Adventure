using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NitroDetonator : MonoBehaviour, IInteractable
{
    [HideInInspector]
    public bool HasBounced;
    private bool HasDetonated;
    private Collider[] HitColliders;
    private bool HasBodySlammed;
    private List<Nitro> AllNitros;

    private void Start()
    {
        transform.parent = GameManager.Instance.AllCrateTypes.transform;
    }

    public void GetAllNitros()
    {
        AllNitros = new List<Nitro>();
        Nitro[] NitroCrates = GameManager.Instance.AllCrateTypes.GetComponentsInChildren<Nitro>();
        foreach (Nitro NitroCrate in NitroCrates)
        {
            AllNitros.Add(NitroCrate);
        }
    }

    public void Interact(int Side)
    {
        switch (Side)
        {
            //Top
            case 1:
                Top();
                break;
            //Attack
            case 7:
                Detonate();
                break;
            //Bodyslam
            case 8:
                Detonate();
                break;
            //Slide
            case 9:
                Detonate();
                break;
        }
    }
    public void DisableCrate()
    {
        Debug.Log(gameObject.GetComponent<Renderer>().name);
    }

    public void ResetCrate()
    {
        HasDetonated = false;
        //GetComponentInChildren<MeshRenderer>().enabled = false;
        GetAllNitros();
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
                    Detonate();
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
    private void Bounce(PlayerScript Player)
    {
        if (Player.Grounded && !HasBounced)
        {
            Player.IsBounce = true;
            Detonate();
        }
    }

    private void Detonate()
    {
        if (!HasDetonated)
        {
            HasDetonated = true;
            foreach(Nitro NitroCrate in AllNitros)
            {
                if (NitroCrate.gameObject.activeSelf)
                {
                    NitroCrate.Explosion();
                }
            }
            DisableCrate();
        }
    }
}
