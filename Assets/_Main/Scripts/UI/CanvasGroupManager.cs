using System.Collections;
using UnityEngine;

public class CanvasGroupManager
{
    private CanvasGroup rootCG;
    private MonoBehaviour executorScript;

    public bool isVisible { 
        get{return rootCG.alpha == 1;}
        set{rootCG.alpha = value ? 1 : 0;}
    }

    public CanvasGroupManager(CanvasGroup rootCG, MonoBehaviour executorScript) {
        this.rootCG = rootCG;
        this.executorScript = executorScript;
    }

    public void Show() {
        executorScript.StartCoroutine(ShowOrHideCoroutine(1f));
    }

    public void Hide() {
        executorScript.StartCoroutine(ShowOrHideCoroutine(0f));
    }

    public IEnumerator ShowOrHideCoroutine(float targetAlpha){
        //Debug.Log($"Showing or hiding '{rootCG.name}'");
        CanvasGroup self = rootCG;

        while(self.alpha != targetAlpha){
            self.alpha = Mathf.MoveTowards(self.alpha, targetAlpha, 3f * Time.deltaTime * 1f);
            yield return null;
        }
    }


}
