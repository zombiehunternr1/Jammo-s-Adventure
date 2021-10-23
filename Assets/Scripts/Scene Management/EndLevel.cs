using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EndLevel : MonoBehaviour
{
    private void OnTriggerEnter(Collider other)
    {
        if (other.GetComponent<PlayerScript>())
        {
            PlayerScript Player = other.GetComponent<PlayerScript>();
            Player.CanMove = false;
            RotateToDirection(Player.MainCamera.transform.position, 100, Player.RB);
            Player.GetComponent<Rigidbody>().constraints = RigidbodyConstraints.FreezeAll;
            Player.GetComponent<Animator>().Play("Victory");
        }
    }

    public void RotateToDirection(Vector3 LookDirection, float TurnSpeed, Rigidbody RB)
    {
        Vector3 CharacterPosition = transform.position;
        CharacterPosition.y = 0;
        LookDirection.y = 0;
        Vector3 NewDirection = LookDirection - CharacterPosition;
        Quaternion Direction = Quaternion.LookRotation(NewDirection);
        Quaternion Slerp = Quaternion.Slerp(transform.rotation, Direction, TurnSpeed * Time.deltaTime);
        RB.MoveRotation(Slerp);
    }
}
