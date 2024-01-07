using UnityEngine;
using System.IO;
using System.Runtime.Serialization;
using System.Runtime.Serialization.Formatters.Binary;

public static class Save_Manager
{
    public static string directory = "SaveData";
    public static string filename = "Save.txt";

    public static void Save(Save_Object[] sObject)
    {
        if (!DirectoryExist())
            Directory.CreateDirectory(Application.persistentDataPath + "/" + directory);
        
        BinaryFormatter binaryFormatter = new BinaryFormatter();
        FileStream file = File.Create(GetFullPath());
        All_Save_Objects data = new All_Save_Objects(sObject);
        binaryFormatter.Serialize(file, data);
        file.Close();
    }

    public static All_Save_Objects Load()
    {
        if (SaveExist())
        {
            try
            {
                BinaryFormatter binaryFormatter = new BinaryFormatter();
                FileStream file = File.Open(GetFullPath(), FileMode.Open);
                All_Save_Objects saveObject = (All_Save_Objects)binaryFormatter.Deserialize(file);
                file.Close();

                return saveObject;
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
