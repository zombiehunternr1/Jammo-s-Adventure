using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Companion : MonoBehaviour
{
    public Transform Target;
    public float DistanceToTarget = 0.5f;
    public float HeightAboutTarget = 0;
    public float HeightDamping = 2;
    public float RotationDamping = 3.5f;
    public float Speed;

    private List<ParticleSystem> Engines = new List<ParticleSystem>();
    float MovingDistanceToTarget;

    private void OnEnable()
    {
        ParticleSystem[] FoundEngines = GetComponentsInChildren<ParticleSystem>();
        foreach(ParticleSystem Engine in FoundEngines)
        {
            Engines.Add(Engine);
            Engine.Stop();
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.GetComponent<PlayerScript>())
        {
            PlayerScript Player = other.GetComponent<PlayerScript>();

            if(GameManager.Instance.GetComponent<EventListener>().PlayerInfo.ExtraHit == 0)
            {
                Destroy(gameObject.GetComponent<BoxCollider>());
                transform.SetParent(Player.CompanionPosition);
                Target = Player.CompanionPosition;
                GameManager.Instance.GetComponent<EventListener>().PlayerInfo.ExtraHit++;
                foreach(ParticleSystem Engine in Engines)
                {
                    Engine.Play();
                }
                StartCoroutine(MoveToPlayer());
            }
            else if(GameManager.Instance.GetComponent<EventListener>().PlayerInfo.ExtraHit != 3)
            {
                GameManager.Instance.GetComponent<EventListener>().PlayerInfo.ExtraHit++;
                Destroy(gameObject);
            }
            else
            {
                Destroy(gameObject);
            }
        }
    }

    IEnumerator MoveToPlayer()
    {
        while(transform.position != Target.position)
        {
            transform.position = Vector3.Lerp(transform.position, Target.position, Speed * Time.deltaTime);
            MovingDistanceToTarget = Vector3.Distance(Target.position, transform.position);
            if(MovingDistanceToTarget < DistanceToTarget)
            {
                StartCoroutine(FollowPlayer());
                StopCoroutine(MoveToPlayer());
            }
            yield return transform.position;
        }
        yield return null;
    }

    IEnumerator FollowPlayer()
    {
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
}
