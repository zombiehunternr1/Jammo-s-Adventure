using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BarrierManager : MonoBehaviour
{
    private List<VolumetricLines.VolumetricLineBehavior> LasterBeams;
    private void Awake()
    {
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
        foreach (VolumetricLines.VolumetricLineBehavior Beam in LasterBeams)
        {
            Beam.StartCoroutine(Beam.DisableLaser());
        }
    }
}
