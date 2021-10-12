using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerDied : MonoBehaviour
{
    public void KillPlayer(PlayerScript Player)
    {
        EventManager.EnablePlayerMovement();
        Player.GetComponent<Animator>().SetTrigger("IsDead");
    }
}
