using UnityEngine;
using System.IO;

[System.Serializable]
public struct SaveData
{
    public int highestLevel;
}

public static class SaveSystem
{
    private static string SavePath => Path.Combine(Application.persistentDataPath, "save.dat");

    public static void SaveGame(int highestLevel)
    {
        SaveData data = new SaveData
        {
            highestLevel = highestLevel
        };

        //create, open and close a stream
        using (FileStream stream = new FileStream(SavePath, FileMode.Create))
        {
            //write to stream in binary
            using (BinaryWriter writer = new BinaryWriter(stream))
            {
                writer.Write(data.highestLevel);
            }
        }

        Debug.Log("Game saved: " + SavePath);
    }

    public static SaveData LoadGame()
    {
        //if there is no save file, create a new one
        if (!File.Exists(SavePath))
        {
            Debug.Log("No save file found.");

            return new SaveData
            {
                highestLevel = 1
            };
        }

        try
        {
            using (FileStream stream = new FileStream(SavePath, FileMode.Open))
            {
                //read the binary stream
                using (BinaryReader reader = new BinaryReader(stream))
                {
                    SaveData data = new SaveData();

                    data.highestLevel = reader.ReadInt32();

                    return data;
                }
            }
        }
        catch (System.Exception e)
        {
            Debug.LogError("Failed to load save: " + e.Message);

            return new SaveData
            {
                highestLevel = 1
            };
        }
    }

    //check if a save file exists at the save path
    public static bool HasSave()
    {
        return File.Exists(SavePath);
    }

    public static void DeleteSave()
    {
        if (File.Exists(SavePath))
        {
            File.Delete(SavePath);
            Debug.Log("Save deleted.");
        }
    }
}
