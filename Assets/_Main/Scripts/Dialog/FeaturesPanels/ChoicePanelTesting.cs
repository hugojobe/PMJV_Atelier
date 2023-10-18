using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ChoicePanelTesting : MonoBehaviour
{
    ChoicePanel panel;

    private void Start() {
        panel = ChoicePanel.instance;

        string[] choices = new string[]{
            "Choix 111111111111 111111",
            "Choix 2",
            "Choix 311111111 11111111111111",
            "Choix 4111111111 1111111 1111111111111 11111111111111111 111111111111 11111111111111111111 111111111111111 111111"
        };

        panel.Show("Question 1", choices);
    }
}
