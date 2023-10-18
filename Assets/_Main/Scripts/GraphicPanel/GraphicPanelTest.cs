using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GraphicPanelTest : MonoBehaviour
{
    private void Start() {
        GraphicPanel panel = GraphicPanelManager.instance.GetPanel("Background");
        GraphicLayer layer = panel.GetLayer(0, true);

        Texture blendTex = Resources.Load<Texture>("Transition Effects/feathers");
        layer.SetTexture("BG Images/BG3", blendingTexture: blendTex);
    }
}
