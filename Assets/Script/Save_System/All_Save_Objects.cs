using UnityEngine.Serialization;

[System.Serializable]
public class All_Save_Objects
{
    public Save_Object_Obstacle[] allSaveObjectsObstacle;
    public Save_Object_Player[] allSaveObjectsPlayer;

    public All_Save_Objects(Save_Object_Obstacle[] saveObjectsObstacle, Save_Object_Player[] saveObjectPlayers)
    {
        this.allSaveObjectsObstacle = saveObjectsObstacle;
        this.allSaveObjectsPlayer = saveObjectPlayers;
    }
}