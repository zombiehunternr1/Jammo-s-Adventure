using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CompanionRobot : MonoBehaviour, ICollectable
{
    public AudioClip GotHitSFX;
    public AudioClip ChargingUpSFX;
    public AudioClip PoweringUpSFX;
    public AudioClip PoweringDownSFX;
    public AudioClip InvincibleSFX;
    public ParticleSystem Explosion;
    public float DistanceToTarget = 0.5f;
    public float HeightAboutTarget = 0;
    public float HeightDamping = 2;
    public float RotationDamping = 3.5f;
    public float Speed;
    [HideInInspector]
    public Material ShellColor;

    AudioSource AudioSource;
    Transform Target;
    bool HasCollided;
    float SpawnHeight = 1.2f;
    private Rigidbody RB;
    private Animator Anim;
    private List<ParticleSystem> Engines = new List<ParticleSystem>();
    float MovingDistanceToTarget;

    private void OnEnable()
    {
        AudioSource = GetComponent<AudioSource>();
        ShellColor = GetComponentInChildren<MeshRenderer>().materials[0];
        RB = GetComponent<Rigidbody>();
        Anim = GetComponent<Animator>();
        ParticleSystem[] FoundEngines = GetComponentsInChildren<ParticleSystem>();
        foreach (ParticleSystem Engine in FoundEngines)
        {
            Engines.Add(Engine);
            Engine.Stop();
        }
        if (transform.position.y > SpawnHeight)
        {
            StartCoroutine(MoveToGround());
        }
        else
        {
            Destroy(RB);
        }
    }
    IEnumerator MoveToPlayer()
    {
        while (transform.position != Target.position)
        {
            transform.position = Vector3.Lerp(transform.position, Target.position, Speed * Time.deltaTime);
            MovingDistanceToTarget = Vector3.Distance(Target.position, transform.position);
            if (MovingDistanceToTarget < DistanceToTarget)
            {
                StartCoroutine(FollowPlayer());
                StopCoroutine(MoveToPlayer());
                CheckColorStatus();
            }
            yield return transform.position;
        }
        yield return null;
    }

    IEnumerator FollowPlayer()
    {
        if (!Anim.GetCurrentAnimatorStateInfo(0).IsName("Explode") && !Anim.GetCurrentAnimatorStateInfo(0).IsName("Damage"))
        {
            Anim.Play("Follow");
        }
        while (true)
        {
            float WantedRotationAngle = Target.eulerAngles.y;
            float WantedHeight = Target.position.y + HeightAboutTarget;

            float CurrenRotationAngle = transform.eulerAngles.y;
            float CurrentHeight = transform.position.y;

            CurrenRotationAngle = Mathf.LerpAngle(CurrenRotationAngle, WantedRotationAngle, RotationDamping * Time.deltaTime);
            CurrentHeight = Mathf.Lerp(CurrentHeight, WantedHeight, HeightDamping * Time.deltaTime);

            Quaternion CurrentRotation = Quaternion.Euler(0, CurrenRotationAngle, 0);

            transform.position = Target.position;
            transform.position -= CurrentRotation * Vector3.forward * DistanceToTarget;
            transform.position = new Vector3(transform.position.x, CurrentHeight, transform.position.z);
            transform.LookAt(Target);
            yield return transform.position;
        }
    }

    private IEnumerator MoveToGround()
    {
        while (transform.position.y != 0.5f)
        {
            if (transform.position.y <= 0.5f)
            {
                transform.position = new Vector3(transform.position.x, 0.5f, transform.position.z);
                Destroy(RB);
            }
            yield return null;
        }
        StopCoroutine(MoveToGround());
    }

    private void PositionRobot()
    {
        Destroy(gameObject.GetComponent<BoxCollider>());
        transform.SetParent(GameManager.Instance.Player.CompanionPosition);
        Target = GameManager.Instance.Player.CompanionPosition;
        foreach (ParticleSystem Engine in Engines)
        {
            Engine.Play();
        }
        StartCoroutine(MoveToPlayer());
    }

    private void CheckColorStatus()
    {
        if(GameManager.Instance.GetComponent<EventListener>().PlayerInfo.ExtraHit == 1)
        {
            GameManager.Instance.Player.GetComponentInChildren<CompanionRobot>().ShellColor.color = Color.red;
        }
        if (GameManager.Instance.GetComponent<EventListener>().PlayerInfo.ExtraHit == 2)
        {
            GameManager.Instance.Player.GetComponentInChildren<CompanionRobot>().ShellColor.color = Color.yellow;
        }
        if (GameManager.Instance.GetComponent<EventListener>().PlayerInfo.ExtraHit == 3)
        {
            GameManager.Instance.Player.GetComponentInChildren<CompanionRobot>().ShellColor.color = Color.white;
        }
    }

    private void CheckHitStatus()
    {
        if (GameManager.Instance.GetComponent<EventListener>().PlayerInfo.ExtraHit == 0)
        {
            GameManager.Instance.GetComponent<EventListener>().PlayerInfo.ExtraHit++;
            Upgrade();
            PositionRobot();
        }
        else if (GameManager.Instance.GetComponent<EventListener>().PlayerInfo.ExtraHit != 3)
        {
            if (GameManager.Instance.Player.CompanionPosition.GetComponentInChildren<CompanionRobot>() == null)
            {
                PositionRobot();
                return;
            }
            GameManager.Instance.GetComponent<EventListener>().PlayerInfo.ExtraHit++;
            if (GameManager.Instance.GetComponent<EventListener>().PlayerInfo.ExtraHit == 3)
            {
                GameManager.Instance.GetComponent<EventListener>().StartCoroutine(GameManager.Instance.GetComponent<EventListener>().Invincible());
                GameManager.Instance.Player.CompanionPosition.GetComponentInChildren<CompanionRobot>().Invincible();
            }
            else
            {
                GameManager.Instance.Player.CompanionPosition.GetComponentInChildren<CompanionRobot>().Upgrade();
            }
            DestroyObject();
        } 
        else
        {
            DestroyObject();
        }
    }

    public void Collect()
    {
        if (!HasCollided)
        {
            HasCollided = true;
            CheckHitStatus();
            CheckColorStatus();
        }
    }

    public void DestroyObject()
    {
        Destroy(gameObject);
    }

    public void TookDamage()
    {
        AudioSource.Stop();
        AudioSource.clip = GotHitSFX;
        AudioSource.Play();
    }

    public void GotDestroyed()
    {
        GetComponentInChildren<MeshRenderer>().gameObject.SetActive(false);
        Instantiate(Explosion, transform.position, Quaternion.identity);
        DestroyObject();
    }

    public void ChargeUp()
    {
        AudioSource.Stop();
        AudioSource.clip = ChargingUpSFX;
        AudioSource.Play();
    }

    public void Upgrade()
    {
        AudioSource.Stop();
        AudioSource.clip = PoweringUpSFX;
        AudioSource.Play();
    }

    public void Downgrade()
    {
        AudioSource.Stop();
        AudioSource.clip = PoweringDownSFX;
        AudioSource.Play();
    }

    public void Invincible()
    {
        AudioSource.Stop();
        AudioSource.clip = InvincibleSFX;
        AudioSource.Play();
    }
}
