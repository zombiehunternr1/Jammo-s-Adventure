using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Explosion : MonoBehaviour
{
    public float ExplosionRadus;

    private SphereCollider HitBox;
    private int Side = 10;

    private void Awake()
    {
        HitBox = GetComponent<SphereCollider>();
        ExplosionRadiusHit();
        StartCoroutine(DisableCollider());
    }

    private void ExplosionRadiusHit()
    {
        Collider[] HitColliders = Physics.OverlapSphere(HitBox.bounds.center, ExplosionRadus);
        foreach(Collider Hit in HitColliders)
        {
            ICrateBase Crate = (ICrateBase)Hit.gameObject.GetComponent(typeof(ICrateBase));
            if(Crate != null)
            {
                Crate.Break(Side);
            }
            if (Hit.gameObject.GetComponent<PlayerScript>())
            {
                PlayerScript Player = Hit.GetComponent<PlayerScript>();
                if (!Player.HasExploded && !GameManager.Booleans.Invulnerable)
                {
                    if(GameManager.Instance.GetComponent<EventListener>().PlayerInfo.ExtraHit <= 0)
                    {
                        Player.HasExploded = true;
                        GameManager.Booleans.Invulnerable = true;
                        Player.RB.constraints = RigidbodyConstraints.FreezeAll;
                        Instantiate(Player.ExplosionModel, Player.transform.position, Player.transform.rotation);
                        EventManager.EnablePlayerMovement();
                        Player.PlayerModel.SetActive(false);
                        StartCoroutine(HoldTransition());
                    }
                    else
                    {
                        EventManager.PlayerGotHit();
                    }
                }
            }
            IEnemyBase Enemy = (IEnemyBase)Hit.gameObject.GetComponent(typeof(IEnemyBase));
            if(Enemy != null)
            {
                Enemy.Collision(Side);
            }
        }
    }

    IEnumerator DisableCollider()
    {
        yield return new WaitForSeconds(.5f);
        HitBox.enabled = false;
    }

    IEnumerator HoldTransition()
    {
        GameManager.Booleans.CameraMove = false;
        yield return new WaitForSeconds(3f);
        EventManager.PlayerGotHit();
    }
}
