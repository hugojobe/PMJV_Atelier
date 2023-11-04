using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class ExtensionCharacters : CommandDatabaseExtension
{
    private static string[] PARAM_IMMEDIATE => new string[]{"-i", "-immediate"};
    private static string[] PARAM_ENABLE => new string[]{"-e", "-enable"};
    private static string PARAM_XPOS => "-x";
    private static string PARAM_YPOS => "-y";
    private static string[] PARAM_SPEED => new string[]{"-spd", "-speed"};
    private static string[] PARAM_SMOOTHING => new string[]{"-s", "-smooth"};
    private static string[] PARAM_SPRITE => new string[]{"-spr", "-sprite"};

    new public static void Extend(CommandDatabase database){
        database.AddCommand("createcharacter", new Action<string[]>(CreateCharacter));
        database.AddCommand("show", new Func<string[], IEnumerator>(ShowAll));
        database.AddCommand("hide", new Func<string[], IEnumerator>(HideAll));
        database.AddCommand("move", new Func<string[], IEnumerator>(MoveCharacter));
        database.AddCommand("setsprite", new Func<string[], IEnumerator>(SetSprite));
    }

    public static void CreateCharacter(string[] data){    
        string characterName = data[0];
        bool enable = false;
        bool immediate = false;

        CommandParameters parameters = ConvertDataToParameters(data);
        parameters.TryGetValue(PARAM_ENABLE, out enable, defaultValue: true);
        parameters.TryGetValue(PARAM_IMMEDIATE, out immediate, defaultValue: false);

        Character character = CharacterManager.instance.CreateCharacter(characterName);

        if(!enable)
            return;
        
        if(immediate){
            character.isVisible = true;
        } else {
            character.Show();
        }

    }

    private static IEnumerator MoveCharacter(string[] data){
        string characterName = data[0];
        Character character = CharacterManager.instance.GetCharacter(characterName);

        if(character == null) yield break;

        float x = 0, y = 0;
        float speed = 2;
        bool smoothing = false;
        bool immediate = false;

        CommandParameters parameters = ConvertDataToParameters(data);
        parameters.TryGetValue(PARAM_XPOS, out x);
        parameters.TryGetValue(PARAM_YPOS, out y);

        parameters.TryGetValue(PARAM_SPEED, out speed, defaultValue: 1f);
        parameters.TryGetValue(PARAM_SMOOTHING, out smoothing, defaultValue: true);
        parameters.TryGetValue(PARAM_IMMEDIATE, out immediate);

        Vector2 position = new Vector2(x, y);

        if(immediate)
            character.SetPosition(position);
        else{
            CommandManager.instance.AddTerminationActionToCurrentProcess(() => {character?.SetPosition(position);});
            yield return character.MoveToPosition(position, speed);
        }
    }

    public static IEnumerator ShowAll(string[] data){
        List<Character> characters = new List<Character>();
        bool immediate = false;
        float speed = 1f;

        foreach(string s in data){
            Character character = CharacterManager.instance.GetCharacter(s, false);
            if(character != null)
                characters.Add(character);
        }

        if(characters.Count == 0)
            yield break;

        CommandParameters parameters = ConvertDataToParameters(data);

        parameters.TryGetValue(PARAM_IMMEDIATE, out immediate, defaultValue: false);
        parameters.TryGetValue(PARAM_SPEED, out speed, defaultValue: 1f);

        foreach(Character character in characters){
            if(immediate)
                character.isVisible = true;
            else
                character.Show(speed);
        }

        if(!immediate){
            CommandManager.instance.AddTerminationActionToCurrentProcess(() => {
                foreach(Character character in characters)
                    character.isVisible = true;    
            });

            while(characters.Any(c => c.isRevealing))
                yield return null;
        }
    }

    public static IEnumerator HideAll(string[] data){
        List<Character> characters = new List<Character>();
        bool immediate = false;
        float speed = 1f;

        foreach(string s in data){
            Character character = CharacterManager.instance.GetCharacter(s, false);
            if(character != null)
                characters.Add(character);
        }

        if(characters.Count == 0)
            yield break;

        CommandParameters parameters = ConvertDataToParameters(data);

        parameters.TryGetValue(PARAM_IMMEDIATE, out immediate, false);
        parameters.TryGetValue(PARAM_SPEED, out speed, defaultValue: 1f);

        foreach(Character character in characters){
            if(immediate)
                character.isVisible = false;
            else
                character.Hide(speed);
        }


        if(!immediate){
            CommandManager.instance.AddTerminationActionToCurrentProcess(() => {
                foreach(Character character in characters)
                    character.isVisible = false;    
            });

            while(characters.Any(c => c.isHiding))
                yield return null;
        }
    }

    public static IEnumerator SetSprite(string[] data){
        string characterName = data[0];
        CharacterSprite character = (CharacterSprite) CharacterManager.instance.GetCharacter(characterName);

        if(character == null) yield break;

        float speed = 1f;
        bool immediate = false;
        string spriteName = "";

        CommandParameters parameters = ConvertDataToParameters(data);
        parameters.TryGetValue(PARAM_IMMEDIATE, out immediate, defaultValue: false);
        parameters.TryGetValue(PARAM_SPEED, out speed, defaultValue: 1f);
        parameters.TryGetValue(PARAM_SPRITE, out spriteName, defaultValue: "");

        Sprite sprite = character.GetSprite(spriteName);

        if(immediate)
            character.TransitionSprite(sprite, 10000);
        else
            character.TransitionSprite(sprite, speed);
    }
}
