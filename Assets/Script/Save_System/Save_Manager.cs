using UnityEngine;
using System.IO;
using System.Runtime.Serialization;
using System.Runtime.Serialization.Formatters.Binary;

public static class Save_Manager
{
    public static string directory = "SaveData";
    public static string filename = "Save.txt";

    public static void Save(Save_Object saveObject)
    {
        if (!DirectoryExist())
            Directory.CreateDirectory(Application.persistentDataPath + "/" + directory);
        
        BinaryFormatter binaryFormatter = new BinaryFormatter();
        FileStream file = File.Create(GetFullPath());
        binaryFormatter.Serialize(file, saveObject);
        file.Close();
    }

    public static Save_Object Load()
    {
        if (SaveExist())
        {
            try
            {
                BinaryFormatter binaryFormatter = new BinaryFormatter();
                FileStream file = File.Open(GetFullPath(), FileMode.Open);
                Save_Object saveObject = (Save_Object)binaryFormatter.Deserialize(file);
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
