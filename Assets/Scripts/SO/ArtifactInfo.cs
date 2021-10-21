using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Artifactinfo", menuName = "Scriptable Objects/Artifact info/Artifactinfo")]
public class ArtifactInfo : ScriptableObject
{
    public int Level;
    public enum ActiveShard { One = 0, Two = 1, Three = 2, Four = 3, Five = 4 };
    public ActiveShard ActiveShardSelected;
    [ColorUsage(true, true)]
    public Color ArtifactColor;
}
