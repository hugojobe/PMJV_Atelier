using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CharacterManager : MonoBehaviour
{
    public const bool ENABLE_ON_START = false;
    public static CharacterManager instance;
    public Dictionary<string, Character> characters = new Dictionary<string, Character>();
    
    public CharacterConfig config => DialogueSystem.instance.config.characterConfig;

    private const string CHARACTER_CASTING_ID = " as ";
    private const string CHARACTER_NAME_ID = "<charname>";
    public string characterRootPathFormat => $"Characters/{CHARACTER_NAME_ID}";
    public string characterPrefabNameFormat = $"Character - [{CHARACTER_NAME_ID}]";
    public string characterPrefabPathFormat => $"{characterRootPathFormat}/{characterPrefabNameFormat}";

    [SerializeField] private RectTransform _characterPanel = null;
    public RectTransform characterPanel => _characterPanel;
    
    private void Awake() {
        instance = this;
    }

    public CharacterConfigData GetCharacterConfig(string characterName){
        return config.GetConfig(characterName);
    }

    public Character GetCharacter(string characterName, bool createIfDoesNotExists = false){
        if(characters.ContainsKey(characterName.ToLower())){
            return characters[characterName.ToLower()];
        } else {
            if(createIfDoesNotExists){
                return CreateCharacter(characterName);
            }
        }

        return null;
    }

    public bool HasCharacter(string characterName) => characters.ContainsKey(characterName.ToLower());

    public Character CreateCharacter(string characterName, bool revealAfterCreation = false){
        if(characters.ContainsKey(characterName.ToLower())){
            Debug.LogWarning($"A character named '{characterName}' already exists. Did not created this character");
            return null;
        }

        CharacterInfo info = GetCharacterInfo(characterName);

        Character character = CreateCharacterFromInfo(info);

        characters.Add(characterName.ToLower(), character);

        if(revealAfterCreation)
            character.Show();

        return character;
    }

    public CharacterInfo GetCharacterInfo(string characterName){
        CharacterInfo result = new CharacterInfo();

        string[] nameData = characterName.Split(CHARACTER_CASTING_ID, System.StringSplitOptions.RemoveEmptyEntries);

        result.name = nameData[0];
        result.castingName = (nameData.Length > 1) ? nameData[1] : result.name;

        result.config = config.GetConfig(result.castingName);

        result.prefab = GetPrefab(result.castingName);

        result.rootcharacterFolder = FormatCharacterPath(characterRootPathFormat, result.castingName);

        return result;
    }

    public GameObject GetPrefab(string characterName){
        string prefabPath = FormatCharacterPath(characterPrefabPathFormat, characterName);
        //Debug.Log($"Prefab path : {prefabPath}");
        return Resources.Load<GameObject>(prefabPath);
    }

    public string FormatCharacterPath(string path, string characterName) => path.Replace(CHARACTER_NAME_ID, characterName);

    public Character CreateCharacterFromInfo(CharacterInfo info){
        CharacterConfigData config = info.config;
        //Debug.Log("info.config name = " + config.name);

        if(config.characterType == Character.CharacterTypes.Text){
            return new CharacterText(info.name, config);
        }
        if(config.characterType == Character.CharacterTypes.Sprite){
            return new CharacterSprite(info.name, config, info.prefab, info.rootcharacterFolder);
        }

        return null;
    }

    public class CharacterInfo{
        public string name = "";
        public string castingName = "";
        public CharacterConfigData config = null;
        public GameObject prefab;
        public string rootcharacterFolder = "";
    }
}
