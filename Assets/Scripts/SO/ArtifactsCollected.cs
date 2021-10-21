using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Artifacts Collected", menuName = "Scriptable Objects/Artifact info/Collected")]
public class ArtifactsCollected : ScriptableObject
{
    public List<ArtifactInfo> Artifacts;
}
