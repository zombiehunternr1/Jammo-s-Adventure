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
                PlayerScript Player = Hit.gameObject.GetComponent<PlayerScript>();
                EventManager.EnablePlayerMovement();
                Player.GetComponent<Animator>().SetTrigger("IsDead");
            }
        }
    }

    IEnumerator DisableCollider()
    {
        yield return new WaitForSeconds(.5f);
        HitBox.enabled = false;
    }
}
