using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Explosion : MonoBehaviour
{
    public float ExplosionRadus = 2f;

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
                if (!Player.HasExploded)
                {
                    Player.HasExploded = true;
                    Player.RB.constraints = RigidbodyConstraints.FreezeAll;
                    Instantiate(Player.ExplosionModel, Player.transform.position, Player.transform.rotation);
                    EventManager.EnablePlayerMovement();
                    Player.PlayerModel.SetActive(false);
                    StartCoroutine(HoldTransition());
                }
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
        EventManager.PlayerDied();
    }
}
