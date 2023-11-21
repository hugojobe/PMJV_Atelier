using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class GraphicData
{
    public string panelName;
    public List<LayerData> layers;

    [System.Serializable]
    public class LayerData {
        public int depth;
        public string graphicName;
        public string graphicPath;
        public bool isVideo;
        public bool useAudio;

        public LayerData(GraphicLayer layer) {
            depth = layer.layerDepth;

            if(layer.currentGraphic == null)
                return;

            var graphic = layer.currentGraphic;

            graphicName = graphic.graphicName;
            graphicPath = graphic.graphicPath;
            isVideo = graphic.isVideo;
            useAudio = graphic.useAudio;
        }
    }

    public static List<GraphicData> Capture() {
            List<GraphicData> graphicPanels = new List<GraphicData>();

            foreach(var panel in GraphicPanelManager.instance.allPanels) {
                if(panel.isClear) continue;

                GraphicData data = new GraphicData();
                data.panelName = panel.panelName;
                data.layers = new List<LayerData>();

                foreach(var layer in panel.layers){
                    LayerData entry = new LayerData(layer);
                    data.layers.Add(entry);
                }

                graphicPanels.Add(data);
            }

            return graphicPanels;
        }
}
