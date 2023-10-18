using System.Collections;
using System.Collections.Generic;
using System.IO;
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
}
