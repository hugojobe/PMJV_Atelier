using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class CharacterData
{
    public string characterName;
    public string displayName;
    public bool enabled;
    public Vector2 position;
    public CharacterConfigCache characterConfig;

    public string dataJSON;

    [System.Serializable]
    public class CharacterConfigCache {
        public string name;
        public string alias;

        public Character.CharacterTypes characterType;
        public Color nameColor;
        public Color dialogColor;

        public string nameFont;
        public string dialogFont;

        public float nameFontScale = 1f;
        public float dialogFontScale = 1f;

        public CharacterConfigCache(CharacterConfigData reference) {
            name = reference.name;
            alias = reference.alias;
            characterType = reference.characterType;
            
            nameColor = reference.nameColor;
            dialogColor = reference.dialogColor;

            nameFont = FilePaths.resourcesFont + reference.nameFont;
            dialogFont = FilePaths.resourcesFont + reference.dialogFont;

            nameFontScale = reference.fontSize;
            dialogFontScale = reference.fontSize;
        }
    }

    public static List<CharacterData> Capture() {
        List<CharacterData> characters = new List<CharacterData>();

        foreach(var character in CharacterManager.instance.characters.Values) {
            if(!character.isVisible) continue;

            CharacterData entry = new CharacterData();
            entry.characterName = character.name;
            entry.displayName = character.displayName;
            entry.enabled = character.isVisible;
            entry.position = character.targetPosition;

            entry.characterConfig = new CharacterConfigCache(character.config);
        
            if(character.config.characterType == Character.CharacterTypes.Sprite) {
                SpriteData spriteData = new SpriteData();

                CharacterSprite sc = character as CharacterSprite;
                
                spriteData.spriteName = sc.GetLayer().sprite.name;
                spriteData.color = sc.GetLayer().color;
            
                entry.dataJSON = JsonUtility.ToJson(spriteData);
            }

            characters.Add(entry);
        }

        return characters;
    }

    [System.Serializable]
    public class SpriteData {
        public string spriteName;
        public Color color;
    }
}
