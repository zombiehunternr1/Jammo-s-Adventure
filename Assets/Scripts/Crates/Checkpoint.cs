using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Checkpoint : MonoBehaviour, ICrateBase
{
    public GameObject BrokenCrate;
    public enum CheckPointCrateTypes { Breakable, Interactable }
    public CheckPointCrateTypes CheckPointCrateType;

    private Collider[] HitColliders;

    Checkpoint ICrateBase.Checkpoint { get => this; set => value = this; }

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
            //Attack
            case 7:
                SetCheckPoint();
                break;
            //Bodyslam
            case 8:
                Top();
                break;
            //Slide
            case 9:
                SetCheckPoint();
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
                    SetCheckPoint();
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
        if (Player.Grounded)
        {
            Player.IsBounce = true;
            SetCheckPoint();
        }
    }

    private void SetCheckPoint()
    {
        if(CheckPointCrateType == Checkpoint.CheckPointCrateTypes.Breakable)
        {
            GameManager.Instance.UpdateCrateCount(this);
        }
        GameManager.Instance.SetCheckpoint(this);
        DisableCrate();
    }

    public void DisableCrate()
    {
        Instantiate(BrokenCrate, transform.position, Quaternion.identity);
        gameObject.SetActive(false);
    }

    public void ResetCrate()
    {
        gameObject.SetActive(true);
    }
}
