using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class Penguin : MonoBehaviour, IEnemyBase
{
    public Transform[] Checkpoints;
    public bool Waiting;
    public float WaitTime;
    public float RotationSpeed;

    private float StoppingDistance = 0.1f;
    private int CheckpointIndex = 0;
    NavMeshAgent Agent;
    private Collider[] HitColliders;
    private bool HasBodySlammed;
    private Animator Anim;

    private void Start()
    {
        Agent = GetComponent<NavMeshAgent>();
        Agent.stoppingDistance = StoppingDistance;
        Anim = GetComponent<Animator>();
    }

    private void Update()
    {
        if(Checkpoint != null)
        {
            if (!Waiting)
            {
                Agent.SetDestination(Checkpoint.position);
                if (Agent.stoppingDistance >= Distance)
                {
                    StartCoroutine(SetNextCheckpoint());
                }
            }
            Rotate();
        }
    }

    private void GetNextCheckpoint()
    {
        CheckpointIndex++;
        if (CheckpointIndex >= Checkpoints.Length)
        {
            CheckpointIndex = 0;
        }
    }

    private void Rotate()
    {
        Vector3 Dir = Checkpoints[CheckpointIndex].position - transform.position;
        Quaternion LookRotation = Quaternion.LookRotation(Dir);
        Vector3 Rotation = Quaternion.Slerp(transform.rotation, LookRotation, Time.deltaTime * RotationSpeed).eulerAngles;
        transform.rotation = Quaternion.Euler(0, Rotation.y, 0);
    }

    private Transform Checkpoint 
    {
        get { return Checkpoints[CheckpointIndex]; }
    }

    private float Distance
    {
        get { return Vector3.Distance(Checkpoints[CheckpointIndex].position, transform.position); }
    }

    private IEnumerator SetNextCheckpoint()
    {
        Waiting = true;
        GetNextCheckpoint();
        yield return new WaitForSeconds(WaitTime);
        Waiting = false;
    }

    public void Collision(int Side)
    {
        if (Side == 1)
            Top();
        else if (Side == 0 || Side == 2)
            HurtPlayer();
        else if (Side <= 6 && Side >= 3)
            HurtPlayer();
        else if (Side == 7)
            DisableEnemy();
        else if (Side == 8)
            Top();
        else if (Side == 9)
            DisableEnemy();
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
                    DisableEnemy();
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
            DisableEnemy();
        }
    }

    public void HurtPlayer()
    {
        if(GameManager.Instance.GetComponent<EventListener>().PlayerInfo.ExtraHit != 0)
        {
            EventManager.PlayerGotHit();
            DisableEnemy();
        }
        else
        {
            GameManager.Booleans.CanMove = false;
            GameManager.Instance.Player.GetComponent<Rigidbody>().constraints = RigidbodyConstraints.FreezeAll;
            GameManager.Instance.Player.GetComponent<Animator>().Play("Dying");
        }
    }

    public void ResetEnemy()
    {
        Debug.Log("Reset");
    }

    public void DisableEnemy()
    {
        Anim.Play("Destroy");
    }
}
