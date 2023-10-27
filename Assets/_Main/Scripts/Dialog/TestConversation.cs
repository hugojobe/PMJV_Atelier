using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TestConversation : MonoBehaviour
{
    private void Start() {
        StartCoroutine(Running());
    }

    IEnumerator Running(){
        List<string> lines = new List<string>(){
            "\"AAAAAAAAAAAAAAAAAAA\"",
            "\"B\"",
            "\"CCCCCCCCCCCCCCCCCCCCCCCCCCCCCCCCCCCCC !\""
        };

        yield return DialogueSystem.instance.Say(lines);

        DialogueSystem.instance.dialogContainer.Hide();
    }

    private void Update() {
        List<string> lines = new List<string>();
        Conversation conversation = null;

        if(Input.GetKeyDown(KeyCode.Q)){
            lines = new List<string>(){
            "\"First\"",
            "\"Second\"",
            "\"Third !\""
            };

            conversation = new Conversation(lines);
            DialogueSystem.instance.conversationManager.Enqueue(conversation);
        }

        if(Input.GetKeyDown(KeyCode.W)){
            lines = new List<string>(){
            "\"First Prior\"",
            "\"Second Prior\"",
            };

            conversation = new Conversation(lines);
            DialogueSystem.instance.conversationManager.EnqueuePriority(conversation);
        }
    }
}
