using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GraphicPanelManager : MonoBehaviour
{
    public static GraphicPanelManager instance {get; private set;}

    public GraphicPanel[] allPanels;

    public const float defaultTransitionSpeed = 3f;

    private void Awake() {
        instance = this;
    }

    public GraphicPanel GetPanel(string name){
        name = name.ToLower();
        foreach(GraphicPanel panel in allPanels){
            if(panel.panelName.ToLower() == name){
                return panel;
            }
        }

        return null;
    }
}
