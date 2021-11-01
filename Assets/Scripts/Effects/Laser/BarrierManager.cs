using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BarrierManager : MonoBehaviour
{
    private List<VolumetricLines.VolumetricLineBehavior> LasterBeams;
    private AudioSource AudioSource;

    private void Awake()
    {
        AudioSource = GetComponent<AudioSource>();
        LasterBeams = new List<VolumetricLines.VolumetricLineBehavior>();
        VolumetricLines.VolumetricLineBehavior[] Beams = GetComponentsInChildren<VolumetricLines.VolumetricLineBehavior>();
        foreach (VolumetricLines.VolumetricLineBehavior Beam in Beams)
        {
            LasterBeams.Add(Beam);
        }
    }
    public void EnableLasers()
    {
        foreach (VolumetricLines.VolumetricLineBehavior Beam in LasterBeams)
        {
            Beam.EnableLaser();
        }
    }

    public void DisableLasers()
    {
        AudioSource.Play();
        foreach (VolumetricLines.VolumetricLineBehavior Beam in LasterBeams)
        {
            Beam.StartCoroutine(Beam.DisableLaser());
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.GetComponent<PlayerScript>())
        {
            if (GameManager.Instance.CheckCrateTotal())
            {
                DisableLasers();
            }
        }
    }

}
