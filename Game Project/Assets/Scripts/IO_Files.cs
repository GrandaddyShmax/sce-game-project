using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;
using System.Runtime.Serialization;

//all the loading and saving functions
public static class IO_Files
{
    public static string ReadString(string path)  //load string from file
    {
        string data = null;
        if (File.Exists(path))
        {
            BinaryFormatter formatter = new BinaryFormatter();
            FileStream stream = new FileStream(path, FileMode.Open);
            data = formatter.Deserialize(stream) as string;
            Debug.Log(data);
            stream.Close();     //opened files must be closed when done with
        }
        return data;
    }
    public static Map_Display.Data_Player ReadData(string path)  //load Data_Player from file
    {
        Map_Display.Data_Player data = null;
        if (File.Exists(path))
        {
            BinaryFormatter formatter = GetBinaryFormatter();
            FileStream stream = new FileStream(path, FileMode.Open);
            data = formatter.Deserialize(stream) as Map_Display.Data_Player;
            stream.Close();     //opened files must be closed when done with
        }
        return data;
    }
    public static void WriteFile(string path, string data)  //save string to file
    {
        Debug.Log("Attempting to create");
        BinaryFormatter formatter = new BinaryFormatter();
        FileStream stream = new FileStream(path, FileMode.Create);
        formatter.Serialize(stream, data);
        Debug.Log("File created");
        stream.Close();         //opened files must be closed when done with
    }

    public static void WriteData(string path, Map_Display.Data_Player data)  //save the player's data to file
    {
        Debug.Log("Attempting to create");
        BinaryFormatter formatter = GetBinaryFormatter();
        FileStream stream = new FileStream(path, FileMode.Create);
        formatter.Serialize(stream, data);
        Debug.Log("File created");
        stream.Close();         //opened files must be closed when done with
    }

    public static void DeleteData(string path)  //delete the file
    {
        Debug.Log("Attempting to delete");
        File.Delete(path);
        Debug.Log("File deleted");
    }

    public static BinaryFormatter GetBinaryFormatter()
    {
        BinaryFormatter formatter = new BinaryFormatter();
        SurrogateSelector selector = new SurrogateSelector();

        Save_SurrogateVec3 surrogateVec3 = new Save_SurrogateVec3();
        Save_SurrogateQuat surrogateQuat = new Save_SurrogateQuat();
        Save_SurrogateColor surrogateColor = new Save_SurrogateColor();

        //Replace type with serializable surrogate.
        selector.AddSurrogate(typeof(Vector3), new StreamingContext(StreamingContextStates.All), surrogateVec3); //Vector3
        selector.AddSurrogate(typeof(Quaternion), new StreamingContext(StreamingContextStates.All), surrogateQuat); //Quaterion
        selector.AddSurrogate(typeof(Color), new StreamingContext(StreamingContextStates.All), surrogateColor); //Color

        formatter.SurrogateSelector = selector;
        return formatter;
    }
}
