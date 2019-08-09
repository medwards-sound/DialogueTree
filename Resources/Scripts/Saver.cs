/******************************************************************
	Saver.cs

    Allows saving of either a single serializable data type
    or a list of a serializable data type.
******************************************************************/

using System.Collections;
using System.Collections.Generic;
using System.Runtime.Serialization.Formatters.Binary;
using System.IO;
using UnityEngine;

public class Saver
{
    public enum saveType {dialogue, state}

    //saves list of serializable data
    public virtual void Save<T>(List<T> data, saveType givenSaveType, string fileName)
    {
        string path = GetPath(givenSaveType, fileName);
        
        BinaryFormatter bf = new BinaryFormatter();
        FileStream fs = File.Create(path);
        bf.Serialize(fs, data);
        fs.Close();
    }

    //saves single serializable data type
    public virtual void SaveSingle<T>(T data, saveType givenSaveType, string fileName)
    {
        BinaryFormatter bf = new BinaryFormatter();
        string path = GetPath(givenSaveType, fileName);
        FileStream fs = File.Create(path);
        bf.Serialize(fs, data);
        Debug.Log("Single Save," + path);
        fs.Close();
    }

    //loads list of serializable data types from file
    public virtual List<T> Load<T>(saveType givenSaveType, string fileName)
    {
        List<T> ret = null;
        string path = GetPath(givenSaveType, fileName);

        if(File.Exists(path))
        {
            BinaryFormatter bf = new BinaryFormatter();
            FileStream fs = File.Open(path, FileMode.Open);
            ret = (List<T>)bf.Deserialize(fs);
            fs.Close();
        }

        return ret;
    }

    //loads single serializable data type from file
    public virtual T LoadSingle<T>(saveType givenSaveType, string fileName)
    {
        T ret = default(T);
        string path = GetPath(givenSaveType, fileName);
        Debug.Log("LS LOAD, " + path);

        if(File.Exists(path))
        {
            BinaryFormatter bf = new BinaryFormatter();
            FileStream fs = File.OpenRead(path);
            Debug.Log("LS READ OK");
            ret = (T)bf.Deserialize(fs);
            // DialogueTree dt = ret;

            fs.Close();
        }

        return ret;
    }

    //assemebles path of file from filename, based on type of save
    static string GetPath(saveType givenSaveType, string fileName)
    {
        string path = null;
        
        switch(givenSaveType)
        {
            case saveType.dialogue:
                path = Application.dataPath + "/StreamingAssets/Dialogue/" + fileName + ".dat"; //hard coded
                break;

            case saveType.state:
                path = Application.persistentDataPath + "/" + fileName + ".dat";
                break;
        }
        
        return path;
    }


    //returns T if file exists
    public static bool CheckForFile(saveType givenSaveType, string fileName)
    {
        return
            File.Exists(GetPath(givenSaveType, fileName));
    }

}
