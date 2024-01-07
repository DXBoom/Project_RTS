using UnityEngine;

public class Save_Load_Call : MonoBehaviour
{
    public Save_Object saveObject;
    
    public void Save()
    {
        Save_Manager.Save(saveObject);
    }

    public void Load()
    {
        saveObject = Save_Manager.Load();
    }
}
