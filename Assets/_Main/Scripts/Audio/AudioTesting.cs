using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AudioTesting : MonoBehaviour
{
    private void Start() {
        StartCoroutine(Running());
    }

    Character CreateCharacter(string name) => CharacterManager.instance.CreateCharacter(name);

    private IEnumerator Running(){
        CharacterSprite perso1 = CreateCharacter("Personnage1") as CharacterSprite;
        perso1.Show();

        yield return new WaitForSecondsRealtime(0.5f);

        //AudioManager.instance.PlaySoundEffect("Audio/SFX/thunder_01");
        AudioManager.instance.PlayTrack("Audio/Music/Upbeat");

        perso1.Say("Hi");

        yield return new WaitForSecondsRealtime(4f);

        AudioManager.instance.PlayTrack("Audio/Music/Happy");
    }
}
