using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu]
public class PlayerData : ScriptableObject
{
    public float speedMovement;
    public float stopDistance;
    public Material selectCharacter;
    public Material unSelectCharacter;
}
