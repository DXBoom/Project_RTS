using System;
using UnityEngine;

public class Save_Load_Call : MonoBehaviour
{
    public static Save_Load_Call Instance;
    
    public Save_Object_Obstacle[] saveObjectObstacle;

    private void Awake()
    {
        if (Instance == null)
            Instance = this;
    }

    public void Save()
    {
        Save_Manager.Save(saveObjectObstacle);
    }

    public void Load()
    {
        All_Save_Objects allSaveObjects = Save_Manager.Load();
        saveObjectObstacle = allSaveObjects.allSaveObjectsObstacle;
    }
}
