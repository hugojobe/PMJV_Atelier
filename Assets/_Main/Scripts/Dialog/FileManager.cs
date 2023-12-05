using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Runtime.InteropServices.WindowsRuntime;
using UnityEditor.VersionControl;
using UnityEngine;

public class FileManager : MonoBehaviour
{
    public static List<string> ReadTextFile(string filePath, bool includeBlakLines = true){
        if(filePath.StartsWith('/'))
            filePath = FilePaths.root + filePath;
        
        List<string> lines = new List<string>();

        try{
            using(StreamReader sr = new StreamReader(filePath)){
                while(!sr.EndOfStream){
                    string line = sr.ReadLine();
                    if(includeBlakLines || !string.IsNullOrWhiteSpace(line))
                        lines.Add(line);
                }
            }
        } catch(FileNotFoundException ex){
            Debug.LogError("File Not Found : " + ex.FileName);
        }

        return lines;
    }

    public static List<string> ReadTextAsset(string filePath, bool includeBlakLines = true){
        TextAsset asset = Resources.Load<TextAsset>(filePath);
        if(asset == null){
            Debug.LogError("Asset Not Found : " + filePath);
            return null;
        }

        return ReadTextAsset(asset, includeBlakLines);
    }

    public static List<string> ReadTextAsset(TextAsset asset, bool includeBlakLines = true){
        List<string> lines = new List<string>();
        using(StringReader sr = new StringReader(asset.text)){
            while(sr.Peek() > -1){
                string line = sr.ReadLine();
                if(includeBlakLines || !string.IsNullOrWhiteSpace(line))
                    lines.Add(line);
            }
        }

        return lines;
    }

    public static bool TryCreateDirectoryFromPath(string path) {
        if(Directory.Exists(path) || File.Exists(path))
            return false;

        if(path.Contains(".")){
            path = Path.GetDirectoryName(path);

            if(Directory.Exists(path))
                return true;
        }

        if(path == string.Empty)
            return false;

        try {
            Directory.CreateDirectory(path);
            return true;
        } catch (System.Exception e){
            Debug.LogError($"Could not create directory ! {e}");
            return false;
        }
    }

    public static void Save(string filePath, string JSonData) {
        if(!TryCreateDirectoryFromPath(filePath)) {
            Debug.LogError($"FAILED TO SAVE FILE '{filePath}'");
            return;
        }

        StreamWriter sw = new StreamWriter(filePath);
        sw.Write(JSonData);
        sw.Close();

        Debug.Log($"Saved data to file '{filePath}'");
    }

    public static T Load<T>(string filePath) {
        if(File.Exists(filePath)) {
            string JSONData = File.ReadAllLines(filePath)[0];
            return JsonUtility.FromJson<T>(JSONData);
        } else {
            Debug.LogError($"Error - File does not exists! '{filePath}'");
            return default(T);
        }
    }
}
