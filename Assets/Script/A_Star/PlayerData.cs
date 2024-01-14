using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//Class with setting for players.
[CreateAssetMenu]
public class PlayerData : ScriptableObject
{
    public float speedMovement;
    public float stopDistance;
    public float durability;
    public float rotationSpeed;
    public Material selectCharacter;
    public Material unSelectCharacter;
}
