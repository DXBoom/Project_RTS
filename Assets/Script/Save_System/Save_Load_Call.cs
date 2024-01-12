using System;
using UnityEngine;
using System.Threading.Tasks;

public class Save_Load_Call : MonoBehaviour
{
    public static Save_Load_Call Instance;

    public All_Save_Objects allSaveObjects;
    public Save_Object_Obstacle[] saveObjectObstacle;
    public Save_Object_Player[] saveObjectPlayer;
    public bool loading;

    private void Awake()
    {
        if (Instance == null)
            Instance = this;
    }

    public void Save()
    {
        SavePlayer();
        Save_Manager.Save(saveObjectObstacle, saveObjectPlayer);
    }

    public void SavePlayer()
    {
        Array.Clear(saveObjectPlayer, 0, 0);
        saveObjectPlayer = new Save_Object_Player[A_Manager.Instance.playersOnMap.Count];

        for (int i = 0; i < saveObjectPlayer.Length; i++)
        {
            saveObjectPlayer[i] = new Save_Object_Player();
        }

        for (int i = 0; i < saveObjectPlayer.Length; i++)
        {
            saveObjectPlayer[i].posX = A_Manager.Instance.playersOnMap[i].gameObject.transform.position.x;
            saveObjectPlayer[i].posY = A_Manager.Instance.playersOnMap[i].gameObject.transform.position.y;
            saveObjectPlayer[i].posZ = A_Manager.Instance.playersOnMap[i].gameObject.transform.position.z;
            saveObjectPlayer[i].isNowPlayer = A_Manager.Instance.playersOnMap[i].gameObject.GetComponent<A_PlayerMovement>().isNowPlayer;
        }
    }

    public async void Load()
    {
        loading = true;
        await Save_Manager.Load();
        saveObjectObstacle = allSaveObjects.allSaveObjectsObstacle;
        LoadObstacles();
        LoadPlayer();
    }

    private void LoadPlayer()
    {
        for (int i = 0; i < A_Manager.Instance.playersOnMap.Count; i++)
        {
            A_Manager.Instance.playersOnMap[i].gameObject.transform.position = new Vector3(saveObjectPlayer[i].posX,
                saveObjectPlayer[i].posY, saveObjectPlayer[i].posZ);
            A_Manager.Instance.playersOnMap[i].gameObject.GetComponent<A_PlayerMovement>().isNowPlayer =
                saveObjectPlayer[i].isNowPlayer;
            
            A_Manager.Instance.mainPlayerMove = false;
            A_Manager.Instance.playersOnMap[i].gameObject.GetComponent<A_PlayerMovement>()._targetPath = null;
            A_Manager.Instance.playersOnMap[i].gameObject.GetComponent<A_PlayerMovement>()._indexPath++;
            
            if (saveObjectPlayer[i].isNowPlayer)
            {
                A_Manager.Instance.playersOnMap[i].gameObject.GetComponent<MeshRenderer>().material =
                    A_Manager.Instance.playersOnMap[i].gameObject.GetComponent<A_PlayerMovement>().playerData.selectCharacter;
            }

            else
            {
                A_Manager.Instance.playersOnMap[i].gameObject.GetComponent<MeshRenderer>().material =
                    A_Manager.Instance.playersOnMap[i].gameObject.GetComponent<A_PlayerMovement>().playerData.unSelectCharacter;
            }
        }

        loading = false;
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
