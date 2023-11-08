using UnityEngine;
using UnityEditor;
using System.Collections.Generic;
using System.IO;
using System.Linq;

public class SpriteLoaderEditor : EditorWindow
{
    private string rootFolderPath = "Assets/";
    private CharacterConfig characterConfigFile;
    private int selectedCharacterIndex = 0;
    private Dictionary<string, Sprite> sprites;

    [MenuItem("Tools/Sprite Loader")]
    public static void ShowWindow()
    {
        GetWindow<SpriteLoaderEditor>("Sprite Loader");
    }

    private void OnGUI()
    {
        EditorGUILayout.LabelField("Sprite Loader", EditorStyles.boldLabel);

        EditorGUILayout.BeginHorizontal();
        rootFolderPath = EditorGUILayout.TextField("Root Folder Path:", rootFolderPath);
        if (GUILayout.Button("Browse", GUILayout.Width(80)))
        {
            string selectedFolder = EditorUtility.OpenFolderPanel("Select Folder", rootFolderPath, "");
            if (!string.IsNullOrEmpty(selectedFolder) && selectedFolder.Contains(Application.dataPath))
            {
                rootFolderPath = "Assets" + selectedFolder.Substring(Application.dataPath.Length);
            }
            else if (!string.IsNullOrEmpty(selectedFolder))
            {
                Debug.LogError("The selected folder must be inside the Assets directory!");
            }
        }
        EditorGUILayout.EndHorizontal();

        characterConfigFile = (CharacterConfig)EditorGUILayout.ObjectField("Character Config:", characterConfigFile, typeof(CharacterConfig), false);

        if (characterConfigFile != null && characterConfigFile.characters != null && characterConfigFile.characters.Length > 0)
        {
            string[] characterNames = GetCharacterNames(characterConfigFile.characters);
            selectedCharacterIndex = EditorGUILayout.Popup("Character Name:", selectedCharacterIndex, characterNames);
        }

        if (GUILayout.Button("Load Sprites"))
        {
            LoadSpritesFromFolder(rootFolderPath);
            if (sprites != null)
            {
                Debug.Log($"{sprites.Count} sprites loaded!");
            }
            else
            {
                Debug.Log("No sprites loaded!");
            }
        }

        if (sprites != null && sprites.Count > 0)
        {
            EditorGUILayout.LabelField($"Loaded {sprites.Count} Sprites:");
            foreach (var pair in sprites)
            {
                EditorGUILayout.LabelField($"Key: {pair.Key}, Sprite: {pair.Value.name}");
            }
        }
    }

    private string[] GetCharacterNames(CharacterConfigData[] characters)
    {
        List<string> names = new List<string>();
        foreach (var character in characters)
        {
            names.Add(character.name);
        }
        return names.ToArray();
    }

    private void LoadSpritesFromFolder(string folderPath)
    {
        if (characterConfigFile != null && characterConfigFile.characters != null && selectedCharacterIndex < characterConfigFile.characters.Length)
        {
            string characterName = characterConfigFile.characters[selectedCharacterIndex].name;
            var config = characterConfigFile.GetConfig(characterName, safe: false);

            string[] spritePaths = Directory.GetFiles(folderPath, "*.png", SearchOption.AllDirectories);

            foreach (string path in spritePaths)
            {
                Sprite sprite = AssetDatabase.LoadAssetAtPath<Sprite>(path);
                if (sprite != null)
                {
                    string keyName = path.Replace(rootFolderPath, "").Replace(".png", "").ToLower();
                    string fullKeyName = keyName.Substring(1); // Removing the starting "/"
                    config.sprites[fullKeyName] = sprite;
                }
            }

            sprites = config.sprites;
        }
        else
        {
            Debug.LogWarning("Invalid CharacterConfigSO or character selection.");
        }
    }
}