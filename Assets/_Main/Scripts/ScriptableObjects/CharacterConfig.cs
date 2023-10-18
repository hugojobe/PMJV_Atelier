using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Dialog/Character Configuration Asset", fileName = "New Character Asset")]
public class CharacterConfig : ScriptableObject
{
    public CharacterConfigData[] characters;

    public CharacterConfigData GetConfig(string characterName){
        characterName = characterName.ToLower();

        for(int i = 0; i < characters.Length; i++){
            CharacterConfigData data = characters[i];

            if(string.Equals(characterName, data.name.ToLower()) || string.Equals(characterName, data.alias.ToLower())){
                return data.Copy();
            }
        }

        return CharacterConfigData.DefaultData;
    }
}
