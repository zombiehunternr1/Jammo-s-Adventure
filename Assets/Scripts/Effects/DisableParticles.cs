using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(ParticleSystem))]
public class DisableParticles : MonoBehaviour
{
    public bool OnlyDeactivate;
    void OnEnable()
    {
        StartCoroutine("CheckIfAlive");
    }

    IEnumerator CheckIfAlive()
    {
        while (true)
        {
            yield return new WaitForSeconds(0.5f);
            if (!GetComponent<ParticleSystem>().IsAlive(true))
            {
                if (OnlyDeactivate)
                {
                    transform.root.gameObject.SetActive(false);
                }
                else
                {
                    GameObject.Destroy(transform.root.gameObject);
                }
                break;
            }
        }
    }
}
