using UnityEngine.Serialization;

[System.Serializable]
public class All_Save_Objects
{
    public Save_Object_Obstacle[] allSaveObjectsObstacle;

    public All_Save_Objects(Save_Object_Obstacle[] saveObjectsObstacle)
    {
        this.allSaveObjectsObstacle = saveObjectsObstacle;
    }
}