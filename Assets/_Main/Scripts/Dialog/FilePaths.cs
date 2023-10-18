using UnityEngine;

public class FilePaths
{
    public static readonly string root = $"{Application.dataPath}/gameData/";

    //Resources paths
    public static readonly string backgroundImages = "BG Images/";
    public static readonly string blendTexs = "Transition Effects/";

    public static readonly string resourcesAudio = "Audio/";
    public static readonly string resourcesSFX = $"{resourcesAudio}SFX/";
    public static readonly string ResourcesMusic = $"{resourcesAudio}Music/";
}
