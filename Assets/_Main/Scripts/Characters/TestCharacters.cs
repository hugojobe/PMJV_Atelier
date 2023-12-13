using System.Collections;
using UnityEngine;

public class TestCharacters : MonoBehaviour
{
    private void Start() {
        //Character Perso1 = CharacterManager.instance.CreateCharacter("Personnage 1");

        StartCoroutine(Test());
    }

    IEnumerator Test(){
        yield return new WaitForSeconds(1f);

        CharacterSprite Perso1 = CharacterManager.instance.CreateCharacter("Personnage 1") as CharacterSprite;

        Perso1.SetPosition(Vector2.zero);

        yield return Perso1.Show();
        
        yield return new WaitForSeconds(1f);

        yield return Perso1.TransitionSprite(Perso1.GetSprite("CH 2"));
        
        yield return new WaitForSeconds(1f);

        yield return Perso1.TransitionSprite(Perso1.GetSprite("CH 1"));
        
        yield return Perso1.Say("Hello");

    }
}
