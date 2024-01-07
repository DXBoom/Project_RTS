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
        //Load Obstacles
        All_Save_Objects allSaveObjects = Save_Manager.Load();
        saveObjectObstacle = allSaveObjects.allSaveObjectsObstacle;
        LoadObstacles();
    }

    private void LoadObstacles()
    {
        if (saveObjectObstacle.Length != null && saveObjectObstacle.Length >= 0)
        {
            if (A_Manager.Instance.obstaclesList != null && A_Manager.Instance.obstaclesList.Count >= 0)
            {
                for (int i = 0; i < A_Manager.Instance.obstaclesList.Count; i++)
                {
                    Destroy(A_Manager.Instance.obstaclesList[i].gameObject);
                }
            }
            
            A_Manager.Instance.obstaclesList.Clear();

            for (int i = 0; i < saveObjectObstacle.Length; i++)
            {
                GameObject newObstacleCreated = Instantiate(A_Manager.Instance.newObstacle, new Vector3(saveObjectObstacle[i].posX, saveObjectObstacle[i].posY, saveObjectObstacle[i].posZ), Quaternion.identity);
                newObstacleCreated.transform.SetParent(A_Manager.Instance.obstacleParent.transform);
                A_Manager.Instance.obstaclesList.Add(newObstacleCreated);
            }
        }
        
        A_Manager.Instance.CalculateWorldMapBorders();
        A_Manager.Instance.GenerateGrid();
    }
}
