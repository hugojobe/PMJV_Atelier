using UnityEngine;
public class FilePaths
{
    public static readonly string root = $"{runtimePath}UserData/";

    //Runtime paths
    public static readonly string gameSaves = $"{runtimePath}SavesData/";

    //Resources paths
    public static readonly string backgroundImages = "BG Images/";
    public static readonly string blendTexs = "Transition Effects/";

    public static readonly string resourcesAudio = "Audio/";
    public static readonly string resourcesSFX = $"{resourcesAudio}SFX/";
    public static readonly string resourcesMusic = $"{resourcesAudio}Music/";
    
    public static readonly string resourcesDialogFiles = $"Dialog Files/";

    public static readonly string resourcesFont = $"Fonts/";

    public static string GetPathToResource(string defaultPath, string resourceName) {
        if(resourceName.StartsWith("~/")) {
            return resourceName.Substring(2);
        }

        return defaultPath + resourceName;
    }

    public static string runtimePath {
        get {
            #if UNITY_EDITOR
                return "Assets/appdata/";
            #else
                return Application.persistentDataPath + "/appdata/";
            #endif
        }
    }
}
