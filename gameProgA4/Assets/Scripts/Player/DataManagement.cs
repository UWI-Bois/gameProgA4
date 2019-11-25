using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;

public class DataManagement : MonoBehaviour
{
    public static DataManagement dataManagement; // static makes the variable accessable through the entire thing

    public int highScore;

    private void Awake() // this predefined function happens before Start()
    {
        if(dataManagement == null)
        {
            DontDestroyOnLoad(gameObject);
            dataManagement = this;
        }
        else if (dataManagement != this) {
            Destroy(gameObject);
        }
    }

    public void SaveData()
    {
        BinaryFormatter binForm = new BinaryFormatter(); // binary formatter
        FileStream file = File.Create(Application.persistentDataPath + "/gameInfo.dat"); // this path stays when you update the app
        GameData data = new GameData();
        data.highScore = highScore;
        // TODO find player name, update scores and store data etc
        binForm.Serialize(file, data); // serializes
        file.Close();
    }

    public void LoadData()
    {
        if(File.Exists(Application.persistentDataPath + "/gameInfo.dat"))
        {
            BinaryFormatter binForm = new BinaryFormatter();
            FileStream file = File.Open(Application.persistentDataPath + "/gameInfo.dat", FileMode.Open);
            GameData data = (GameData)binForm.Deserialize(file);
            file.Close();
            highScore = data.highScore;
        }
    }
}

[Serializable]
class GameData
{
    // TODO: player name and id?
    public int highScore;

}
