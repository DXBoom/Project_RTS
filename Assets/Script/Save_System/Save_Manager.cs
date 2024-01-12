using UnityEngine;
using System.IO;
using System.Runtime.Serialization;
using System.Runtime.Serialization.Formatters.Binary;
using System.Threading.Tasks;

public static class Save_Manager
{
    public static string directory = "SaveData";
    public static string filename = "save.sav";

    public static async void Save(Save_Object_Obstacle[] sObject, Save_Object_Player[] sObjectPlayer)
    {
        if (!DirectoryExist())
            Directory.CreateDirectory(Application.persistentDataPath + "/" + directory);
        
        BinaryFormatter binaryFormatter = new BinaryFormatter();
        FileStream file = File.Create(GetFullPath());
       
        await Task.Run(() =>
        {
            All_Save_Objects data = new All_Save_Objects(sObject, sObjectPlayer);
            binaryFormatter.Serialize(file, data);
        });

#if UNITY_EDITOR
        Debug.Log("Save Async!");
#endif
        file.Close();
    }

    public static async Task<All_Save_Objects> Load()
    {
        if (SaveExist())
        {
            try
            {
                BinaryFormatter binaryFormatter = new BinaryFormatter();
                FileStream file = File.Open(GetFullPath(), FileMode.Open);

                await Task.Run(() =>
                {
                    All_Save_Objects saveObject = (All_Save_Objects)binaryFormatter.Deserialize(file);
                    Save_Load_Call.Instance.allSaveObjects = saveObject;
                    
                    file.Close();
                    return saveObject;
                });
            }

            catch
            {
                Debug.Log("Failed to load file!");
            }
        }

        return null;
    }

    private static bool SaveExist()
    {
        return File.Exists(GetFullPath());
    }

    private static bool DirectoryExist()
    {
        return Directory.Exists(Application.persistentDataPath + "/" + directory);
    }

    private static string GetFullPath()
    {
        return Application.persistentDataPath + "/" + directory + "/" + filename;
    }
}
