using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerDied : MonoBehaviour
{
    public void KillPlayer(PlayerScript Player)
    {
        Player.GetComponent<Animator>().SetTrigger("IsDead");
        Player.CanMove = false;
        EventManager.PlayerDied();
    }
}
