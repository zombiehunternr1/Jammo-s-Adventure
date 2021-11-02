using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Activator : MonoBehaviour, IInteractable
{
    public float ActivationSpeed;
    public GameObject GhostCrate;
    public AudioClip ActivatorCrateActivatedSFX;
    public AudioClip ActivateGhostCratesSFX;
    public List<GameObject> Crates;

    [HideInInspector]
    public bool HasBounced;
    private bool HasActivated;
    private Collider[] HitColliders;
    private bool HasBodySlammed;
    private List<GameObject> TempList;
    private AudioSource AudioSource;

    private void Start()
    {
        transform.parent = GameManager.Instance.AllCrateTypes.transform;
        AudioSource = GetComponent<AudioSource>();
    }

    public void DisableCrate()
    {
        throw new System.NotImplementedException();
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
                StartCoroutine(ActivateCrates());
                break;
            //Bodyslam
            case 8:
                StartCoroutine(ActivateCrates());
                break;
            //Slide
            case 9:
                StartCoroutine(ActivateCrates());
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
                if (Player.IsBodyslamPerforming && !HasBodySlammed)
                {
                    HasBodySlammed = true;
                    Player.StartCoroutine(Player.DownwardsForce());
                    StartCoroutine(ActivateCrates());
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
            StartCoroutine(ActivateCrates());
        }
    }

    public void ResetCrate()
    {
        DisableCrates();
    }

    private void DisableCrates()
    {
        if(Crates != null || Crates.Count != 0)
        {
            TempList = new List<GameObject>();
            HasActivated = false;
            GetComponent<Renderer>().enabled = true;
            foreach (GameObject Crate in Crates)
            {
                int LastChild = Crate.transform.childCount - 1;
                if (!Crate.transform.GetChild(LastChild).gameObject.CompareTag("GhostCrate"))
                {
                    GameObject Ghost = Instantiate(GhostCrate, transform.position, transform.rotation);
                    Ghost.transform.parent = Crate.transform;
                    Ghost.transform.position = new Vector3(Crate.transform.position.x, 0.5f, Crate.transform.position.z);
                    Crate.GetComponent<BoxCollider>().enabled = false;
                    Crate.transform.GetChild(LastChild).GetComponent<Renderer>().enabled = false;
                    TempList.Add(Crate);
                }
                else
                {
                    if (Crate.activeSelf)
                    {
                        Crate.GetComponentInChildren<Renderer>().enabled = false;
                        Crate.GetComponent<BoxCollider>().enabled = false;
                        Crate.transform.GetChild(LastChild).GetComponent<Renderer>().enabled = true;
                        TempList.Add(Crate);
                    }
                }
            }
            Crates = TempList;
            if(Crates == null || Crates.Count == 0)
            {
                GetComponent<Renderer>().enabled = false;
            }
        }
    }

    private IEnumerator ActivateCrates()
    {
        if (!HasActivated)
        {
            AudioSource.clip = ActivatorCrateActivatedSFX;
            AudioSource.Play();
            GetComponent<Renderer>().enabled = false;
            HasActivated = true;
            yield return new WaitForSeconds(0.7f);
            AudioSource.clip = ActivateGhostCratesSFX;
            foreach (GameObject Crate in Crates)
            {
                int LastChild = Crate.transform.childCount - 1;
                if (Crate.transform.GetChild(LastChild).gameObject.CompareTag("GhostCrate"))
                {
                    Crate.transform.GetChild(LastChild).GetComponent<Renderer>().enabled = false;
                    Crate.GetComponent<BoxCollider>().enabled = true;
                    Crate.GetComponentInChildren<Renderer>().enabled = true;
                    AudioSource.Play();
                    yield return new WaitForSeconds(ActivationSpeed);
                }
            }
        }
        yield return null;
    }
}
