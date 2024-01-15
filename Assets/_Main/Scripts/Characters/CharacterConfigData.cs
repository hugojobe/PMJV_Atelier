using System;
using AYellowpaper.SerializedCollections;
using TMPro;
using UnityEngine;

[Serializable]
public class CharacterConfigData
{
    public string name;
    public string alias;
    public Character.CharacterTypes characterType;

    public Color nameColor;
    public Color dialogColor;
    public TMP_FontAsset nameFont;
    public TMP_FontAsset dialogFont;
    public float fontSize;

    [SerializedDictionary("Path / ID", "Sprite")]
    public SerializedDictionary<string, Sprite> sprites = new SerializedDictionary<string, Sprite>();

    public CharacterConfigData Copy(){
        CharacterConfigData result = new CharacterConfigData();

        result.name = name;
        result.alias = alias;
        result.characterType = characterType;
        result.nameFont = nameFont;
        result.dialogFont = dialogFont;

        result.nameColor = new Color(nameColor.r, nameColor.g, nameColor.b);
        result.dialogColor = new Color(dialogColor.r, dialogColor.g, dialogColor.b);

        result.fontSize = fontSize;

        result.sprites = sprites;
    
        return result;
    }

    public static Color defaultColor => DialogueSystem.instance.config.defautTextColor;
    public static TMP_FontAsset defaultFont => DialogueSystem.instance.config.defaultFont;

    public static CharacterConfigData DefaultData{
        get{
            CharacterConfigData result = new CharacterConfigData();

            result.name = "";
            result.alias = "";
            result.characterType = Character.CharacterTypes.Text;
            result.nameFont = defaultFont;
            result.dialogFont = defaultFont;

            result.nameColor = new Color(defaultColor.r, defaultColor.g, defaultColor.b);
            result.dialogColor = new Color(defaultColor.r, defaultColor.g, defaultColor.b);

            result.fontSize = DialogueSystem.instance.config.defaultFontSize;

            result.sprites = new SerializedDictionary<string, Sprite>();
    
            return result;
        }
    }
}
